using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    abstract public class UIFactoryPanel : UIPopupPanel
    {
        CMFactoryConf _factoryConf;

        protected UIFactoryControl _factoryCtrl;
        public UIFactoryControl factoryCtrl => _factoryCtrl;

        protected CMFactory _factory;
        public CMFactory factory => _factory;

        virtual public string factoryName => "";

        protected int _lastUpdateProduceVer;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            _factoryConf = cmGame.gameConf.getCMFactoryConf(factoryName);
            if (_factoryConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"UIFactoryPanel [{factoryName}] init config not exist");
                return;
            }

            _factoryCtrl = this._uiObjects["FactoryUIControl"] as UIFactoryControl;

            _factoryCtrl.ActBtn.RegisterCallback<MouseUpEvent>(onActBtnClick);

            _factory = cmGame.GetFactory(factoryName);

            _refreshInfo();
        }

        public void onActBtnClick(MouseUpEvent e)
        {
            if (_factory == null)
            {
                // not activate
                ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                if (cmGame.Self.TrySubGold(_factoryConf.activateGoldCost))
                {
                    // active defense hero
                    _factory = cmGame.AddFactory(_factoryConf.mapBuildName);
                    //cmGame.baseInfo.markDirty();

                    _refreshInfo();
                }
                else
                {
                    // TO DO : not enough gold
                }
            }
            else
            {
                // upgrade
                if (_factory.TryUpgrade())
                {
                    _refreshInfo();
                }
            }

        }

        virtual protected void _refreshInfo()
        {
            if(_factory == null)
            {
                // not activate
                _factoryCtrl.ActBtn.text = "Activate";

                _factoryCtrl.CD.text = $"CD: 0";
                _factoryCtrl.Level.text = $"Level: 0";

                _factoryCtrl.Info.text = $"Activate Gold: {_factoryConf.activateGoldCost}";
            }
            else
            {
                _lastUpdateProduceVer = _factory.produceVer;

                _factoryCtrl.ActBtn.text = "Upgrade";

                _factoryCtrl.CD.text = $"CD: {_factory.produceCD}";
                _factoryCtrl.Level.text = $"Level: {_factory.localFacInfo.level}";

                _factoryCtrl.Info.text = $"Upgrade Gold: {_factory.getUpgradeGoldCost()}";

                if (_factory.localFacInfo.buildingInputProduct != null)
                {
                    _factoryCtrl.inputNumber.text = $"{_factory.localFacInfo.buildingInputProduct.productName}: {_factory.localFacInfo.buildingInputProduct.count}";
                }
                if (_factory.localFacInfo.buildingOutputProduct != null)
                {
                    // for Debug ...
                    _factoryCtrl.outputNumber.text = $"{_factory.localFacInfo.buildingOutputProduct.productName}: {_factory.localFacInfo.buildingOutputProduct.count}";
                }
            }
        }

        public override void showUI()
        {
            base.showUI();

            if (_factory != null)
            {
                // for Debug ...
                var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
                var meatInfo = cmGame.Self.GetBackpackProductInfo("meat");
                if (meatInfo != null)
                {
                    _factory.fillProduct(meatInfo);

                    _refreshInfo();
                }
            }

            UnityGameApp.Inst.addUpdateCall(this.OnUpdate);
        }

        public override void hideUI()
        {
            base.hideUI();


            UnityGameApp.Inst.removeUpdateCall(this.OnUpdate);
        }

        protected void OnUpdate()
        {
            if(_factory == null)
            {
                return;
            }

            if(_factory.currentProductInputStore <= 0)
            {
                _factoryCtrl.ProduceProgeress.value = 0.0f;
                return;
            }

            _factoryCtrl.ProduceProgeress.value = (1.0f - (_factory.currentCD / _factory.produceCD))*100;

            if(_lastUpdateProduceVer != _factory.produceVer)
            {
                _refreshInfo();
            }
        }
    }

    public class UIFactory1Panel : UIFactoryPanel
    {
        override public string type => "UIFactory1Panel";
        public static UIFactory1Panel create()
        {
            return new UIFactory1Panel();
        }
        override public string factoryName => "factoryBuilding1";
    }
    public class UIFactory2Panel : UIFactoryPanel
    {
        override public string type => "UIFactory2Panel";
        public static UIFactory2Panel create()
        {
            return new UIFactory2Panel();
        }
        override public string factoryName => "factoryBuilding2";
    }
}
