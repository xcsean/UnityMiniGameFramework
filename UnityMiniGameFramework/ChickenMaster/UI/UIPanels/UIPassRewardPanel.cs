using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class ProgressAniParams
    {
        public int id;
        public float cur;
        public float end;
        public int have;
        public int normal;
        public int video;
        public int total;
    }
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
                    //if(rewardConf.gold > 0)
                    //{
                    //    var grid = layoutGrid.Q<VisualElement>($"grid{index}");
                    //    grid.Q<Label>("count").text = $"x{rewardConf.gold}";
                    //    grid.Q<Label>("have").text = $"OWNED: {StringUtil.StringNumFormat(bi.gold.ToString())}";
                    //    grid.Q<Label>("name").text = "GOLD";
                    //    grid.Q<VisualElement>("weapon").style.display = DisplayStyle.None;
                    //    grid.Q<Label>("have").style.display = DisplayStyle.Flex;
                    //    var tx = ((UnityResourceManager)UnityGameApp.Inst.Resource).LoadTexture($"icons/common/icon_jinbi");
                    //    if (tx != null)
                    //    {
                    //        grid.Q<VisualElement>("RewardIcon").style.backgroundImage = tx;
                    //        grid.Q<VisualElement>("RewardIcon").style.width = tx.width;
                    //        grid.Q<VisualElement>("RewardIcon").style.height = tx.height;
                    //        grid.Q<VisualElement>("RewardIcon").style.left = (170 - tx.width) / 2;
                    //        grid.Q<VisualElement>("RewardIcon").style.top = (120 - tx.height) / 2;
                    //    }
                    //    grid.style.display = DisplayStyle.Flex;
                    //    index++;
                    //}

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
                            int rewardCount = rewardConf.items[i].count;
                            grid.Q<Label>("count").text = $"X{rewardCount}";
                            grid.Q<Label>("name").text = $"{rewardConf.items[i].itemName}";
                            grid.Q<VisualElement>("weapon").style.display = DisplayStyle.Flex;
                            grid.Q<Label>("have").style.display = DisplayStyle.None;
                            grid.Q<VisualElement>("RewardIcon").style.backgroundImage = null;
                            foreach (var gun in gunConf)
                            {
                                if (gun.Value.upgradeItemName == rewardConf.items[i].itemName)
                                {
                                    var gunInfo = cmGame.GetWeaponInfo(gun.Value.id);
                                    int lv = gunInfo == null ? 1 : gunInfo.level;
                                    var weaponInfo = cmGame.Self.GetBackpackItemInfo(rewardConf.items[i].itemName);
                                    int have = weaponInfo == null ? 0 : weaponInfo.count;
                                    int upgradeNeed = gun.Value.gunLevelConf[lv].upgrageCostItemCost;
                                    grid.Q<Label>("progress").text = $"0/{upgradeNeed}";
                                    grid.Q<Label>("lv").text = $"{lv}";
                                    float prog = (float)have / gun.Value.gunLevelConf[lv].upgrageCostItemCost;
                                    progs.Add(new ProgressAniParams()
                                    {
                                        id = index,
                                        cur = 0f,
                                        end = prog,
                                        have = have,
                                        normal = have + rewardCount,
                                        video = have + rewardCount * 3,
                                        total = upgradeNeed
                                    });

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

                    for (var i = 0; i < index; i++)
                    {
                        var grid = layoutGrid.Q<VisualElement>($"grid{i}");
                        var y = index < 3 ? 135 : i > 3 ? 270 : 0;
                        var x = index == 1 ? 210 : index == 2 ? 105 : 0;
                        grid.style.left = x;
                        grid.style.top = y;
                    }
                }
            }
            showAni(2);
            UnityGameApp.Inst.addUpdateCall(onUpdate);
        }

        private void onClickVideoGet()
        {
            SDKManager.showAutoAd((SdkEvent args) =>
            {
                switch (args.type)
                {
                    case AdEventType.RewardEvent:
                        //TODO 看完视频下发奖励
                        MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Debug, $"Callback AdEventArgs." + args.type.ToString());
                        onVideoCb();
                        break;

                    default:
                        break;
                }
            });
        }

        private void onClickNormalGet()
        {
            collectRewards();
            //showAni(2);
        }

        private void onVideoCb()
        {
            showAni(3);
            //collectRewards(true);
        }

        private void onClickClose()
        {
            //showAni(2);
            collectRewards();
        }

        private List<ProgressAniParams> progs = new List<ProgressAniParams>() { };
        private void showAni(int type)
        {
            foreach (var prog in progs)
            {
                int cur = type == 3 ? prog.normal : type == 2 ? prog.have : 0;
                prog.cur = type == 1f ? 0f : ((float)cur / prog.total);
                int end = type == 3 ? prog.video : type == 2 ? prog.normal : prog.have;
                prog.end = end > prog.total ? 1f :((float)end / prog.total);
                VisualElement grid = layoutGrid.Q<VisualElement>($"grid{prog.id}");
                grid.Q<VisualElement>("bar").style.width = new StyleLength(new Length(prog.cur * 93));
                grid.Q<Label>("progress").text = $"{end}/{prog.total}";
            }
            aniType = type;
        }

        private int aniType = 0;// 1-start,2-normal,3-video
        private long lastMillisecond = 0;
        private void onUpdate()
        {
            if (aniType == 0)
            {
                return;
            }

            foreach (var prog in progs)
            {
                prog.cur += 0.01f;
                VisualElement grid = layoutGrid.Q<VisualElement>($"grid{prog.id}");
                grid.Q<VisualElement>("bar").style.width = new StyleLength(new Length(prog.cur * 93));

                if (prog.cur >= prog.end)
                {
                    prog.cur = prog.end;

                    if (aniType == 3)
                    {
                        long nowMillisecond = (long)(DateTime.Now.Ticks / 10000);
                        if (lastMillisecond == 0)
                        {
                            lastMillisecond = nowMillisecond;
                            return;
                        }
                        if (nowMillisecond - lastMillisecond > 1000)
                        {
                            collectRewards(true);
                            aniType = 0;
                            lastMillisecond = 0;
                        }
                    }
                    else
                    {
                        aniType = 0;
                        lastMillisecond = 0;
                    }
                }
            }
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
            UnityGameApp.Inst.removeUpdateCall(onUpdate);
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
