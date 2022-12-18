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

        protected Label _Info;
        public Label Info => _Info;


        protected Button _UpgradeBtn;
        public Button UpgradeBtn => _UpgradeBtn;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _Level = this._uiObjects["Level"].unityVisualElement as Label;
            _Info = this._uiObjects["Info"].unityVisualElement as Label;

            _UpgradeBtn = this._uiObjects["UpgradeBtn"].unityVisualElement as Button;
            _UpgradeBtn.RegisterCallback<MouseUpEvent>(onUpgradeClick);

            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            _storeHouse = cmGame.StoreHouse;

            refreshInfo();

            _storeHouse.setUIPanel(this);
        }

        public void refreshInfo()
        {
            _Level.text = $"Lv: {_storeHouse.storeHouseInfo.level}";
            string info =
                $"workers: {_storeHouse.storeHouseInfo.storeHouseWorkers.Count}";


            foreach (var worker in _storeHouse.workers)
            {
                info += $"\r\n -{worker.workerConf.mapNpcName} max : {worker.maxCarryCount}";
            }

            info += $"\r\n{_storeHouse.storeHouseConf.storeProductName}: {_storeHouse.storeHouseInfo.storeCount}/{_storeHouse.currentLevelConf.MaxstoreCount}\r\n" +
                $"fetch pack: {_storeHouse.currentLevelConf.fetchPackCount}\r\n" +
                $"upgrade gold: {_storeHouse.currentLevelConf.upgradeGoldCost}";

            _Info.text = info;
        }

        public void onUpgradeClick(MouseUpEvent e)
        {
            // upgrade
            if (_storeHouse.TryUpgrade())
            {
                refreshInfo();
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
