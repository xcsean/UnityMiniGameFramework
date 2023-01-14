using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class UIOfflineRewardPanel : UIPanel
    {
        override public string type => "UIOfflineRewardPanel";
        public static UIOfflineRewardPanel create()
        {
            return new UIOfflineRewardPanel();
        }

        public Button CloseButton;
        public Button VideoGetButton;
        public Label OfflineTimeLabel;
        public VisualElement coinGrid;
        public VisualElement expGrid;
        public VisualElement meatGrid;

        private LocalAwardInfo _offlineReward;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            BindShowActionVE(this._uiObjects["Content"].unityVisualElement);

            CloseButton = this._uiObjects["CloseButton"].unityVisualElement as Button;
            VideoGetButton = this._uiObjects["VideoGetButton"].unityVisualElement as Button;
            OfflineTimeLabel = this._uiObjects["OfflineTimeLabel"].unityVisualElement as Label;
            coinGrid = this._uiObjects["coinGrid"].unityVisualElement;
            expGrid = this._uiObjects["expGrid"].unityVisualElement;
            meatGrid = this._uiObjects["meatGrid"].unityVisualElement;

            CloseButton.clicked += onCloseClick;
            VideoGetButton.clicked += onVideoClick;
        }

        public override void showUI()
        {
            base.showUI();
        }

        public void showReward(LocalAwardInfo offlineReward, long offLineMillisecond)
        {
            _offlineReward = offlineReward;
            int second = (int)(offLineMillisecond / 1000 / 60) * 60; // 分钟向下取整
            var hours = second / (60 * 60);
            var mins = (second - hours * 60 * 60) / 60;
            var secs = second - hours * 60 * 60 - mins * 60;
            var str = hours >= 10 ? hours.ToString() : "0" + hours.ToString();
            str += mins >= 10 ? ":" + mins.ToString() : ":0" + mins.ToString();
            str += secs >= 10 ? ":" + secs.ToString() : ":0" + secs.ToString();
            OfflineTimeLabel.text = $"OFF-LINE TIME: {str}";

            coinGrid.style.display = offlineReward.gold > 0 ? DisplayStyle.Flex : DisplayStyle.None;
            coinGrid.Q<Label>("count").text = $"x{_offlineReward.gold}";
            expGrid.style.display = offlineReward.exp > 0 ? DisplayStyle.Flex : DisplayStyle.None;
            expGrid.Q<Label>("count").text = $"x{_offlineReward.exp}";
            meatGrid.style.display = DisplayStyle.None;
            foreach (var pro in offlineReward.products)
            {
                if(pro.Value <= 0)
                {
                    continue;
                }
                if (pro.Key == "meat")
                {
                    meatGrid.style.display = DisplayStyle.Flex;
                    meatGrid.Q<Label>("count").text = $"x{pro.Value}";
                }
            }

            this.showUI();
        }

        private void onCloseClick()
        {
            receiveRewards(1);
            this.hideUI();
        }

        private void onVideoClick()
        {
            SDKManager.showAutoAd(onVideoCb, "offline_reward");
        }

        private void onVideoCb()
        {
            receiveRewards(2);
            this.hideUI();
        }

        private void receiveRewards(int rate = 1)
        {
            var _baseInfo = UnityGameApp.Inst.Datas.localUserData.getData("baseInfo");
            var bi = _baseInfo.getData() as LocalBaseInfo;
            bi.unfetchedOfflineAward.gold += _offlineReward.gold * rate;
            bi.unfetchedOfflineAward.exp += _offlineReward.exp * rate;

            bi.gold += _offlineReward.gold * rate;
            bi.exp += _offlineReward.exp * rate;

            foreach (var itemAwd in _offlineReward.items)
            {
                if (bi.unfetchedOfflineAward.items.ContainsKey(itemAwd.Key))
                {
                    bi.unfetchedOfflineAward.items[itemAwd.Key] += itemAwd.Value * rate;
                }
                else
                {
                    bi.unfetchedOfflineAward.items[itemAwd.Key] = itemAwd.Value * rate;
                }
            }
            foreach (var prodAwd in _offlineReward.products)
            {
                if (bi.unfetchedOfflineAward.products.ContainsKey(prodAwd.Key))
                {
                    bi.unfetchedOfflineAward.products[prodAwd.Key] += prodAwd.Value * rate;
                }
                else
                {
                    bi.unfetchedOfflineAward.products[prodAwd.Key] = prodAwd.Value * rate;
                }
            }

            var _cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            _cmGame.uiMainPanel.refreshAll();

            _baseInfo.markDirty();
            _offlineReward = new LocalAwardInfo();
        }
    }
}
