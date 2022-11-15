using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

using Ionic.Zlib;

namespace UnityMiniGameFramework
{
    class ConnectorWebSocket : IConnector
    {
        protected ClientWebSocket _webSock;
        
        protected IConnectorHandler _handler;
        protected uint _bufferSize;

        public ConnectorWebSocket(uint bufferSize)
        {
            _handler = null;
            _webSock = new ClientWebSocket();
            _bufferSize = bufferSize;
        }

        bool IConnector.isConnected
        {
            get
            {
                return _webSock.State == WebSocketState.Open;
            }
        }

        async void _receiver()
        {
            List<byte> bs = new List<byte>();
            var buffer = new byte[_bufferSize];

            while (true)
            {
                if (_webSock.State != WebSocketState.Open)
                {
                    _handler.onClose();
                    break;
                }

                try
                {
                    WebSocketReceiveResult result = await _webSock.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    bs.AddRange(buffer.Take(result.Count));

                    if (result.EndOfMessage)
                    {
                        var bytes = bs.ToArray();
                        bs.Clear();

                        //var now = DateTime.Now.Millisecond;
                        //byte[] afterCompress;
                        //if(bytes.Length > 1024)
                        //{
                        //    //afterCompress = ZlibStream.CompressBuffer(bytes);
                        //    afterCompress = DeflateStream.CompressBuffer(bytes);

                        //    Console.WriteLine($"before compress {bytes.Length} affter compress {afterCompress.Length} compress time {DateTime.Now.Millisecond - now}");
                        //}

                        //Console.WriteLine("on _receive: [" + string.Join(", ", bytes) +"]");

                        _handler.onReceive(bytes);
                    }
                }
                catch (Exception ex)
                {
                    if (_handler != null)
                    {
                        _handler.onError($"ConnectorWebSocket._receiver error: {ex.Message}", ex.HResult);
                    }

                    Debug.DebugOutput(DebugTraceType.DTT_Error, $"ConnectorWebSocket._receiver error: {ex.Message}");
                    Debug.DebugOutput(DebugTraceType.DTT_Error, ex.StackTrace);
                }
            }
        }

        async Task IConnector.ConnectAsync(string uri, uint port)
        {
            try
            {
                await _webSock.ConnectAsync(new Uri(uri), CancellationToken.None);

                _handler.onConnected();
            }
            catch (Exception ex)
            {
                if (_handler != null)
                {
                    _handler.onError($"ConnectorWebSocket.ConnectAsync error: {ex.Message}", ex.HResult);
                }

                Debug.DebugOutput(DebugTraceType.DTT_Error, $"ConnectorWebSocket.ConnectAsync error: {ex.Message}");
                Debug.DebugOutput(DebugTraceType.DTT_Error, ex.StackTrace);
            }

#pragma warning disable 4014
            Task.Run((Action)_receiver); // do not await
#pragma warning restore 4014
        }

        bool IConnector.Connect(string uri, uint port)
        {
            Task.Run(async () =>
            {
                try
                {
                    await _webSock.ConnectAsync(new Uri(uri), CancellationToken.None);

                    _handler.onConnected();
                }
                catch (Exception ex)
                {
                    if (_handler != null)
                    {
                        _handler.onError($"ConnectorWebSocket.Connect error: {ex.Message}", ex.HResult);
                    }

                    Debug.DebugOutput(DebugTraceType.DTT_Error, $"ConnectorWebSocket.Connect error: {ex.Message}");
                    Debug.DebugOutput(DebugTraceType.DTT_Error, ex.StackTrace);
                }

                await Task.Run((Action)_receiver);
            });
                
            return true;
        }

        bool IConnector.Close()
        {
            Task.Run(async () =>
            {
                try
                {
                    await _webSock.CloseAsync(WebSocketCloseStatus.NormalClosure, "close", CancellationToken.None);
                }
                catch (Exception ex)
                {
                    if (_handler != null)
                    {
                        _handler.onError($"ConnectorWebSocket.Close error: {ex.Message}", ex.HResult);
                    }

                    Debug.DebugOutput(DebugTraceType.DTT_Error, $"ConnectorWebSocket.Close error: {ex.Message}");
                    Debug.DebugOutput(DebugTraceType.DTT_Error, ex.StackTrace);
                }
            });

            return true;
        }

        async Task IConnector.CloseAsync()
        {
            try
            {
                await _webSock.CloseAsync(WebSocketCloseStatus.NormalClosure, "close", CancellationToken.None);
            }
            catch (Exception ex)
            {
                if (_handler != null)
                {
                    _handler.onError($"ConnectorWebSocket.CloseAsync error: {ex.Message}", ex.HResult);
                }

                Debug.DebugOutput(DebugTraceType.DTT_Error, $"ConnectorWebSocket.CloseAsync error: {ex.Message}");
                Debug.DebugOutput(DebugTraceType.DTT_Error, ex.StackTrace);
            }
        }

        bool IConnector.Send<T>(byte[] buff, T refObj)
        {
            _handler.onSending(refObj);

            Task.Run(async () =>
            {
                try
                {
                    await _webSock.SendAsync(new ArraySegment<byte>(buff), WebSocketMessageType.Binary, true, CancellationToken.None);
                    _handler.onSended(refObj);
                }
                catch (Exception ex)
                {
                    if (_handler != null)
                    {
                        _handler.onError($"ConnectorWebSocket.Send error: {ex.Message}", ex.HResult);
                    }

                    Debug.DebugOutput(DebugTraceType.DTT_Error, $"ConnectorWebSocket.Send error: {ex.Message}");
                    Debug.DebugOutput(DebugTraceType.DTT_Error, ex.StackTrace);
                }
            });

            return true;
        }

        async Task IConnector.SendAsync<T>(byte[] buff, T refObj)
        {
            try
            {
                _handler.onSending(refObj);
                await _webSock.SendAsync(new ArraySegment<byte>(buff), WebSocketMessageType.Binary, true, CancellationToken.None);
                _handler.onSended(refObj);
            }
            catch (Exception ex)
            {
                if (_handler != null)
                {
                    _handler.onError($"ConnectorWebSocket.SendAsync error: {ex.Message}", ex.HResult);
                }

                Debug.DebugOutput(DebugTraceType.DTT_Error, $"ConnectorWebSocket.SendAsync error: {ex.Message}");
                Debug.DebugOutput(DebugTraceType.DTT_Error, ex.StackTrace);
            }
        }

        void IConnector.SetHandler(IConnectorHandler h)
        {
            _handler = h;
        }
    }
}
