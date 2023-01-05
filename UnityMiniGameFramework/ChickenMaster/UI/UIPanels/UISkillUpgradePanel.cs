using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class UISkillUpgradePanel: UIPanel
    {
        override public string type => "UISkillUpgradePanel";

        protected Button _closeBtn;
        protected VisualElement content;
        public static UISkillUpgradePanel create()
        {
            return new UISkillUpgradePanel();
        }
        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _closeBtn = this._uiObjects["CloseButton"].unityVisualElement as Button;
            _closeBtn.RegisterCallback<MouseUpEvent>(onClickClose);
            content = this._uiObjects["unity-content-container"].unityVisualElement;
        }

        public override void showUI()
        {
            base.showUI();

            //for (int index = 1; index < content.childCount + 1; index++) {
            //    var grid = content.Q<VisualElement>($"grid{index}");
            //    //grid.Q<Label>("title").text = $"X{index}";
            //    grid.visible = true;
            //}
        }

        private void onClickClose(MouseUpEvent e)
        {
            hideUI();
        }

    }
}
