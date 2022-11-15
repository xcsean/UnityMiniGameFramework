using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public class SessionClient : IConnectorHandler
    {
        protected IConnector _conn;
        protected IProtocol _proto;
        protected ISessionClientHandler _handler;

        public SessionClient(IConnector conn, IProtocol proto, ISessionClientHandler handler)
        {
            _conn = conn;
            _proto = proto;
            _handler = handler;

            _conn.SetHandler(this);
        }

        public IConnector connector
        {
            get
            {
                return _conn;
            }
        }

        public IProtocol protocol
        {
            get
            {
                return _proto;
            }
        }

        public async Task ConnectAsync(string uri, uint port)
        {
            _handler.onConnecting();

            await _conn.ConnectAsync(uri, port);
        }

        public async Task SendAsync(IMessage msg)
        {
            await _conn.SendAsync(msg.rawData, msg);
        }

        public async Task CloseAsync()
        {
            await _conn.CloseAsync();
        }

        public bool Connect(string uri, uint port)
        {
            _handler.onConnecting();

            return _conn.Connect(uri, port);
        }

        public bool Send(IMessage msg)
        {
            return _conn.Send(msg.rawData, msg);
        }

        public bool Close()
        {
            return _conn.Close();
        }

        void IConnectorHandler.onClose()
        {
            // TO DO : Clear & Close
            _handler.onClose();
        }

        void IConnectorHandler.onConnected()
        {
            // TO DO : 
            _handler.onConnected();
        }

        void IConnectorHandler.onError(string errMsg, int errCode)
        {
            _handler.onError(errMsg, errCode);
        }

        void IConnectorHandler.onReceive(byte[] data)
        {
            uint offset = 0;
            while(offset < data.Length)
            {
                uint msgLen = 0;
                IMessage msg = _proto.decode(data, offset, out msgLen);
                offset += msgLen;
                _handler.onReceive(msg);
            }

        }

        void IConnectorHandler.onSended<T>(T refObj)
        {
            _handler.onSended((IMessage)refObj);
        }

        void IConnectorHandler.onSending<T>(T refObj)
        {
            _handler.onSending((IMessage)refObj);
        }
    }
}
