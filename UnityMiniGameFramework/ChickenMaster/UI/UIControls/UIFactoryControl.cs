using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UIFactoryControl : UIObject
    {
        override public string type => "UIFactoryControl";
        new public static UIFactoryControl create()
        {
            return new UIFactoryControl();
        }

        protected VisualElement _BG;
        protected VisualElement BG => _BG;

        protected Label _inputNumber;
        public Label inputNumber => _inputNumber;

        protected Label _outputNumber;
        public Label outputNumber => _outputNumber;

        protected Label _Level;
        public Label Level => _Level;

        protected Label _CD;
        public Label CD => _CD;

        protected ProgressBar _ProduceProgeress;
        public ProgressBar ProduceProgeress => _ProduceProgeress;

        protected Label _Info;
        public Label Info => _Info;

        protected Button _ActBtn;
        public Button ActBtn => _ActBtn;


        override public void onInit(UIControlConf c, VisualElement o)
        {
            base.onInit(c, o);

            _BG = this._subControls["BG"].unityVisualElement;
            _inputNumber = this._subControls["inputNumber"].unityVisualElement as Label;
            _outputNumber = this._subControls["outputNumber"].unityVisualElement as Label;
            _Level = this._subControls["Level"].unityVisualElement as Label;
            _CD = this._subControls["CD"].unityVisualElement as Label;
            _Info = this._subControls["Info"].unityVisualElement as Label;
            _ActBtn = this._subControls["ActBtn"].unityVisualElement as Button;

            _ProduceProgeress = this._subControls["ProduceProgress"].unityVisualElement as ProgressBar;
        }
    }
}
