using System;
using System.Threading;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public interface IConnector
    {
        bool isConnected
        {
            get;
        }

        void SetHandler(IConnectorHandler h);

        bool Connect(string uri, uint port);
        Task ConnectAsync(string uri, uint port);

        bool Close();
        Task CloseAsync();

        bool Send<T>(byte[] buff, T refObj);
        Task SendAsync<T>(byte[] buff, T refObj);
    }
}
