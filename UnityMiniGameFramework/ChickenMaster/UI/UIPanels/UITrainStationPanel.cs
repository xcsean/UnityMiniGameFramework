using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
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

        protected Label _NextLv;
        public Label NextLv => _NextLv;

        protected Label _NextCapacity;
        public Label NextCapacity => _NextCapacity;

        protected Label _UpgradePrice;
        public Label UpgradePrice => _UpgradePrice;


        protected Button _UpgradeBtn;
        public Button UpgradeBtn => _UpgradeBtn;

        protected Button _CallBtn;
        public Button CallBtn => _CallBtn;

        protected Button _SpeedUpBtn;
        public Button SpeedUpBtn => _SpeedUpBtn;

        protected Button _CloseBtn;
        public Button CloseBtn => _CloseBtn;

        private Boolean isMaxLevel = false;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _Level = this._uiObjects["level"].unityVisualElement as Label;
            _CurLv = this._uiObjects["curLv"].unityVisualElement as Label;
            _CurCapacity = this._uiObjects["curCapacity"].unityVisualElement as Label;
            _NextLv = this._uiObjects["nextLv"].unityVisualElement as Label;
            _NextCapacity = this._uiObjects["nextCapacity"].unityVisualElement as Label;
            _UpgradePrice = this._uiObjects["UpgradePrice"].unityVisualElement as Label;

            _UpgradeBtn = this._uiObjects["UpgradeBtn"].unityVisualElement as Button;
            _UpgradeBtn.RegisterCallback<MouseUpEvent>(onUpgradeClick);

            _CallBtn = this._uiObjects["CallBtn"].unityVisualElement as Button;
            _CallBtn.RegisterCallback<MouseUpEvent>(onCallClick);
            _SpeedUpBtn = this._uiObjects["SpeedUpBtn"].unityVisualElement as Button;
            _SpeedUpBtn.RegisterCallback<MouseUpEvent>(onSpeedUpClick);
            _CloseBtn = this._uiObjects["CloseButton"].unityVisualElement as Button;
            _CloseBtn.RegisterCallback<MouseUpEvent>(onCloseClick);

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

        public void refreshInfo()
        {
            _Level.text = $"Lv: {_trainStation.trainStationInfo.level}";
            _CurLv.text = $"{_trainStation.trainStationInfo.level}";
            _CurCapacity.text = $"{_trainStation.currentLevelConf.MaxstoreCount}";
            isMaxLevel = _trainStation.trainStaionConf.levelConfs.Count <= _trainStation.trainStationInfo.level;
            _NextLv.text = isMaxLevel ? $"{_trainStation.trainStationInfo.level}" : $"{_trainStation.trainStationInfo.level + 1}";
            _NextCapacity.text = isMaxLevel ? $"{_trainStation.currentLevelConf.MaxstoreCount}" : $"{_trainStation.trainStaionConf.levelConfs[_trainStation.trainStationInfo.level + 1].MaxstoreCount}";
            UpgradePrice.text = $"upgrade gold: {_trainStation.currentLevelConf.upgradeGoldCost}";
            UpgradeBtn.text = isMaxLevel ? "Max  Level" : "Upgrade";

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

        public void onUpgradeClick(MouseUpEvent e)
        {
            // upgrade
            if (_trainStation.TryUpgrade())
            {
                refreshInfo();
            }
        }
        public void onCallClick(MouseUpEvent e)
        {
            // call
            if (_trainStation.CallTrainNow())
            {
                refreshInfo();
            }
        }
        public void onSpeedUpClick(MouseUpEvent e)
        {

        }

    }
}
