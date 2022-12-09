using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UIMainPanel : UIPanel
    {
        override public string type => "UIMainPanel";
        public static UIMainPanel create()
        {
            return new UIMainPanel();
        }

        protected VisualElement _joystickArea;
        protected UIJoyStickControl _joystick;
        public UIJoyStickControl Joystick => _joystick;

        protected Label _meatNum;
        public Label meatNum => _meatNum;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _joystick = this._uiObjects["JoyStick"] as UIJoyStickControl;
            _meatNum = this._uiObjects["MeatNum"].unityVisualElement as Label;
            
        }
    }
}
