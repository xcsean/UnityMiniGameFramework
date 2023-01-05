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
        protected VisualElement _dmgPb;
        protected Label _timeLab;

        private long buffTime = 0;
        private CMSingleBuffConf buffCfg;
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
            _dmgPb = this._uiObjects["TimeProgressBar"].unityVisualElement;
            _timeLab = this._uiObjects["TimeLabel"].unityVisualElement as Label;
        }

        private void onClickClose(MouseUpEvent e)
        {
            hideUI();
        }

        private void onClickVideo(MouseUpEvent e)
        {
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

            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var bi = cmGame.baseInfo.getData() as LocalBaseInfo;
            bi.buffs.doubleAtk = buffTime;
            cmGame.baseInfo.markDirty();
        }

        public void setBuffTime()
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var bi = cmGame.baseInfo.getData() as LocalBaseInfo;
            buffTime = bi.buffs.doubleAtk;
            buffCfg = cmGame.gameConf.gameConfs.buffsConf.doubleAtk;
        }

        private int onUpdate()
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
                _dmgPb.style.width = new StyleLength(new Length(prog * 326));
            }
            else
            {
                _timeLab.text = "REMAINING TIME: 00:00:00";
                _dmgPb.style.width = new StyleLength(new Length(0f * 326));
            }

            return 1;
        }

        public override void showUI()
        {
            base.showUI();
            setBuffTime();
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.addUpdateFunc(onUpdate);
        }

        public override void hideUI()
        {
            base.hideUI();
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.removeUpdateFunc(onUpdate);
        }
    }
}
