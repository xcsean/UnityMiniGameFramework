using MiniGameFramework;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    /// <summary>
    /// 通用生产工厂界面
    /// </summary>
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

        public string factoryName => _factoryName;
        protected string _factoryName = "factoryBuilding1";

        protected Label labProductDesc;
        protected Label labLvCur;
        protected Label labLvNext;
        protected Label labCostCur;
        protected Label labCostNext;
        protected Label labGetCur;
        protected Label labGetNext;
        protected Label labEfficiencyCur;
        protected Label labEfficiencyNext;
        protected Label labCostCoin;
        protected VisualElement sprChicken;

        protected Button nBtnEfficiency;
        protected Button nBtnUpgrade;
        protected Button nBtnClose;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            FindUI();
            InitFactoryInfo(factoryName);
            RefreshInfo();
        }

        private void FindUI()
        {
            sprChicken = this._uiObjects["sprChicken"].unityVisualElement;
            labProductDesc = this._uiObjects["labProductDesc"].unityVisualElement as Label;
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

            nBtnEfficiency.clicked += OnEfficiencyBtnClick;
            nBtnUpgrade.clicked += OnUpgradeBtnClick;
        }

        public void InitFactoryInfo(string name)
        {
            _factoryName = name;

            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            _factoryConf = cmGame.gameConf.getCMFactoryConf(factoryName);
            if (_factoryConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"UICommonFactoryPanel [{factoryName}] init config not exist");
                return;
            }
            _factory = cmGame.GetFactory(factoryName);

            // 产品图
            var tx = ((UnityResourceManager)UnityGameApp.Inst.Resource).LoadProductIcon($"icon_{_factoryConf.outputProductName}");
            if (tx != null)
            {
                sprChicken.style.backgroundImage = tx;
                sprChicken.style.width = tx.width;
                sprChicken.style.height = tx.height;
            }
        }

        /// <summary>
        /// 升级
        /// </summary>
        protected void OnUpgradeBtnClick()
        {
            if (_factory == null)
            {
                ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                if (cmGame.Self.TrySubGold(_factoryConf.activateGoldCost))
                {
                    _factory = cmGame.AddFactory(_factoryConf.mapBuildName);

                    RefreshInfo();
                }
                else
                {
                    cmGame.uiMainPanel.NofityMessage(CMGNotifyType.CMG_ERROR, "insuffcient gold !");
                }
            }
            else
            {
                int lv = _factory.localFacInfo.level;
                if (_factory.TryUpgrade())
                {
                    RefreshInfo();
                }
                Debug.DebugOutput(DebugTraceType.DTT_Debug, $"当前等级：{lv}，升级后等级：{_factory.localFacInfo.level}");
            }
        }

        /// <summary>
        /// 效率翻倍
        /// </summary>
        protected void OnEfficiencyBtnClick()
        {
            Debug.DebugOutput(DebugTraceType.DTT_Debug, "onEfficiencyBtnClick...");
        }

        protected void RefreshInfo()
        {
            if (_factory == null)
            {
                //labProductDesc.text = $"";
                labCostCoin.text = $"{_factoryConf.activateGoldCost}";
                labLvCur.text = $"Lv.{0}";
                labLvNext.text = $"Lv.{1}";
                labCostCur.text = $"{0}";
                labCostNext.text = $"{0}";
                labGetCur.text = $"{0}";
                labGetNext.text = $"{0}";
                labEfficiencyCur.text = $"{0}";
                labEfficiencyNext.text = $"{0}";
                nBtnUpgrade.text = $"ACTIVATE";
            }
            else
            {
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

                //labProductDesc.text = $"";
                labCostCoin.text = $"{_curLevelConf.upgradeGoldCost}";
                labLvCur.text = $"Lv.{curLv}";
                labLvNext.text = isMaxLV ? $"Lv.Max" : $"Lv.{ curLv + 1}";
                labCostCur.text = StringUtil.StringNumFormat($"{_curLevelConf.maxInputProductStore}");
                labCostNext.text = StringUtil.StringNumFormat($"{_nextLevelConf.maxInputProductStore}");
                labGetCur.text = StringUtil.StringNumFormat($"{_curLevelConf.maxOutputProductStore}");
                labGetNext.text = StringUtil.StringNumFormat($"{_nextLevelConf.maxOutputProductStore}");
                labEfficiencyCur.text = $"{_curLevelConf.produceOutputCount}";
                labEfficiencyNext.text = $"{_nextLevelConf.produceOutputCount}";
                nBtnUpgrade.text = $"UPGRADE";
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
