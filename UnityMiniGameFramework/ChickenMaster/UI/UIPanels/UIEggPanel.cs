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
        protected Button _recoverBtn;
        protected VisualElement stars;

        private float _hp;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _recoveryTime = this._uiObjects["RecoverTime"].unityVisualElement as Label;

            _startBtn = this._uiObjects["StartBtn"].unityVisualElement as Button;
            _recoverBtn = this._uiObjects["RecoverBtn"].unityVisualElement as Button;
            stars = this._uiObjects["stars"].unityVisualElement;

            _startBtn.style.display = DisplayStyle.None;
            _recoverBtn.style.display = DisplayStyle.None;
            //_startBtn.clicked += onStartLevelClick;
            //_recoverBtn.clicked += onRecoverClick;
            _recoveryTime.text = "READY";
        }

        //public void onStartLevelClick()
        //{
        //    var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
        //    var bi = (cmGame.baseInfo.getData() as LocalBaseInfo);

        //    if (bi.egg.hp <= 0)
        //    {
        //        // can't start level
        //        return;
        //    }

        //    var lvlConf = cmGame.GetCurrentDefenseLevelConf();
        //    if (lvlConf == null)
        //    {
        //        Debug.DebugOutput(DebugTraceType.DTT_Error, $"GetCurrentDefenseLevelConf [{bi.currentLevel}] not exist");
        //        return;
        //    }

        //    if (UnityGameApp.Inst.MainScene.map.currentLevel == null)
        //    {
        //        var level = UnityGameApp.Inst.MainScene.map.CreateLevel(lvlConf.mapLevelName);
        //        if (level != null)
        //        {
        //            (level as CMShootingLevel).SetDefenseLevelConf(lvlConf, bi.currentLevel);

        //            level.Start();
        //            _recoveryTime.text = "";
        //        }
        //    }
        //    else if (!UnityGameApp.Inst.MainScene.map.currentLevel.isStarted)
        //    {
        //        var level = UnityGameApp.Inst.MainScene.map.CreateLevel(lvlConf.mapLevelName);
        //        if (level != null)
        //        {
        //            (level as CMShootingLevel).SetDefenseLevelConf(lvlConf, bi.currentLevel);

        //            level.Start();
        //            _recoveryTime.text = "";
        //        }
        //    }
        //    else
        //    {
        //        // level is ongoing
        //    }
        //    //changeEggState(true);
        //}

        //private void onVideoCb()
        //{
        //    var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
        //    cmGame.Egg.recoverEgg();
        //}

        //private void onRecoverClick()
        //{
        //    SDKManager.showAutoAd(onVideoCb, "recover_egg");
        //}

        public void changeEggState(bool isFighting)
        {
            _recoveryTime.style.display = (!isFighting) ? DisplayStyle.Flex : DisplayStyle.None;
            //bool isDie = _hp <= 0;
            //_startBtn.style.display = (!isFighting && !isDie) ? DisplayStyle.Flex : DisplayStyle.None;
            //_recoverBtn.style.display = (!isFighting && isDie) ? DisplayStyle.Flex : DisplayStyle.None;

            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.uiMainPanel.changeBattleStartByEggState(isFighting, _hp);
            if (_hp > 0)
            {
                _recoveryTime.text = "READY";
            }
        }

        public void onEggDie()
        {
            // TO DO : disable start button, show quick recover button (ad. button)
            //changeEggState(false);
        }
        public void onEggRecover()
        {
            _recoveryTime.text = "READY";
        }

        public void clearRecoverTime()
        {
            _recoveryTime.text = "";
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
                star.style.display = (hp > a) ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
    }
}
