using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class UIUpgradePanel : UIPopupPanel
    {
        override public string type => "UIUpgradePanel";
        public static UIUpgradePanel create()
        {
            return new UIUpgradePanel();
        }

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

        }

    }
}
