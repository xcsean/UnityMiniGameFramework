using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class ServerAddressConf
    {
        public string uri { get; set; }
        public uint port { get; set; }
    }

    public class ConnectorConf
    {
        public ServerAddressConf addr { get; set; }
        public uint buffSize { get; set; }
        public string protocolType { get; set; }
        public string connectorType { get; set; }
    }

    public class RESTFulAPIConf
    {
        public string url { get; set; }
    }

    public class NetConfig
    {
        public ConnectorConf webSockConn { get; set; }

        public RESTFulAPIConf restfulConf { get; set; }
    }

    public class NetWorkConfig : JsonConfig
    {
        override public string type => "NetWorkConfig";
        public static NetWorkConfig create()
        {
            return new NetWorkConfig();
        }

        public NetConfig netConf => (NetConfig)_conf;

        override protected object _JsonDeserialize(string confStr)
        {
            return JsonSerializer.Deserialize<NetConfig>(confStr);
        }
    }
}
