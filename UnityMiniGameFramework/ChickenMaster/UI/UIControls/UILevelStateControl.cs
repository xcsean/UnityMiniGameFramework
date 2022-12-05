using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UILevelStateControl : UIObject
    {
        override public string type => "UILevelStateControl";
        new public static UILevelStateControl create()
        {
            return new UILevelStateControl();
        }

        protected Label _timeLeftText;
        public Label timeLeftText => _timeLeftText;

        override public void onInit(UIControlConf c, VisualElement o)
        {
            base.onInit(c, o);

            _timeLeftText = this._subControls["TimeLeftText"].unityVisualElement as Label;

        }


    }
}
