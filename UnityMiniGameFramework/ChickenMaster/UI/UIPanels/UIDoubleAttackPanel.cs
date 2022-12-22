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
        protected Button _videoBtn;
        protected ProgressBar _dmgPb;
        protected Label _timeLab;
        public static UIDoubleAttackPanel create()
        {
            return new UIDoubleAttackPanel();
        }

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _closeBtn = this._uiObjects["CloseButton"].unityVisualElement as Button;
            _closeBtn.RegisterCallback<MouseUpEvent>(onClickClose);
            _videoBtn = this._uiObjects["VideoButton"].unityVisualElement as Button;
            _videoBtn.RegisterCallback<MouseUpEvent>(onClickVideo);
            _dmgPb = this._uiObjects["TimeProgressBar"].unityVisualElement as ProgressBar;
            _timeLab = this._uiObjects["TimeLabel"].unityVisualElement as Label;
        }

        private void onClickClose(MouseUpEvent e)
        {
            hideUI();
        }

        private void onClickVideo(MouseUpEvent e)
        {
            var baseInfo = UnityGameApp.Inst.Datas.localUserData.getData("baseInfo");
        }

        public void setBuffTime(int time)
        {
            var mins = time / 60;
            var secs = time - mins * 60;

            _timeLab.text = $"Remaining Time: {mins}M{secs}S";
            _dmgPb.value = (float)time / 60 * 60;
        }
    }
}
