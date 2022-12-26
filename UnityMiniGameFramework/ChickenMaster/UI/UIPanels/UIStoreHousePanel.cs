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

        public Label curLv;
        public Label curCapacity;
        public Label curStorage;
        public Label curProductivity;
        public Label nextLv;
        public Label nextCapacity;
        public Label nextStorage;
        public Label nextProductivity;


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

            curLv = this._uiObjects["curLv"].unityVisualElement as Label;
            curStorage = this._uiObjects["curStorage"].unityVisualElement as Label;
            curCapacity = this._uiObjects["curCapacity"].unityVisualElement as Label;
            curProductivity = this._uiObjects["curProductivity"].unityVisualElement as Label;
            nextLv = this._uiObjects["nextLv"].unityVisualElement as Label;
            nextStorage = this._uiObjects["nextStorage"].unityVisualElement as Label;
            nextCapacity = this._uiObjects["nextCapacity"].unityVisualElement as Label;
            nextProductivity = this._uiObjects["nextProductivity"].unityVisualElement as Label;
            _UpgradePrice = this._uiObjects["upgradePrice"].unityVisualElement as Label;

            _UpgradeBtn = this._uiObjects["UpgradeBtn"].unityVisualElement as Button;
            _VideoBtn = this._uiObjects["VideoBtn"].unityVisualElement as Button;
            _UpgradeBtn.RegisterCallback<MouseUpEvent>(onUpgradeClick);
            _VideoBtn.RegisterCallback<MouseUpEvent>(onVideoClick);

            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            _storeHouse = cmGame.StoreHouse;

            refreshInfo();

            _storeHouse.setUIPanel(this);
        }

        public void refreshInfo()
        {
            curLv.text = $"{_storeHouse.storeHouseInfo.level}";
            //string info =
            //    $"workers: {_storeHouse.storeHouseInfo.storeHouseWorkers.Count}";


            //foreach (var worker in _storeHouse.workers)
            //{
            //    info += $"\r\n -{worker.workerConf.mapNpcName} max : {worker.maxCarryCount}";
            //}

            //info += $"\r\n{_storeHouse.storeHouseConf.storeProductName}: {_storeHouse.storeHouseInfo.storeCount}/{_storeHouse.currentLevelConf.MaxstoreCount}\r\n" +
            //    $"fetch pack: {_storeHouse.currentLevelConf.fetchPackCount}\r\n" +
            //    $"upgrade gold: {_storeHouse.currentLevelConf.upgradeGoldCost}";

            curStorage.text = $"{_storeHouse.currentLevelConf.MaxstoreCount}";
            //_Output.text = $"{_storeHouse.currentLevelConf.outputCeiling}";
            curCapacity.text = $"{_storeHouse.storeHouseConf.workerConf.levelCarryCount[_storeHouse.storeHouseInfo.level]}";
            curProductivity.text = $"{_storeHouse.currentLevelConf.efficiency}";
            _UpgradePrice.text = $"{_storeHouse.currentLevelConf.upgradeGoldCost}";

            isMaxLevel = _storeHouse.storeHouseInfo.level >= _storeHouse.storeHouseConf.levelConfs.Count;
            _UpgradeBtn.text = isMaxLevel ? "CONFIRM" : "UPGRADE";

            if (!isMaxLevel)
            {
                nextLv.text = $"{_storeHouse.storeHouseInfo.level + 1}";
                nextStorage.text = $"{_storeHouse.storeHouseConf.levelConfs[_storeHouse.storeHouseInfo.level + 1].MaxstoreCount}";
                nextCapacity.text = $"{_storeHouse.storeHouseConf.workerConf.levelCarryCount[_storeHouse.storeHouseInfo.level + 1]}";
                nextProductivity.text = $"{_storeHouse.storeHouseConf.levelConfs[_storeHouse.storeHouseInfo.level + 1].efficiency}";
            }
            else
            {
                nextLv.text = "Max";
                nextStorage.text = $"{_storeHouse.currentLevelConf.MaxstoreCount}";
                nextCapacity.text = $"{_storeHouse.storeHouseConf.workerConf.levelCarryCount[_storeHouse.storeHouseInfo.level]}";
                nextProductivity.text = $"{_storeHouse.currentLevelConf.efficiency}";
            }
        }

        public void onUpgradeClick(MouseUpEvent e)
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
        public void onVideoClick(MouseUpEvent e)
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
