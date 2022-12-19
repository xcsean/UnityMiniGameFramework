using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UIGetSkillPanel : UIPanel
    {
        override public string type => "UIGetSkillPanel";
        public static UIGetSkillPanel create()
        {
            return new UIGetSkillPanel();
        }

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);
        }
    }
}
