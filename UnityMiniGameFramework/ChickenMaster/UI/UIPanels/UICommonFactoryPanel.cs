using MiniGameFramework;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UICommonFactoryPanel : UIPopupPanel
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

        protected Label labTitle;
        protected Label labLvCur;
        protected Label labLvNext;
        protected Label labCostCur;
        protected Label labCostNext;
        protected Label labGetCur;
        protected Label labGetNext;
        protected Label labEfficiencyCur;
        protected Label labEfficiencyNext;
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

            _factory = cmGame.GetFactory(factoryName);

            RefreshInfo();
        }

        private void FindUI()
        {
            labTitle = this._uiObjects["labTitle"].unityVisualElement as Label;
            labLvCur = this._uiObjects["labLvCur"].unityVisualElement as Label;
            labLvNext = this._uiObjects["labLvNext"].unityVisualElement as Label;
            labCostCur = this._uiObjects["labCostResCur"].unityVisualElement as Label;
            labCostNext = this._uiObjects["labCostResNext"].unityVisualElement as Label;
            labGetCur = this._uiObjects["labGetResCur"].unityVisualElement as Label;
            labGetNext = this._uiObjects["labGetResNext"].unityVisualElement as Label;
            labEfficiencyCur = this._uiObjects["labEfficiencyCur"].unityVisualElement as Label;
            labEfficiencyNext = this._uiObjects["labEfficiencyNext"].unityVisualElement as Label;
            labCostCoin = this._uiObjects["labCostCoin"].unityVisualElement as Label;
            nBtnEfficiency = this._uiObjects["nBtnEfficiency"].unityVisualElement as Button;
            nBtnUpgrade = this._uiObjects["nBtnUpgrade"].unityVisualElement as Button;

            nBtnEfficiency.RegisterCallback<MouseUpEvent>(OnEfficiencyBtnClick);
            nBtnUpgrade.RegisterCallback<MouseUpEvent>(OnUpgradeBtnClick);

            labCostCoin.text = "0";
        }

        /// <summary>
        /// 升级
        /// </summary>
        protected void OnUpgradeBtnClick(MouseUpEvent e)
        {
            if (_factory == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, "UICommonFactoryPanel _factory is null");
            }
            else
            {
                int lv = _factory.localFacInfo.level;
                if (_factory.TryUpgrade())
                {
                    RefreshInfo();
                }
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"当前等级：{lv}，升级后等级：{_factory.localFacInfo.level}");
            }
        }

        /// <summary>
        /// 效率翻倍
        /// </summary>
        protected void OnEfficiencyBtnClick(MouseUpEvent e)
        {
            Debug.DebugOutput(DebugTraceType.DTT_Debug, "onEfficiencyBtnClick...");
        }

        protected void RefreshInfo()
        {
            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            if (_factory == null)
            {
                if (cmGame.Self.TrySubGold(_factoryConf.activateGoldCost))
                {
                    _factory = cmGame.AddFactory(_factoryConf.mapBuildName);
                }
                else
                {
                    cmGame.uiMainPanel.NofityMessage(CMGNotifyType.CMG_ERROR, "insuffcient gold !");
                }
            }
            bool isMaxLV = false;
            int curLv = _factory.localFacInfo.level;
            var _curLevelConf = _factoryConf.levelConfs[curLv];
            var _nextLevelConf = _curLevelConf;

            if (_factoryConf.levelConfs.ContainsKey(curLv + 1))
            {
                _nextLevelConf = _factoryConf.levelConfs[curLv + 1];
            }
            else
            {
                isMaxLV = true;
            }
            if (_factory == null)
            {
                labTitle.text = $"";
                labCostCoin.text = $"Coin:{0}";
                labLvCur.text = $"Lv.{0}";
                labLvNext.text = $"Lv.{1}";
                labCostCur.text = $"{0}/次";
                labCostNext.text = $"{0}/次";
                labGetCur.text = $"{0}/次";
                labGetNext.text = $"{0}/次";
                labEfficiencyCur.text = $"{0}/次";
                labEfficiencyNext.text = $"{0}/次";
            }
            else
            {
                labTitle.text = $"没有配置标题";
                labCostCoin.text = $"Coin:{_curLevelConf.upgradeGoldCost}";
                labLvCur.text = $"Lv.{curLv}";
                labLvNext.text = isMaxLV ? $"Lv.Max" : $"Lv.{ curLv + 1}";
                labCostCur.text = $"{_curLevelConf.maxInputProductStore}/次";
                labCostNext.text = $"{_nextLevelConf.maxInputProductStore}/次";
                labGetCur.text = $"{_curLevelConf.maxOutputProductStore}/次";
                labGetNext.text = $"{_nextLevelConf.maxOutputProductStore}/次";
                labEfficiencyCur.text = $"{_curLevelConf.produceOutputCount}/次";
                labEfficiencyNext.text = $"{_nextLevelConf.produceOutputCount}/次";
            }
        }

        public override void showUI()
        {
            base.showUI();

            RefreshInfo();
        }

        public override void hideUI()
        {
            base.hideUI();
        }

    }
}
