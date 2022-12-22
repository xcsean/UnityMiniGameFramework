using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class UIDoubleAttackPanel : UIPanel
    {
        override public string type => "UIDoubleAttackPanel";

        protected Button _closeBtn;
        public static UIDoubleAttackPanel create()
        {
            return new UIDoubleAttackPanel();
        }

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _closeBtn = this._uiObjects["CloseBtn"].unityVisualElement as Button;
            _closeBtn.RegisterCallback<MouseUpEvent>(onClickClose);
        }

        private void onClickClose(MouseUpEvent e)
        {
            hideUI();
        }
    }
}
