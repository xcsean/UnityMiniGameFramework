using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UIOfflineRewardPanel : UIPanel
    {
        override public string type => "UIOfflineRewardPanel";
        public static UIOfflineRewardPanel create()
        {
            return new UIOfflineRewardPanel();
        }

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);
        }
    }
}
