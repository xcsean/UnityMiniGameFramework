using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace UnityMiniGameFramework
{
    public class ClientSocket
    {
        private static int ReceiveSizeWaring = 1024 * 16;
        private static object RecLocker = new object();
        private bool handleMsgLimit = true;

        private int maxCountPerFrame = 128;
        private int minCountPerFrame = 32;
        private int recvieArrLen = 32;
        private int _perFrameRate = 3;
        private ProtoBytes[] recvieArr = new ProtoBytes[32];

        public delegate void ProtoCallbackEvent(uint id, ByteBuffer proto);

        public delegate void ReconnectionEvent();

        ProtoCallbackEvent receiveCallback = null;
        ReconnectionEvent reconnectionEvent = null;
        static ClientSocket instance;

        Thread receiveThread;
        Socket socket;
        bool _connected;
        string host;
        int port;

        ProtoMsg mSendProto = new ProtoMsg();
        ProtoMsg mReceiveProto = new ProtoMsg();

        private Queue<ProtoBytes> mReceiveQueue = new Queue<ProtoBytes>();
        private MyPool<ProtoBytes> ProtoBytesPool;

        int packSize = -1;
        int receiveingMsgLen = 0;

        public void SetHandleMsg(bool handleLimit, int maxCountPerFrame, int minCountPerFrame, int perFrameRate)
        {
            this.handleMsgLimit = handleLimit;
            this.maxCountPerFrame = maxCountPerFrame;
            this.minCountPerFrame = minCountPerFrame;
            this._perFrameRate = perFrameRate;
        }

        public void SetProtoHeader(UInt64 uid)
        {
            mSendProto.Uid = uid;
        }

        private ProtoBytes NewProtoByte()
        {
            return new ProtoBytes();
        }

        ClientSocket()
        {
            if (ClientSocket.instance != null)
            {
                throw new Exception("单例实例化错误!");
            }

            ClientSocket.instance = this;
        }

        public static ClientSocket GetInstance()
        {
            return ClientSocket.instance ?? new ClientSocket();
        }

        public bool IsConnected
        {
            get { return _connected; }
        }

        public void Init()
        {
            ProtoBytesPool = new MyPool<ProtoBytes>(NewProtoByte);
        }

        public void Dispose()
        {
            Closed();
            receiveCallback = null;
        }

        public void Closed()
        {
            packSize = -1;
            _connected = false;
            try
            {
                if (receiveThread != null && receiveThread.ThreadState == ThreadState.Running) receiveThread.Abort();
                receiveThread = null;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }

            try
            {
                if (socket != null && socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    socket = (Socket) null;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }

            while (mReceiveQueue.Count > 0) ProtoBytesPool.Store(mReceiveQueue.Dequeue());
        }

        public void SetIp(string host, int port)
        {
            this.host = host;
            this.port = port;
        }

        public void Connect()
        {
            Closed();
            Debug.LogWarningFormat("开始连接服务器...host={0},port={1}", host, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 30000);
            socket.BeginConnect(host, port, OnConnect, socket);
        }

        void OnConnect(IAsyncResult iAsyncResult)
        {
            Socket asyncState = (Socket) iAsyncResult.AsyncState;
            if (asyncState != socket)
                return;
            try
            {
                asyncState.EndConnect(iAsyncResult);
            }
            catch (Exception ex)
            {
                OnError(asyncState, "ConnectCallback:" + ex.Message + "\n" + ex.StackTrace);
                throw;
            }

            _connected = true;
            Debug.LogWarningFormat("服务器连接成功...host={0},port={1}", host, port);
        }

        void OnError(Socket socket_1, string error)
        {
            Debug.LogError(error);
            Closed();
        }

        void Receive()
        {
            while (_connected)
            {
                Receiveing();
                Thread.Sleep(15);
            }
        }

        void Receiveing()
        {
            try
            {
                if (packSize == -1)
                {
                    if (socket.Available < ProtoMsg.HeaderSize) return;
                    socket.Receive(mReceiveProto.Bytes, 0, ProtoMsg.HeaderSize, SocketFlags.None);
                    mReceiveProto.ReadHeader();
                    receiveingMsgLen = 0;
                }

                int ava = socket.Available;
                packSize = (int) mReceiveProto.BodyLen;
                if (ava > 0)
                {
                    int receiveSize = Math.Min(ava, packSize - receiveingMsgLen);
                    socket.Receive(mReceiveProto.Bytes, receiveingMsgLen, receiveSize, SocketFlags.None);
                    receiveingMsgLen = receiveingMsgLen + receiveSize;
                }

                if (receiveingMsgLen < packSize) return;
            }
            catch (Exception ex)
            {
                OnError(socket, "ReceiveSorket: " + ex.Message + "\n" + ex.StackTrace);
            }

            try
            {
                ProtoBytes p = ProtoBytesPool.New().SetProtoMsg(mReceiveProto);
                lock (RecLocker) mReceiveQueue.Enqueue(p);
                if (packSize > ReceiveSizeWaring)
                    Debug.LogWarningFormat("msg size warning, packSize={0}", packSize);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                throw;
            }

            packSize = -1;
            receiveingMsgLen = 0;
            if (socket.Available > 0)
                Receiveing();
        }

        public void Send(uint cmd, byte[] serializedString)
        {
            if (!_connected) return;
            mSendProto.Cmd = cmd;
            mSendProto.WriteProto(serializedString);
            SendMsg();
        }

        void SendMsg()
        {
            try
            {
                socket.Send(mSendProto.Bytes, mSendProto.Size, SocketFlags.None);
            }
            catch (Exception ex)
            {
                reconnectionEvent();
                OnError(null, "SendMsg: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public void Run()
        {
            HandleMsg();
        }

        void _ResetRecvieArrLen(int count)
        {
            recvieArrLen = count + count / 3;
            Array.Resize(ref recvieArr, recvieArrLen);
        }

        void HandleMsg()
        {
            int num = 0;
            lock (RecLocker)
            {
                int count = mReceiveQueue.Count;
                if (count == 0) return;
                if (handleMsgLimit)
                {
                    if (count > minCountPerFrame)
                    {
                        count = count / _perFrameRate;
                        if (count < minCountPerFrame)
                        {
                            count = minCountPerFrame;
                        }
                        else if (count > maxCountPerFrame)
                        {
                            count = maxCountPerFrame;
                        }
                    }
                }

                if (count > recvieArrLen) _ResetRecvieArrLen(count);
                for (; num < count; num++)
                {
                    recvieArr[num] = mReceiveQueue.Dequeue();
                }
            }

            for (int index = 0; index < num; index++)
            {
                var protoMsg = recvieArr[index];
                recvieArr[index] = null;
                uint protoId = protoMsg.ProtoId;
                if (receiveCallback != null)
                {
                    try
                    {
                        receiveCallback(protoId, new ByteBuffer(protoMsg.Bytes, protoMsg.Size));
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }
                    finally
                    {
                        ProtoBytesPool.Store(protoMsg);
                    }
                }
            }
        }

        public void SetReceiveCallback(ProtoCallbackEvent callback)
        {
            this.receiveCallback = callback;
        }

        public void SetReconnectionCallback(ReconnectionEvent callback)
        {
            this.reconnectionEvent = callback;
        }

        public static int ReadBytes(byte[] bytes, int offset, int count)
        {
            int num1 = 0;
            int num2 = offset + count;
            for (int index = offset; index < num2; ++index)
                num1 = num1 * 256 + bytes[index];
            return num1;
        }
    }
}