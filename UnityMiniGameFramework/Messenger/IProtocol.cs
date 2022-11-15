using System;
using System.Collections.Generic;
using System.Text;

namespace UnityMiniGameFramework
{
    public interface IProtocol
    {
        void regCommand(uint iCmd, Type t);

        byte[] encodeToBytes<T>(uint iCmd, T t);

        IMessage encode<T>(uint iCmd, T t);

        IMessage decode(byte[] buff, uint offset, out uint length);
    }
}
