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
        public string type;
        public int id;
        public float cur;
        public float end;
        public int have;
        public int normal;
        public int video;
        public int total;
        public int temp;
    }
    public class UIPassRewardPanel : UIPopupPanel
    {
        override public string type => "UIPassRewardPanel";
        public static UIPassRewardPanel create()
        {
            return new UIPassRewardPanel();
        }

        protected Button NormalGetButton;
        protected Button VideoGetButton;
        private VisualElement layoutGrid;
        private CMDefenseLevelAward rewards;

        private bool bReceived = false;
        private readonly Vec2 gridSize = new Vec2(210, 270);
        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            BindShowActionVE(this._uiObjects["Content"].unityVisualElement);

            NormalGetButton = this._uiObjects["NormalGetButton"].unityVisualElement as Button;
            VideoGetButton = this._uiObjects["VideoGetButton"].unityVisualElement as Button;
            layoutGrid = this._uiObjects["layoutGrid"].unityVisualElement;

            NormalGetButton.clicked += onClickNormalGet;
            VideoGetButton.clicked += onClickVideoGet;
        }

        public override void showUI()
        {
            base.showUI();
            showReward();

            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.uiMainPanel.Joystick.OnMouseUp(null);
        }

        private void showReward()
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var bi = (cmGame.baseInfo.getData() as LocalBaseInfo);
            int level = bi.currentLevel - 1;
            var _gameConf = UnityGameApp.Inst.Conf.getConfig("cmgame") as CMGameConfig;

            for (int i = 0; i < 6; i++)
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

                    if (rewardConf.exp > 0)
                    {
                        var grid = layoutGrid.Q<VisualElement>($"grid{index}");
                        grid.Q<Label>("count").text = $"X{rewardConf.exp}";
                        grid.Q<Label>("count").style.display = DisplayStyle.None;
                        grid.Q<Label>("have").text = $"OWNED: {bi.exp}";
                        grid.Q<Label>("name").text = $"EXP +{rewardConf.exp}";
                        grid.Q<Label>("lv").text = $"{bi.level}";
                        grid.Q<VisualElement>("weapon").style.display = DisplayStyle.Flex;
                        grid.Q<Label>("have").style.display = DisplayStyle.None;
                        grid.style.display = DisplayStyle.Flex;
                        int upgradeNeed = cmGame.gameConf.getLevelUpExpRequire(bi.level);
                        grid.Q<Label>("progress").text = $"0/{upgradeNeed}";
                        float prog = bi.exp > upgradeNeed ? 1f : ((float)bi.exp / upgradeNeed);
                        grid.Q<VisualElement>("bar").style.width = new StyleLength(new Length(prog * 93f));
                        progs.Add(new ProgressAniParams()
                        {
                            type = "exp",
                            id = index,
                            cur = 0f,
                            end = prog,
                            have = bi.exp,
                            normal = bi.exp + rewardConf.exp,
                            video = bi.exp + rewardConf.exp * 3,
                            total = upgradeNeed,
                            temp = bi.level
                        });
                        var tx = ((UnityResourceManager)UnityGameApp.Inst.Resource).LoadTexture($"icons/common/icon_exp");
                        if (tx != null)
                        {
                            grid.Q<VisualElement>("RewardIcon").style.backgroundImage = tx;
                            grid.Q<VisualElement>("RewardIcon").style.width = tx.width;
                            grid.Q<VisualElement>("RewardIcon").style.height = tx.height;
                            grid.Q<VisualElement>("RewardIcon").style.left = (170 - tx.width) / 2;
                            grid.Q<VisualElement>("RewardIcon").style.top = (120 - tx.height) / 2;
                        }
                        index++;
                    }

                    var gunConf = _gameConf.gameConfs.gunConfs;
                    for (var i = 0; i < rewardConf.items.Count; i++)
                    {
                        if(index < 6)
                        {
                            var grid = layoutGrid.Q<VisualElement>($"grid{index}");
                            int rewardCount = rewardConf.items[i].count;
                            grid.Q<Label>("count").text = $"X{rewardCount}";
                            grid.Q<Label>("count").style.display = DisplayStyle.None;
                            grid.Q<Label>("name").text = $"piece x{rewardCount}";//$"{rewardConf.items[i].itemName}";
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
                                    float prog = have > upgradeNeed ? 1f : ((float)have / upgradeNeed);
                                    grid.Q<VisualElement>("bar").style.width = new StyleLength(new Length(prog * 93f));
                                    progs.Add(new ProgressAniParams()
                                    {
                                        type = "gunpiece",
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

                    var x = index == 1 ? 210 : index == 2 ? 105 : 0;
                    for (var i = 0; i < index; i++)
                    {
                        var grid = layoutGrid.Q<VisualElement>($"grid{i}");
                        var y = index < 3 ? 135 : i > 3 ? 270 : 0;
                        grid.style.left = x + 210 * (i % 3);
                        grid.style.top = y;
                    }
                }
            }
            UnityGameApp.Inst.addUpdateCall(startDelay);
            bReceived = false;
        }

        private void startDelay()
        {
            long nowMillisecond = (long)(DateTime.Now.Ticks / 10000);
            if (lastMillisecond == 0)
            {
                lastMillisecond = nowMillisecond;
                return;
            }
            if (nowMillisecond - lastMillisecond > 500)
            {
                lastMillisecond = 0;
                showAni(2);
                UnityGameApp.Inst.addUpdateCall(onUpdate);
                UnityGameApp.Inst.removeUpdateCall(startDelay);
            }
        }

        private void onClickVideoGet()
        {
            if (bReceived)
            {
                return;
            }
            SDKManager.showAutoAd(onVideoCb, "pass_reward");
        }

        private void onClickNormalGet()
        {
            if (bReceived)
            {
                return;
            }
            bReceived = true;
            collectRewards();
            //showAni(2);
        }

        private void onVideoCb()
        {
            bReceived = true;
            showAni(3);
            //collectRewards(true);
        }

        public override void onCloseClick()
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
                prog.cur = type == 1 ? 0f : ((float)cur / prog.total);
                int end = type == 3 ? prog.video : type == 2 ? prog.normal : prog.have;
                prog.end = end > prog.total ? 1f : ((float)end / prog.total);
                VisualElement grid = layoutGrid.Q<VisualElement>($"grid{prog.id}");

                float progress = prog.cur > 1f ? 1f : prog.cur;
                grid.Q<VisualElement>("bar").style.width = new StyleLength(new Length(progress * 93f));
                grid.Q<Label>("progress").text = $"{end}/{prog.total}";
                if (type == 3)
                {
                    if (prog.type == "exp")
                    {
                        grid.Q<Label>("name").text = $"EXP +{prog.video - prog.have}";
                    }
                    else if (prog.type == "gunpiece")
                    {
                        grid.Q<Label>("name").text = $"piece x{prog.video - prog.have}";
                    }
                    else
                    {
                        grid.Q<Label>("count").text = $"X{prog.video - prog.have}";
                    }
                }
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

            bool isEnd = true;
            foreach (var prog in progs)
            {
                prog.cur += 0.01f;
                VisualElement grid = layoutGrid.Q<VisualElement>($"grid{prog.id}");
                float progress = prog.cur > 1f ? (prog.cur / 1f) : prog.cur;
                grid.Q<VisualElement>("bar").style.width = new StyleLength(new Length(progress * 93f));

                if (prog.cur >= prog.end)
                {
                    prog.cur = prog.end;

                    if(prog.end == 1f && prog.type == "exp")
                    {
                        isEnd = false;
                        prog.cur = 0f;
                        prog.video -= prog.total;
                        prog.normal -= prog.total;
                        prog.temp += 1;
                        var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                        prog.total = cmGame.gameConf.getLevelUpExpRequire(prog.temp);
                        var end = aniType == 2 ? prog.normal : aniType == 3 ? prog.video : prog.have;
                        prog.end = end > prog.total ? 1f : (float)end / prog.total;
                        grid.Q<Label>("lv").text = $"{prog.temp}";
                        grid.Q<Label>("progress").text = $"{end}/{prog.total}";
                    }
                }
                else
                {
                    isEnd = false;
                }
            }

            if (isEnd)
            {
                if (aniType == 3)
                {
                    long nowMillisecond = (long)(DateTime.Now.Ticks / 10000);
                    if (lastMillisecond == 0)
                    {
                        lastMillisecond = nowMillisecond;
                        return;
                    }
                    if (nowMillisecond - lastMillisecond > 500)
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

        private void collectRewards(bool isVideo = false)
        {
            int triple = isVideo ? 3 : 1;
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.Self.AddGold(rewards.gold * triple);
            cmGame.Self.AddExp(rewards.exp * triple);

            List<string> list = new List<string>();
            foreach (var reward in rewards.items)
            {
                cmGame.Self._RealAddBackpackItem(reward.itemName, reward.count * triple);
                list.Add(reward.itemName);
            }
            UnityGameApp.Inst.RESTFulClient.ReportList(UnityGameApp.Inst.AnalysisMgr.GetPointData8(), list);
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
        }
    }
}
