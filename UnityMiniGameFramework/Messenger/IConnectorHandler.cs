using System;
using System.Collections.Generic;
using System.Text;

namespace UnityMiniGameFramework
{
    public interface IConnectorHandler
    {
        void onConnected();
        void onClose();

        void onSending<T>(T refObj);
        void onSended<T>(T refObj);

        void onReceive(byte[] data);

        void onError(string errMsg, int errCode);
    }
}
