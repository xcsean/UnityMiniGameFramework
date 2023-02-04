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
        public Label nextlevel;
        public Label nextcapacity;
        public Label nextstorage;


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
            nextlevel = this._uiObjects["NextLevel"].unityVisualElement as Label;
            nextstorage = this._uiObjects["NextStorage"].unityVisualElement as Label;
            nextcapacity = this._uiObjects["NextCapacity"].unityVisualElement as Label;
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
            int CurLevel = _storeHouse.storeHouseInfo.level;
            storage.text = StringUtil.StringNumFormatWithDot($"{_storeHouse.currentLevelConf.MaxstoreCount}");
            capacity.text = StringUtil.StringNumFormatWithDot($"{_storeHouse.storeHouseConf.workerConf.levelCarryCount[CurLevel]}");
            _UpgradePrice.text = StringUtil.StringNumFormatWithDot($"{_storeHouse.currentLevelConf.upgradeGoldCost}");

            isMaxLevel = !(_storeHouse.storeHouseConf.levelConfs.ContainsKey(CurLevel + 1) && _storeHouse.storeHouseConf.workerConf.levelCarryCount.ContainsKey(CurLevel + 1));
            _UpgradeBtn.text = isMaxLevel ? "OK" : "UPGRADE";
            level.text = isMaxLevel ? $"Lv.{CurLevel} is max" : $"Lv.{CurLevel}";
            nextlevel.text = $"Lv.{CurLevel + 1}";
            nextlevel.style.display = isMaxLevel ? DisplayStyle.None : DisplayStyle.Flex;

            nextstorage.style.display = isMaxLevel ? DisplayStyle.None : DisplayStyle.Flex;
            nextcapacity.style.display = isMaxLevel ? DisplayStyle.None : DisplayStyle.Flex;
            if (!isMaxLevel)
            {
                nextstorage.text = StringUtil.StringNumFormatWithDot($"{_storeHouse.storeHouseConf.levelConfs[CurLevel + 1].MaxstoreCount}");
                nextcapacity.text = StringUtil.StringNumFormatWithDot($"{_storeHouse.storeHouseConf.workerConf.levelCarryCount[CurLevel + 1]}");
            }
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
            SDKManager.showAutoAd(onVideoCb, "storehouse_porter_speed_up");
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

            //foreach (var worker in _storeHouse.workers)
            //{
            //    //worker.maxCarryCount *= 2;
            //}

            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var bi = cmGame.baseInfo.getData() as LocalBaseInfo;
            long buffTime = bi.buffs.storehouseProterSpeed;
            CMSingleBuffConf buffCfg = cmGame.gameConf.gameConfs.buffsConf.storehouseProterSpeed;
            long nowMillisecond = (long)(DateTime.Now.Ticks / 10000);
            if (buffTime < nowMillisecond)
            {
                buffTime = nowMillisecond + buffCfg.videoGet * 1000;
            }
            else
            {
                buffTime += buffCfg.videoGet * 1000;
                if (buffTime - nowMillisecond > buffCfg.maxBuff * 1000)
                {
                    buffTime = nowMillisecond + buffCfg.maxBuff * 1000;
                }
            }

            bi.buffs.storehouseProterSpeed = buffTime;
            cmGame.baseInfo.markDirty();
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
