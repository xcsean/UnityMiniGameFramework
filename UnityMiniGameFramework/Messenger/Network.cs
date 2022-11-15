using System;
using System.Collections.Generic;
using System.Text;

namespace UnityMiniGameFramework
{
    public class Network
    {
        public static IConnector CreateConnector(string connType, uint bufferSize)
        {
            return ConnectorImpls.CreateConnector(connType, bufferSize);
        }

        public static IListener CreateListener(string connType)
        {
            return ListenerImpls.CreateListener(connType);
        }

        public static IProtocol CreateProtocol(string protoType)
        {
            return ProtocolImpls.CreateProtocol(protoType);
        }

        public static SessionClient CreateSessionClient(IConnector conn, IProtocol proto, ISessionClientHandler handler)
        {
            SessionClient s = new SessionClient(conn, proto, handler);
            
            return s;
        }
        public static SessionServer CreateSessionServer(IListener lis, IProtocol proto)
        {
            SessionServer s = new SessionServer(lis, proto);

            return s;
        }
    }
}
