using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public class UIControlConf
    {
        public string name { get; set; }
        public string type { get; set; }

        public List<UIControlConf> subControls { get; set; }


        // TO DO : add configs;
    }

    public class UIPanelConf
    {
        public string name { get; set; }
        public string type { get; set; }

        public string uiFile { get; set; }

        public int sortOrder { get; set; }

        public List<UIControlConf> controls { get; set; }


        // TO DO : add configs;
    }

    public class UIPanelConfig : JsonConfig
    {
        override public string type => "UIPanelConfig";
        public static UIPanelConfig create()
        {
            return new UIPanelConfig();
        }

        public UIPanelConf uiPanelConf => (UIPanelConf)_conf;

        override protected object _JsonDeserialize(string confStr)
        {
            return JsonSerializer.Deserialize<UIPanelConf>(confStr);
        }
    }
}
