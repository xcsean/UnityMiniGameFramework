using System;
using System.Collections.Generic;
using System.Text;

namespace MiniGameFramework
{
    class ConnectorImpls
    {
        public static IConnector CreateConnector(string connType, uint bufferSize)
        {
            switch (connType)
            {
                case "websock":
                    return new ConnectorWebSocket(bufferSize);
                case "socket":
                    return new ConnectorSocket();
            }
            return null;
        }
    }
}
