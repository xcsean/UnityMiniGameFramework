using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class UIGetSkillPanel : UIPanel
    {
        override public string type => "UIGetSkillPanel";

        protected Button _closeBtn;
        public static UIGetSkillPanel create()
        {
            return new UIGetSkillPanel();
        }

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _closeBtn = _uiObjects["CloseButton"].unityVisualElement as Button;
            _closeBtn.RegisterCallback<MouseUpEvent>(onClickClose);
        }

        private void onClickClose(MouseUpEvent e)
        {
            hideUI();
        }
    }
}
