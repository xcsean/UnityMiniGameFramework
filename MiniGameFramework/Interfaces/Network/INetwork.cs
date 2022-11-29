using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public interface INetwork
    {
        IConnector CreateConnector(string connType, uint bufferSize);

        IListener CreateListener(string connType);

        IProtocol CreateProtocol(string protoType);

        SessionClient CreateSessionClient(IConnector conn, IProtocol proto, ISessionClientHandler handler);
        SessionServer CreateSessionServer(IListener lis, IProtocol proto);

        IHttpClient CreateHttpClient();
    }
}
