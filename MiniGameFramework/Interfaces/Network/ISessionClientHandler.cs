using System;
using System.Collections.Generic;
using System.Text;

namespace MiniGameFramework
{
    public interface ISessionClientHandler
    {
        void onConnecting();
        void onConnected();

        void onClose();

        void onSending(IMessage msg);
        void onSended(IMessage msg);

        void onReceive(IMessage msg);
        
        void onError(string errMsg, int errCode);
    }
}
