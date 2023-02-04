using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{

    public class Format
    {
        public double value { get; set; }
        public string symbol { get; set; }
    }
    public class UITrainStationPanel : UIPopupPanel
    {
        override public string type => "UITrainStationPanel";
        public static UITrainStationPanel create()
        {
            return new UITrainStationPanel();
        }

        CMTrainStation _trainStation;

        protected Label _Level;
        public Label Level => _Level;

        protected Label _CurLv;
        public Label CurLv => _CurLv;

        protected Label _CurCapacity;
        public Label CurCapacity => _CurCapacity;

        protected Label _CurStorage;
        public Label CurStorage => _CurStorage;

        protected Label _CurStationStorage;
        public Label CurStationStorage => _CurStationStorage;

        protected Label _NextLv;
        public Label NextLv => _NextLv;

        protected Label _NextCapacity;
        public Label NextCapacity => _NextCapacity;

        protected Label _NextStorage;
        public Label NextStorage => _NextStorage;

        protected Label _NextStationStorage;
        public Label NextStationStorage => _NextStationStorage;

        protected Label _UpgradePrice;
        public Label UpgradePrice => _UpgradePrice;


        protected Button _UpgradeBtn;
        public Button UpgradeBtn => _UpgradeBtn;

        protected Button _CallBtn;
        public Button CallBtn => _CallBtn;

        protected Button _SpeedUpBtn;
        public Button SpeedUpBtn => _SpeedUpBtn;

        private Boolean isMaxLevel = false;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            BindMoveActionVE(this._uiObjects["Content"].unityVisualElement);

            _Level = this._uiObjects["level"].unityVisualElement as Label;
            _CurLv = this._uiObjects["curLv"].unityVisualElement as Label;
            _CurCapacity = this._uiObjects["curCapacity"].unityVisualElement as Label;
            _CurStorage = this._uiObjects["curStorage"].unityVisualElement as Label;
            _CurStationStorage = this._uiObjects["curStationStorage"].unityVisualElement as Label;
            _NextLv = this._uiObjects["nextLv"].unityVisualElement as Label;
            _NextCapacity = this._uiObjects["nextCapacity"].unityVisualElement as Label;
            _NextStorage = this._uiObjects["nextStorage"].unityVisualElement as Label;
            _NextStationStorage = this._uiObjects["nextStationStorage"].unityVisualElement as Label;
            _UpgradePrice = this._uiObjects["UpgradePrice"].unityVisualElement as Label;

            _UpgradeBtn = this._uiObjects["UpgradeBtn"].unityVisualElement as Button;
            _UpgradeBtn.clicked += onUpgradeClick;

            _CallBtn = this._uiObjects["CallBtn"].unityVisualElement as Button;
            _CallBtn.clicked += onCallClick;
            _SpeedUpBtn = this._uiObjects["SpeedUpBtn"].unityVisualElement as Button;
            _SpeedUpBtn.clicked += onSpeedUpClick;

            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            _trainStation = cmGame.TrainStation;

            refreshInfo();

            _trainStation.setUIPanel(this);
        }

        public override void showUI()
        {
            base.showUI();

            refreshInfo();
        }

        private string numChange(int num)
        {
            return StringUtil.StringNumFormat(num.ToString());
            try
            {
                List<Format> numDatas = new List<Format>()
                {
                    new Format() {value = 1, symbol = ""},
                    //new Format() {value = 1e2, symbol = "H"},
                    new Format() {value = 1e3, symbol = "K"},
                    new Format() {value = 1e6, symbol = "M"},
                    new Format() {value = 1e9, symbol = "G"},
                    new Format() {value = 1e12, symbol = "T"},
                    new Format() {value = 1e15, symbol = "P"},
                    new Format() {value = 1e18, symbol = "E"}
                };

                int i = 0;
                for (i = numDatas.Count - 1; i > 0; i--)
                {
                    if (num >= numDatas[i].value)
                    {
                        break;
                    }
                }
                return Math.Round(num / numDatas[i].value) + numDatas[i].symbol;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            //return num.ToString();
        }

        public void refreshInfo()
        {
            isMaxLevel = _trainStation.trainStaionConf.levelConfs.Count <= _trainStation.trainStationInfo.level;
            var levelCarryConf = _trainStation.trainStaionConf.workerConf.levelCarryCount;
            int nextLevel = isMaxLevel ? _trainStation.trainStationInfo.level : _trainStation.trainStationInfo.level + 1;
            _Level.text = $"Lv. {_trainStation.trainStationInfo.level}";
            _CurLv.text = $"{_trainStation.trainStationInfo.level}";
            _NextLv.text = $"{nextLevel}";
            _CurCapacity.text = $"{numChange(levelCarryConf[_trainStation.trainStationInfo.level])}";
            _NextCapacity.text = $"{numChange(levelCarryConf[nextLevel])}";
            _CurStorage.text = $"{numChange(_trainStation.trainStaionConf.levelConfs[_trainStation.trainStationInfo.level].maxSellCountPerRound)}";
            _CurStationStorage.text = $"{numChange(_trainStation.trainStaionConf.levelConfs[_trainStation.trainStationInfo.level].MaxstoreCount)}";
            _NextStorage.text = $"{numChange(_trainStation.trainStaionConf.levelConfs[nextLevel].maxSellCountPerRound)}";
            _NextStationStorage.text = $"{numChange(_trainStation.trainStaionConf.levelConfs[nextLevel].MaxstoreCount)}";
            UpgradePrice.text = $"{numChange(_trainStation.currentLevelConf.upgradeGoldCost)}";
            UpgradeBtn.text = isMaxLevel ? "OK" : "Upgrade";

            //TimeSpan t = new TimeSpan(_trainStation.train.timeToTrainArrival * 10000);
            //string info =
            //    $"Workers: {_trainStation.trainStationInfo.trainStationWorkers.Count}";

            //foreach (var worker in _trainStation.workers)
            //{
            //    info += $"\r\n -{worker.workerConf.mapNpcName} max : {worker.maxCarryCount}";
            //}

            //info +=
            //    $"\r\nUpgrade gold: {_trainStation.currentLevelConf.upgradeGoldCost}\r\n" +
            //    $"Train Arrive: {t.Minutes}:{t.Seconds}\r\n" +
            //    $"Train Sell: {_trainStation.currentLevelConf.maxSellCountPerRound}\r\n" +
            //    $"Stored: {_trainStation.currTotalStoreCount}/{_trainStation.currentLevelConf.MaxstoreCount}";

            //foreach(var prod in _trainStation.trainStationInfo.storeProducts)
            //{
            //    info += $"\r\n -{prod.productName} : {prod.count}";
            //}

        }

        public void onUpgradeClick()
        {
            if (isMaxLevel)
            {
                hideUI();
                return;
            }
            // upgrade
            if (_trainStation.TryUpgrade())
            {
                refreshInfo();
            }
        }
        public void onCallClick()
        {
            SDKManager.showAutoAd(onCallVideoCb, "trainstation_call");
        }
        public void onSpeedUpClick()
        {
            SDKManager.showAutoAd(onSpeedUpVideoCb, "trainstation_porter_speed_up");
        }

        private void onSpeedUpVideoCb()
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var bi = cmGame.baseInfo.getData() as LocalBaseInfo;
            long buffTime = bi.buffs.trainProterSpeed;
            CMSingleBuffConf buffCfg = cmGame.gameConf.gameConfs.buffsConf.trainProterSpeed;
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

            bi.buffs.trainProterSpeed = buffTime;
            cmGame.baseInfo.markDirty();
        }

        private void onCallVideoCb()
        {
            // call
            if (_trainStation.CallTrainNow())
            {
                refreshInfo();
            }
        }

    }
}
