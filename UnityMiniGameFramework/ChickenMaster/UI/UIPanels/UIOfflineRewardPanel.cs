﻿using System;
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
        public Label RewardLabel;

        private LocalAwardInfo _offlineReward;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            CloseButton = this._uiObjects["CloseButton"].unityVisualElement as Button;
            VideoGetButton = this._uiObjects["VideoGetButton"].unityVisualElement as Button;
            OfflineTimeLabel = this._uiObjects["OfflineTimeLabel"].unityVisualElement as Label;
            RewardLabel = this._uiObjects["RewardLabel"].unityVisualElement as Label;

            CloseButton.RegisterCallback<MouseUpEvent>(onCloseClick);
            VideoGetButton.RegisterCallback<MouseUpEvent>(onVideoClick);
        }

        public override void showUI()
        {
            base.showUI();
        }

        public void showReward(LocalAwardInfo offlineReward, long offLineMillisecond)
        {
            _offlineReward = offlineReward;
            RewardLabel.text = $"reward gold: {_offlineReward.gold}";
            int second = (int)(offLineMillisecond / 1000);
            var hours = second / (60 * 60);
            var mins = (second - hours * 60 * 60) / 60;
            var secs = second - hours * 60 * 60 - mins * 60;
            OfflineTimeLabel.text = $"Offline duration: {hours}:{mins}:{secs}";

            this.showUI();
        }

        private void onCloseClick(MouseUpEvent e)
        {
            receiveRewards(1);
            this.hideUI();
        }

        private void onVideoClick(MouseUpEvent e)
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

            _baseInfo.markDirty();
        }
    }
}