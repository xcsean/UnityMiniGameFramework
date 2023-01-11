using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UIStoreHousePanel : UIPopupPanel
    {
        override public string type => "UIStoreHousePanel";
        public static UIStoreHousePanel create()
        {
            return new UIStoreHousePanel();
        }

        CMStoreHouse _storeHouse;

        public Label level;
        public Label capacity;
        public Label storage;


        protected Label _UpgradePrice;
        public Label UpgradePrice => _UpgradePrice;


        protected Button _UpgradeBtn;
        public Button UpgradeBtn => _UpgradeBtn;

        protected Button _VideoBtn;
        public Button VideoBtn => _VideoBtn;

        private Boolean isMaxLevel = false;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);
            
            BindMoveActionVE(this._uiObjects["Content"].unityVisualElement);

            level = this._uiObjects["Level"].unityVisualElement as Label;
            storage = this._uiObjects["Storage"].unityVisualElement as Label;
            capacity = this._uiObjects["Capacity"].unityVisualElement as Label;
            _UpgradePrice = this._uiObjects["upgradePrice"].unityVisualElement as Label;

            _UpgradeBtn = this._uiObjects["UpgradeBtn"].unityVisualElement as Button;
            _VideoBtn = this._uiObjects["VideoBtn"].unityVisualElement as Button;
            _UpgradeBtn.clicked += onUpgradeClick;
            _VideoBtn.clicked += onVideoClick;

            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            _storeHouse = cmGame.StoreHouse;

            refreshInfo();

            _storeHouse.setUIPanel(this);
        }

        public void refreshInfo()
        {
            //string info =
            //    $"workers: {_storeHouse.storeHouseInfo.storeHouseWorkers.Count}";


            //foreach (var worker in _storeHouse.workers)
            //{
            //    info += $"\r\n -{worker.workerConf.mapNpcName} max : {worker.maxCarryCount}";
            //}

            //info += $"\r\n{_storeHouse.storeHouseConf.storeProductName}: {_storeHouse.storeHouseInfo.storeCount}/{_storeHouse.currentLevelConf.MaxstoreCount}\r\n" +
            //    $"fetch pack: {_storeHouse.currentLevelConf.fetchPackCount}\r\n" +
            //    $"upgrade gold: {_storeHouse.currentLevelConf.upgradeGoldCost}";

            storage.text = $"{_storeHouse.currentLevelConf.MaxstoreCount}";
            capacity.text = $"{_storeHouse.storeHouseConf.workerConf.levelCarryCount[_storeHouse.storeHouseInfo.level]}";
            _UpgradePrice.text = $"{_storeHouse.currentLevelConf.upgradeGoldCost}";

            isMaxLevel = _storeHouse.storeHouseInfo.level >= _storeHouse.storeHouseConf.levelConfs.Count;
            _UpgradeBtn.text = isMaxLevel ? "OK" : "UPGRADE";
            level.text = isMaxLevel ? $"Lv.{_storeHouse.storeHouseInfo.level} is max" : $"Lv.{_storeHouse.storeHouseInfo.level}";
        }

        public void onUpgradeClick()
        {
            if (isMaxLevel)
            {
                this.hideUI();
                return;
            }
            // upgrade
            if (_storeHouse.TryUpgrade())
            {
                refreshInfo();
            }
        }
        public void onVideoClick()
        {
            SDKManager.showAutoAd((AdEventArgs args) =>
            {
                if (args.type == VideoEvent.RewardEvent)
                {
                    //TODO 看完视频下发奖励
                    MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Debug, $"Callback AdEventArgs." + args.type.ToString());
                    onVideoCb();
                }
            });
        }

        private void onVideoCb()
        {
            //long nowMillisecond = (long)(DateTime.Now.Ticks / 10000);
            //foreach (var worker in _storeHouse.workers)
            //{
            //    if(worker.workerInfo.buffRecoveryTime > nowMillisecond)
            //    {
            //        worker.workerInfo.buffRecoveryTime += 60 * 1000;
            //    }
            //    else
            //    {
            //        worker.workerInfo.buffRecoveryTime = nowMillisecond + 60 * 1000;
            //    }
            //}

            foreach (var worker in _storeHouse.workers)
            {
                //worker.maxCarryCount *= 2;
            }
        }

        public override void showUI()
        {
            base.showUI();

            if (_storeHouse != null)
            {
                // TO DO : play fill product animation

                ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                var prodInfo = cmGame.Self.GetBackpackProductInfo(_storeHouse.storeHouseConf.storeProductName);
                if (prodInfo != null)
                {
                    _storeHouse.TryFillStoreProduct(prodInfo);

                    refreshInfo();
                }
            }
        }
    }
}
