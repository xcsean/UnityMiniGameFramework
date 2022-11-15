using System;
using System.Collections.Generic;
using System.Text;

namespace UnityMiniGameFramework
{
    class ProtocolImpls
    {
        public static IProtocol CreateProtocol(string protoType)
        {
            switch (protoType)
            {
                case "protoBuff":
                    return new ProtocolProtoBuffer();
                case "json":
                    return new ProtocolJson();
            }
            return null;
        }
    }
}
