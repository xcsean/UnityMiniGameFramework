using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class UIDoubleExpPanel : UIPopupPanel
    {
        override public string type => "UIDoubleExpPanel";

        protected Button _videoBtn;
        protected VisualElement _expPb;
        protected Label _timeLab;

        private long buffTime = 0;
        private CMSingleBuffConf buffCfg;
        public static UIDoubleExpPanel create()
        {
            return new UIDoubleExpPanel();
        }

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            BindShowActionVE(this._uiObjects["Content"].unityVisualElement);

            _videoBtn = this._uiObjects["VideoButton"].unityVisualElement as Button;
            _videoBtn.clicked += onClickVideo;
            _expPb = this._uiObjects["TimeProgressBar"].unityVisualElement;
            _timeLab = this._uiObjects["TimeLabel"].unityVisualElement as Label;
        }
        private void onClickVideo()
        {
            SDKManager.showAutoAd(onVideoCb, "double_exp");
        }
        private void onVideoCb()
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            long nowMillisecond = (long)(DateTime.Now.Ticks / 10000);
            if (buffTime < nowMillisecond)
            {
                buffTime = nowMillisecond + buffCfg.videoGet * 1000;
            }
            else
            {
                buffTime += buffCfg.videoGet * 1000;
                if (buffTime - nowMillisecond > buffCfg.maxBuff * 1000)
                {
                    buffTime = nowMillisecond + buffCfg.maxBuff * 1000;
                }
            }

            var bi = cmGame.baseInfo.getData() as LocalBaseInfo;
            bi.buffs.doubleExp = buffTime;
            cmGame.baseInfo.markDirty();
        }

        public void setBuffTime()
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var bi = cmGame.baseInfo.getData() as LocalBaseInfo;
            buffTime = bi.buffs.doubleExp;
            buffCfg = cmGame.gameConf.gameConfs.buffsConf.doubleExp;
        }

        private void onUpdate()
        {
            long nowMillisecond = (long)(DateTime.Now.Ticks / 10000);
            if (buffTime > nowMillisecond)
            {
                int time = (int)(buffTime - nowMillisecond) / 1000;

                int hours = time / 60 / 60;
                int mins = (time - hours * 60 * 60) / 60;
                int secs = time - hours * 60 * 60 - mins * 60;
                var str = hours >= 10 ? hours.ToString() : "0" + hours.ToString();
                str += mins >= 10 ? ":" + mins.ToString() : ":0" + mins.ToString();
                str += secs >= 10 ? ":" + secs.ToString() : ":0" + secs.ToString();

                _timeLab.text = $"REMAINING TIME: {str}";
                float prog = (float)time / buffCfg.maxBuff;
                _expPb.style.width = new StyleLength(new Length(prog * 326));
            } else
            {
                _timeLab.text = "REMAINING TIME: 00:00:00";
                _expPb.style.width = new StyleLength(new Length(0f * 326));
            }
        }

        public override void showUI()
        {
            base.showUI();
            setBuffTime();
            UnityGameApp.Inst.addUpdateCall(onUpdate);
        }

        public override void hideUI()
        {
            base.hideUI();
            UnityGameApp.Inst.removeUpdateCall(onUpdate);
        }
    }
}
