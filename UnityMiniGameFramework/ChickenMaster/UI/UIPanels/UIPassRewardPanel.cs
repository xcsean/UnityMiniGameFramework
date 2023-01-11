using System;
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
        protected Button CloseButton;
        private VisualElement layoutGrid;
        private CMDefenseLevelAward rewards;

        private readonly Vec2 gridSize = new Vec2(210, 270);
        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            BindShowActionVE(this._uiObjects["Content"].unityVisualElement);

            NormalGetButton = this._uiObjects["NormalGetButton"].unityVisualElement as Button;
            VideoGetButton = this._uiObjects["VideoGetButton"].unityVisualElement as Button;
            CloseButton = this._uiObjects["CloseButton"].unityVisualElement as Button;
            layoutGrid = this._uiObjects["layoutGrid"].unityVisualElement;

            NormalGetButton.clicked += onClickNormalGet;
            VideoGetButton.clicked += onClickVideoGet;
            CloseButton.clicked += onClickClose;
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

            for(int i = 0; i < 6; i++)
            {
                var grid = layoutGrid.Q<VisualElement>($"grid{i}");
                grid.style.display = DisplayStyle.None;
            }

            foreach (var lvlConf in _gameConf.gameConfs.defenseLevels)
            {
                if (level >= lvlConf.levelRangeMin && level <= lvlConf.levelRangeMax)
                {
                    var rewardConf = _gameConf.gameConfs.defenseFCLevelAwards[level];
                    rewards = rewardConf;
                    int index = 0;
                    if(rewardConf.gold > 0)
                    {
                        var grid = layoutGrid.Q<VisualElement>($"grid{index}");
                        grid.Q<Label>("count").text = $"x{rewardConf.gold}";
                        grid.Q<Label>("have").text = $"OWNED: {StringUtil.StringNumFormat(bi.gold.ToString())}";
                        grid.Q<Label>("name").text = "GOLD";
                        grid.Q<VisualElement>("weapon").style.display = DisplayStyle.None;
                        grid.Q<Label>("have").style.display = DisplayStyle.Flex;
                        var tx = ((UnityResourceManager)UnityGameApp.Inst.Resource).LoadTexture($"icons/common/icon_jinbi");
                        if (tx != null)
                        {
                            grid.Q<VisualElement>("RewardIcon").style.backgroundImage = tx;
                            grid.Q<VisualElement>("RewardIcon").style.width = tx.width;
                            grid.Q<VisualElement>("RewardIcon").style.height = tx.height;
                            grid.Q<VisualElement>("RewardIcon").style.left = (170 - tx.width) / 2;
                            grid.Q<VisualElement>("RewardIcon").style.top = (120 - tx.height) / 2;
                        }
                        grid.style.display = DisplayStyle.Flex;
                        index++;
                    }

                    //if (rewardConf.exp > 0)
                    //{
                    //    var grid = layoutGrid.Q<VisualElement>($"grid{index}");
                    //    grid.Q<Label>("count").text = $"X{rewardConf.exp}";
                    //    grid.Q<Label>("have").text = $"OWNED: {bi.exp}";
                    //    grid.Q<Label>("name").text = "EXP";
                    //    grid.Q<VisualElement>("weapon").style.display = DisplayStyle.Flex;
                    //    grid.Q<Label>("have").style.display = DisplayStyle.Flex;
                    //    grid.style.display = DisplayStyle.Flex;
                    //    index++;
                    //}

                    var gunConf = _gameConf.gameConfs.gunConfs;
                    for (var i = 0; i < rewardConf.items.Count; i++)
                    {
                        if(index < 6)
                        {
                            var grid = layoutGrid.Q<VisualElement>($"grid{index}");
                            grid.Q<Label>("count").text = $"X{rewardConf.items[i].count}";
                            grid.Q<Label>("name").text = $"{rewardConf.items[i].itemName}";
                            grid.Q<VisualElement>("weapon").style.display = DisplayStyle.None;
                            grid.Q<Label>("have").style.display = DisplayStyle.None;
                            grid.Q<VisualElement>("RewardIcon").style.backgroundImage = null;
                            foreach (var gun in gunConf)
                            {
                                if (gun.Value.upgradeItemName == rewardConf.items[i].itemName)
                                {
                                    var tx = ((UnityResourceManager)UnityGameApp.Inst.Resource).LoadTexture($"icons/weapons/{gun.Value.weaponIcon}");
                                    if (tx != null)
                                    {
                                        grid.Q<VisualElement>("RewardIcon").style.backgroundImage = tx;
                                        grid.Q<VisualElement>("RewardIcon").style.width = tx.width;
                                        grid.Q<VisualElement>("RewardIcon").style.height = tx.height;
                                        grid.Q<VisualElement>("RewardIcon").style.left = (170 - tx.width) / 2;
                                        grid.Q<VisualElement>("RewardIcon").style.top = (120 - tx.height) / 2;
                                    }
                                }
                            }
                            grid.style.display = DisplayStyle.Flex;
                            index++;
                        }
                    }
                }
            }
        }

        private void onClickNormalGet()
        {
            collectRewards();
        }
        private void onClickVideoGet()
        {
            collectRewards(true);
            SDKManager.showAutoAd((SdkEvent args) =>
            {
                if (args.type == AdEventType.RewardEvent)
                {
                    //TODO 看完视频下发奖励
                    MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Debug, $"Callback AdEventArgs." + args.type.ToString());
                    onVideoCb();
                }
            });
        }

        private void onVideoCb()
        {
            collectRewards(true);
        }

        private void onClickClose()
        {
            collectRewards();
        }

        private void collectRewards(bool isVideo = false)
        {
            int triple = isVideo ? 3 : 1;
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.Self.AddGold(rewards.gold * triple);
            cmGame.Self.AddExp(rewards.exp * triple);
            foreach (var reward in rewards.items)
            {
                cmGame.Self._RealAddBackpackItem(reward.itemName, reward.count * triple);
            }
            hideUI();
        }

        public override void hideUI()
        {
            for (int i = 0; i < 6; i++)
            {
                var grid = layoutGrid.Q<VisualElement>($"grid{i}");
                grid.style.display = DisplayStyle.None;
                grid.Q<VisualElement>("weapon").style.display = DisplayStyle.None;
                grid.Q<Label>("have").style.display = DisplayStyle.None;
            }
            base.hideUI();
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.reshowAllUI();
        }
    }
}
