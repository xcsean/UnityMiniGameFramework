using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UIDoubleExpPanel : UIPanel
    {
        override public string type => "UIDoubleExpPanel";
        public static UIDoubleExpPanel create()
        {
            return new UIDoubleExpPanel();
        }

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);
        }
    }
}
