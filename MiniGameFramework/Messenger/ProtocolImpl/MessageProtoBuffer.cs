using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    class MessageProtoBuffer : IMessage
    {
        protected uint _iCommand;
        protected string _strCommand;
        protected byte[] _rawData;
        protected object _data;

        public MessageProtoBuffer(uint iCmd, byte[] rawData, object data, string strCmd=null)
        {
            _iCommand = iCmd;
            _strCommand = strCmd;
            _rawData = rawData;
            _data = data;
        }
        
        uint IMessage.iCommand
        {
            get
            {
                return _iCommand;
            }
        }

        string IMessage.strCommand
        {
            get
            {
                return _strCommand;
            }
        }

        object IMessage.data
        {
            get
            {
                return _data;
            }
        }

        byte[] IMessage.rawData
        {
            get
            {
                return _rawData;
            }
        }

        T IMessage.getStructureData<T>()
        {
            return (T)_data;
        }

    }
}
