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

        protected Label _Level;
        public Label Level => _Level;

        protected Label _Output;
        public Label Output => _Output;

        protected Label _Capacity;
        public Label Capacity => _Capacity;

        protected Label _Efficiency;
        public Label Efficiency => _Efficiency;

        protected Label _Storage;
        public Label Storage => _Storage;

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

            _Level = this._uiObjects["level"].unityVisualElement as Label;
            _Storage = this._uiObjects["storage"].unityVisualElement as Label;
            _Output = this._uiObjects["output"].unityVisualElement as Label;
            _Capacity = this._uiObjects["capacity"].unityVisualElement as Label;
            _Efficiency = this._uiObjects["efficiency"].unityVisualElement as Label;
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
            _Level.text = $"{_storeHouse.storeHouseInfo.level}";
            //string info =
            //    $"workers: {_storeHouse.storeHouseInfo.storeHouseWorkers.Count}";


            //foreach (var worker in _storeHouse.workers)
            //{
            //    info += $"\r\n -{worker.workerConf.mapNpcName} max : {worker.maxCarryCount}";
            //}

            //info += $"\r\n{_storeHouse.storeHouseConf.storeProductName}: {_storeHouse.storeHouseInfo.storeCount}/{_storeHouse.currentLevelConf.MaxstoreCount}\r\n" +
            //    $"fetch pack: {_storeHouse.currentLevelConf.fetchPackCount}\r\n" +
            //    $"upgrade gold: {_storeHouse.currentLevelConf.upgradeGoldCost}";

            _Storage.text = $"{_storeHouse.currentLevelConf.MaxstoreCount}";
            _Output.text = $"{_storeHouse.currentLevelConf.outputCeiling}";
            _Capacity.text = $"{_storeHouse.storeHouseConf.workerConf.levelCarryCount[_storeHouse.storeHouseInfo.level]}";
            _Efficiency.text = $"{_storeHouse.currentLevelConf.efficiency}";
            _UpgradePrice.text = $"upgrade gold:{_storeHouse.currentLevelConf.upgradeGoldCost}";

            isMaxLevel = _storeHouse.storeHouseInfo.level >= _storeHouse.storeHouseConf.levelConfs.Count;
            _UpgradeBtn.text = isMaxLevel ? "Comfirm" : "Upgrade";
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
