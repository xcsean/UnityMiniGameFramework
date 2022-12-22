﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class UIDoubleExpPanel : UIPanel
    {
        override public string type => "UIDoubleExpPanel";

        protected Button _closeBtn;
        protected Button _videoBtn;
        protected ProgressBar _expPb;
        protected Label _timeLab;
        public static UIDoubleExpPanel create()
        {
            return new UIDoubleExpPanel();
        }

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _closeBtn = this._uiObjects["CloseButton"].unityVisualElement as Button;
            _closeBtn.RegisterCallback<MouseUpEvent>(onClickClose);
            _videoBtn = this._uiObjects["VideoButton"].unityVisualElement as Button;
            _videoBtn.RegisterCallback<MouseUpEvent>(onClickVideo);
            _expPb = this._uiObjects["TimeProgressBar"].unityVisualElement as ProgressBar;
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

        public void setBuffTime(long time)
        {
            TimeSpan t = new TimeSpan(time);
            _timeLab.text = $"Remaining Time: {t.Minutes}M{t.Seconds}S";
            _expPb.value = (float)time / 60 * 60 * 1000;
        }
    }
}
