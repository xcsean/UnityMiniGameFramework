using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    class ProtocolJson : IProtocol
    {
        public IMessage decode(byte[] buff, uint offset, out uint length)
        {
            throw new NotImplementedException();
        }

        public IMessage encode<T>(uint iCmd, T t)
        {
            throw new NotImplementedException();
        }

        public byte[] encodeToBytes<T>(uint iCmd, T t)
        {
            throw new NotImplementedException();
        }

        public void regCommand(uint iCmd, Type t)
        {
            throw new NotImplementedException();
        }
    }
}
