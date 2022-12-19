using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UIDoubleAttackPanel : UIPanel
    {
        override public string type => "UIDoubleAttackPanel";
        public static UIDoubleAttackPanel create()
        {
            return new UIDoubleAttackPanel();
        }

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);
        }
    }
}
