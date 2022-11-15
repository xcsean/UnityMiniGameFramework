using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    class ConnectorSocket : IConnector
    {
        public bool isConnected => throw new NotImplementedException();

        public bool Close()
        {
            throw new NotImplementedException();
        }

        public Task CloseAsync()
        {
            throw new NotImplementedException();
        }

        public bool Connect(string uri, uint port)
        {
            throw new NotImplementedException();
        }

        public Task ConnectAsync(string uri, uint port)
        {
            throw new NotImplementedException();
        }

        public bool Send<T>(byte[] buff, T refObj)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync<T>(byte[] buff, T refObj)
        {
            throw new NotImplementedException();
        }

        public void SetHandler(IConnectorHandler h)
        {
            throw new NotImplementedException();
        }
    }
}
