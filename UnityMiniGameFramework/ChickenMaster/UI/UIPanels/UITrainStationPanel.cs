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

        protected Label _Info;
        public Label Info => _Info;


        protected Button _UpgradeBtn;
        public Button UpgradeBtn => _UpgradeBtn;

        protected Button _CallBtn;
        public Button CallBtn => _CallBtn;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _Level = this._uiObjects["Level"].unityVisualElement as Label;
            _Info = this._uiObjects["Info"].unityVisualElement as Label;

            _UpgradeBtn = this._uiObjects["UpgradeBtn"].unityVisualElement as Button;
            _UpgradeBtn.RegisterCallback<MouseUpEvent>(onUpgradeClick);

            _CallBtn = this._uiObjects["CallBtn"].unityVisualElement as Button;
            _CallBtn.RegisterCallback<MouseUpEvent>(onCallClick);

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

            TimeSpan t = new TimeSpan(_trainStation.train.timeToTrainArrival * 10000);
            string info =
                $"Workers: {_trainStation.trainStationInfo.trainStationWorkers.Count}";

            foreach (var worker in _trainStation.workers)
            {
                info += $"\r\n -{worker.workerConf.mapNpcName} max : {worker.maxCarryCount}";
            }

            info +=
                $"\r\nUpgrade gold: {_trainStation.currentLevelConf.upgradeGoldCost}\r\n" +
                $"Train Arrive: {t.Minutes}:{t.Seconds}\r\n" +
                $"Train Sell: {_trainStation.currentLevelConf.maxSellCountPerRound}\r\n" +
                $"Stored: {_trainStation.currTotalStoreCount}/{_trainStation.currentLevelConf.MaxstoreCount}";

            foreach(var prod in _trainStation.trainStationInfo.storeProducts)
            {
                info += $"\r\n -{prod.productName} : {prod.count}";
            }

            _Info.text = info;
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
            // upgrade
            if (_trainStation.CallTrainNow())
            {
                refreshInfo();
            }
        }

    }
}
