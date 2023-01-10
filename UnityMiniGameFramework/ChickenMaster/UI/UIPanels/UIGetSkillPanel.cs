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
        protected Button _videoBtn;
        protected ProgressBar _progressBar;
        protected Label _timeLab;
        protected Label _countLab;
        protected Label _skillLab;
        protected VisualElement _skillIcon;
        public static UIGetSkillPanel create()
        {
            return new UIGetSkillPanel();
        }

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _closeBtn = _uiObjects["CloseButton"].unityVisualElement as Button;
            _closeBtn.clicked += onClickClose;
            _videoBtn = this._uiObjects["VideoButton"].unityVisualElement as Button;
            _videoBtn.clicked += onClickVideo;
            _progressBar = this._uiObjects["ProgressBar"].unityVisualElement as ProgressBar;
            _timeLab = this._uiObjects["Time"].unityVisualElement as Label;
            _countLab = this._uiObjects["SkillCount"].unityVisualElement as Label;
            _skillLab = this._uiObjects["SkillTitle"].unityVisualElement as Label;
            _skillIcon = this._uiObjects["SkillIcon"].unityVisualElement;

            BindShowActionVE(this._uiObjects["Content"].unityVisualElement);
        }

        private void onClickClose()
        {
            hideUI();
        }

        private void onClickVideo()
        {

        }
    }
}
