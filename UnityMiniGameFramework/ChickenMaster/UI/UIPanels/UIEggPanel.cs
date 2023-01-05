﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using MiniGameFramework;
namespace UnityMiniGameFramework
{
    public class UIEggPanel : UIPanel
    {
        override public string type => "UIEggPanel";
        public static UIEggPanel create()
        {
            return new UIEggPanel();
        }

        protected Label _recoveryTime;
        public Label RecoveryTime => _recoveryTime;


        protected Button _startBtn;
        protected Button RecoverBtn;
        protected VisualElement stars;

        private float _hp;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            //this.unityUIDocument.sortingOrder = -1;

            _recoveryTime = this._uiObjects["RecoverTime"].unityVisualElement as Label;

            _startBtn = this._uiObjects["StartBtn"].unityVisualElement as Button;
            _startBtn.RegisterCallback<MouseUpEvent>(onStartLevelClick);
            RecoverBtn = this._uiObjects["RecoverBtn"].unityVisualElement as Button;
            RecoverBtn.RegisterCallback<MouseUpEvent>(onRecoverClick);
            stars = this._uiObjects["stars"].unityVisualElement;

            _recoveryTime.text = "Ready";
        }

        public void onStartLevelClick(MouseUpEvent e)
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var bi = (cmGame.baseInfo.getData() as LocalBaseInfo);

            if (bi.egg.hp <= 0)
            {
                // can't start level
                return;
            }

            var lvlConf = cmGame.GetCurrentDefenseLevelConf();
            if (lvlConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"GetCurrentDefenseLevelConf [{bi.currentLevel}] not exist");
                return;
            }

            if (UnityGameApp.Inst.MainScene.map.currentLevel == null)
            {
                var level = UnityGameApp.Inst.MainScene.map.CreateLevel(lvlConf.mapLevelName);
                if (level != null)
                {
                    (level as CMShootingLevel).SetDefenseLevelConf(lvlConf, bi.currentLevel);

                    level.Start();
                    _recoveryTime.text = "";
                }
            }
            else if (!UnityGameApp.Inst.MainScene.map.currentLevel.isStarted)
            {
                var level = UnityGameApp.Inst.MainScene.map.CreateLevel(lvlConf.mapLevelName);
                if (level != null)
                {
                    (level as CMShootingLevel).SetDefenseLevelConf(lvlConf, bi.currentLevel);

                    level.Start();
                    _recoveryTime.text = "";
                }
            }
            else
            {
                // level is ongoing
            }
            //changeEggState(true);
        }

        private void onRecoverClick(MouseUpEvent e)
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.Egg.recoverEgg();
        }

        public void changeEggState(bool isFighting)
        {
            bool isDie = _hp <= 0;
            _recoveryTime.visible = !isFighting;
            _startBtn.visible = (!isFighting && !isDie);
            RecoverBtn.visible = (!isFighting && isDie);

            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.uiMainPanel.ShowBattleStartInfo(isFighting && !isDie);
        }

        public void onEggDie()
        {
            // TO DO : disable start button, show quick recover button (ad. button)
            //changeEggState(false);
        }
        public void onEggRecover()
        {
            //changeEggState(false);
            _recoveryTime.text = "Ready";
            // TO DO : show start button
        }

        public void refreshRecoveryTime(long time)
        {
            var t = new TimeSpan((long)(time * 10000));

            _recoveryTime.text = $"{t.Minutes}:{t.Seconds}";
        }

        public void setHp(float hp)
        {
            _hp = hp;
            for (var i = 0; i < stars.childCount; i++)
            {
                var star = stars.ElementAt(i);
                float a = (float) i / stars.childCount;
                star.visible = hp > a;
            }
        }
    }
}
