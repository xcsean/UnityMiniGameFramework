using System;
using System.Collections.Generic;
using System.Text;

namespace MiniGameFramework
{
    class Network : INetwork
    {
        public IConnector CreateConnector(string connType, uint bufferSize)
        {
            return ConnectorImpls.CreateConnector(connType, bufferSize);
        }

        public IListener CreateListener(string connType)
        {
            return ListenerImpls.CreateListener(connType);
        }

        public IProtocol CreateProtocol(string protoType)
        {
            return ProtocolImpls.CreateProtocol(protoType);
        }

        public SessionClient CreateSessionClient(IConnector conn, IProtocol proto, ISessionClientHandler handler)
        {
            SessionClient s = new SessionClient(conn, proto, handler);
            
            return s;
        }
        public SessionServer CreateSessionServer(IListener lis, IProtocol proto)
        {
            SessionServer s = new SessionServer(lis, proto);

            return s;
        }
    }
}
