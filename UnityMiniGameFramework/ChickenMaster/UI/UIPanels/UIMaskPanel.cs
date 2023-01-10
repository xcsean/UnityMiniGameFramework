using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class UIMaskPanel : UIPanel
    {
        override public string type => "UIMaskPanel";

        public static UIMaskPanel create()
        {
            return new UIMaskPanel();
        }
        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);
        }
    }
}
