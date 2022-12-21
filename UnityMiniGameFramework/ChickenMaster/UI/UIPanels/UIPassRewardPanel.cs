﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class UIPassRewardPanel : UIPanel
    {
        override public string type => "UIPassRewardPanel";
        public static UIPassRewardPanel create()
        {
            return new UIPassRewardPanel();
        }

        protected Button NormalGetButton;
        protected Button VideoGetButton;
        //protected Image RewardIcon;
        protected Label RewardNumLabel;
        //private VisualElement gridObj;

        private int rewardNum = 0;
        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);
            NormalGetButton = this._uiObjects["NormalGetButton"].unityVisualElement as Button;
            VideoGetButton = this._uiObjects["VideoGetButton"].unityVisualElement as Button;
            //RewardIcon = this._uiObjects["RewardIcon"].unityVisualElement as Image;
            //RewardNumLabel = this._uiObjects["RewardNumLabel"].unityVisualElement as Label;
            //gridObj = this._uiObjects["grid"].unityVisualElement as VisualElement;
            NormalGetButton.clicked += this.onClickNormalGet;
            VideoGetButton.clicked += this.onClickVideoGet;
        }

        public override void showUI()
        {
            base.showUI();
            showReward();
        }

        private void showReward()
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var bi = (cmGame.baseInfo.getData() as LocalBaseInfo);
            int level = bi.currentLevel - 1;
            var _gameConf = UnityGameApp.Inst.Conf.getConfig("cmgame") as CMGameConfig;
            foreach (var lvlConf in _gameConf.gameConfs.defenseLevels)
            {
                if (level >= lvlConf.levelRangeMin && level <= lvlConf.levelRangeMax)
                {
                    //rewardNum = lvlConf.passReward;

                    //for (var i = 0; i < lvlConf.passReward.Count; i++)
                    //{
                    //    var item = gridObj;
                    //    if(i != 0)
                    //    {

                    //    }
                    //}
                }
            }
            rewardNum = 100;
            RewardNumLabel.text = "gold:" + rewardNum.ToString();
        }

        private void onClickNormalGet()
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.Self.AddGold(rewardNum);
            this.hideUI();
        }
        private void onClickVideoGet()
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.Self.AddGold(rewardNum * 3);
            this.hideUI();
        }
    }
}
