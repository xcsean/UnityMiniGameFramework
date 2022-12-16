using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class UIStoreHousePanel : UIPopupPanel
    {
        override public string type => "UIStoreHousePanel";
        public static UIStoreHousePanel create()
        {
            return new UIStoreHousePanel();
        }
    }
}
