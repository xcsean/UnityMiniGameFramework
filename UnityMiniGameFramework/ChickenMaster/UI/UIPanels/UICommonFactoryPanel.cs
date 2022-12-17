using MiniGameFramework;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UICommonFactoryPanel :  UIPopupPanel
    {
        override public string type => "UICommonFactoryPanel";
        public static UICommonFactoryPanel create()
        {
            return new UICommonFactoryPanel();
        }

        CMFactoryConf _factoryConf;

        protected CMFactory _factory;
        public CMFactory factory => _factory;

        virtual public string factoryName => "factoryBuilding1";

        protected int _lastUpdateProduceVer;

        protected Label labTitle;
        protected Label labLvCur;
        protected Label labLvNext;
        protected Label labCostCur;
        protected Label labCostNext;
        protected Label labGetCur;
        protected Label labGetNext;
        protected Label labCostCoin;

        protected Button nBtnEfficiency;
        protected Button nBtnUpgrade;
        protected Button nBtnClose;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            _factoryConf = cmGame.gameConf.getCMFactoryConf(factoryName);
            if (_factoryConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"UICommonFactoryPanel [{factoryName}] init config not exist");
                return;
            }

            FindUI();
        }

        private void FindUI() {
            labTitle = this._uiObjects["labTitle"].unityVisualElement as Label;
            labLvCur = this._uiObjects["labLvCur"].unityVisualElement as Label;
            labLvNext = this._uiObjects["labLvNext"].unityVisualElement as Label;
            labCostCur = this._uiObjects["labCostResCur"].unityVisualElement as Label;
            labCostNext = this._uiObjects["labCostResNext"].unityVisualElement as Label;
            labGetCur = this._uiObjects["labGetResCur"].unityVisualElement as Label;
            labGetNext = this._uiObjects["labGetResNext"].unityVisualElement as Label;
            labGetNext = this._uiObjects["labEfficiencyCur"].unityVisualElement as Label;
            labGetNext = this._uiObjects["labEfficiencyNext"].unityVisualElement as Label;
            labCostCoin = this._uiObjects["labCostCoin"].unityVisualElement as Label;
            nBtnEfficiency = this._uiObjects["nBtnEfficiency"].unityVisualElement as Button;
            nBtnUpgrade = this._uiObjects["nBtnUpgrade"].unityVisualElement as Button;

            nBtnEfficiency.RegisterCallback<MouseUpEvent>(OnUpgradeBtnClick);
            nBtnUpgrade.RegisterCallback<MouseUpEvent>(OnEfficiencyBtnClick);

            labCostCoin.text = "99099";
        }

        protected void OnUpgradeBtnClick(MouseUpEvent e)
        {
            Debug.DebugOutput(DebugTraceType.DTT_Debug, "onUpgradeBtnClick...");
        }

        protected void OnEfficiencyBtnClick(MouseUpEvent e)
        {
            Debug.DebugOutput(DebugTraceType.DTT_Debug, "onEfficiencyBtnClick...");
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
        }
    }

}
