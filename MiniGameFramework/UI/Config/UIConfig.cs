using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public class MainUIConfig
    {
        public Dictionary<string, string> UIPanels { get; set; }

        // TO DO : add configs;
    }

    public class UIConfig : JsonConfig
    {
        override public string type => "UIConfig";
        public static UIConfig create()
        {
            return new UIConfig();
        }

        public MainUIConfig uiConf => (MainUIConfig)_conf;

        override protected object _JsonDeserialize(string confStr)
        {
            //return JsonSerializer.Deserialize<MainUIConfig>(confStr);
            return JsonUtil.FromJson<MainUIConfig>(confStr);
        }
    }
}
