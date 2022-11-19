using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class UIPanelStartMain : UIPanel
    {
        override public string type => "UIPanelStartMain";
        public static UIPanelStartMain create()
        {
            return new UIPanelStartMain();
        }


    }
}
