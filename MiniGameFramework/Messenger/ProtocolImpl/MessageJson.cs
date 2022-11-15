using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace MiniGameFramework
{
    class MessageJson : IMessage
    {
        protected uint _iCommand;
        protected string _strCommand;
        protected byte[] _rawData;
        protected object _data;

        public MessageJson(uint iCmd, byte[] rawData, object data, string strCmd = null)
        {
            _iCommand = iCmd;
            _strCommand = strCmd;
            _rawData = rawData;
            _data = data;
        }

        public uint iCommand => _iCommand;

        public string strCommand => _strCommand;

        public byte[] rawData => _rawData;

        public object data => _data;

        public T getStructureData<T>()
        {
            return (T)_data;
        }
    }
}
