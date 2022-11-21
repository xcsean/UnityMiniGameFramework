using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class UIMainPanel : UIPanel
    {
        override public string type => "UIMainPanel";
        public static UIMainPanel create()
        {
            return new UIMainPanel();
        }


    }
}
