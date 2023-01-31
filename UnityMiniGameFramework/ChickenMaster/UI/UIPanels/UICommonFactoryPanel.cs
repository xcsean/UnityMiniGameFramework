using System;
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
        protected Label labProductTitle;
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
            BindMoveActionVE(this._uiObjects["Content"].unityVisualElement);

            sprChicken = this._uiObjects["sprChicken"].unityVisualElement;
            labProductDesc = this._uiObjects["labProductDesc"].unityVisualElement as Label;
            labProductTitle = this._uiObjects["labProductTitle"].unityVisualElement as Label;
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
            if (_factory == null || !_factory.IsActive)
            {
                ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                // 依赖关卡激活
                if (_factoryConf.userLevelRequire > 0 && (cmGame.baseInfo.getData() as LocalBaseInfo).currentLevel < _factoryConf.userLevelRequire)
                {
                    // for Debug ...
                    cmGame.ShowTips(CMGNotifyType.CMG_ERROR, "Game Level not reach !");
                    return;
                }

                if (cmGame.Self.TrySubGold(_factoryConf.activateGoldCost))
                {
                    _factory = cmGame.AddFactory(_factoryConf.mapBuildName);

                    RefreshInfo();
                }
                else
                {
                    cmGame.ShowTips(CMGNotifyType.CMG_ERROR, "insuffcient gold !");
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
            SDKManager.showAutoAd(onVideoCb, $"{_factoryConf.mapBuildName}_productivity_x2");
        }

        private void onVideoCb()
        {
            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            var bi = cmGame.baseInfo.getData() as LocalBaseInfo;
            long buffTime;
            CMSingleBuffConf buffCfg;
            switch (_factoryConf.mapBuildName)
            {
                case "factoryBuilding1":
                    buffTime = bi.buffs.factory1Productivity;
                    buffCfg = cmGame.gameConf.gameConfs.buffsConf.factory1Productivity;
                    buffTime = addBuff(buffTime, buffCfg);
                    bi.buffs.factory1Productivity = buffTime;
                    break;
                case "factoryBuilding2":
                    buffTime = bi.buffs.factory2Productivity;
                    buffCfg = cmGame.gameConf.gameConfs.buffsConf.factory2Productivity;
                    buffTime = addBuff(buffTime, buffCfg);
                    bi.buffs.factory2Productivity = buffTime;
                    break;
                case "factoryBuilding3":
                    buffTime = bi.buffs.factory3Productivity;
                    buffCfg = cmGame.gameConf.gameConfs.buffsConf.factory3Productivity;
                    buffTime = addBuff(buffTime, buffCfg);
                    bi.buffs.factory3Productivity = buffTime;
                    break;
                case "factoryBuilding4":
                    buffTime = bi.buffs.factory4Productivity;
                    buffCfg = cmGame.gameConf.gameConfs.buffsConf.factory4Productivity;
                    buffTime = addBuff(buffTime, buffCfg);
                    bi.buffs.factory4Productivity = buffTime;
                    break;
                case "factoryBuilding5":
                    buffTime = bi.buffs.factory5Productivity;
                    buffCfg = cmGame.gameConf.gameConfs.buffsConf.factory5Productivity;
                    buffTime = addBuff(buffTime, buffCfg);
                    bi.buffs.factory5Productivity = buffTime;
                    break;
                case "factoryBuilding6":
                    buffTime = bi.buffs.factory6Productivity;
                    buffCfg = cmGame.gameConf.gameConfs.buffsConf.factory6Productivity;
                    buffTime = addBuff(buffTime, buffCfg);
                    bi.buffs.factory6Productivity = buffTime;
                    break;

                default:

                    return;
            }
            cmGame.baseInfo.markDirty();
        }

        private long addBuff(long buffTime, CMSingleBuffConf buffCfg)
        {
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

            return buffTime;
        }


        protected void RefreshInfo()
        {
            bool isMaxLV = false;
            int curLv = (_factory == null || _factory.localFacInfo == null) ? 0 : _factory.localFacInfo.level;
            var _curLevelConf = (_factory == null || _factory.localFacInfo == null) ? null : _factoryConf.levelConfs[curLv];
            var _nextLevelConf = _curLevelConf;

            if (_factoryConf.levelConfs.ContainsKey(curLv + 1))
            {
                _nextLevelConf = _factoryConf.levelConfs[curLv + 1];
            }
            else
            {
                isMaxLV = true;
            }

            if (_curLevelConf == null)
            {
                labCostCoin.text = StringUtil.StringNumFormat($"{_factoryConf.activateGoldCost}");
                labLvCur.text = $"Lv.{0}";
                labLvNext.text = $"Lv.{1}";
                labCostCur.text = $"{0}";
                labGetCur.text = $"{0}";
                labCostNext.text = StringUtil.StringNumFormat($"{_nextLevelConf.maxInputProductStore}");
                labGetNext.text = StringUtil.StringNumFormat($"{_nextLevelConf.maxOutputProductStore}");
                labEfficiencyCur.text = $"{0}";
                labEfficiencyNext.text = StringUtil.StringNumFormat($"{_nextLevelConf.produceOutputCount}");
                nBtnUpgrade.text = $"ACTIVATE";
            }
            else
            {
                labCostCoin.text = StringUtil.StringNumFormat($"{_curLevelConf.upgradeGoldCost}");
                labLvCur.text = $"Lv.{curLv}";
                labLvNext.text = isMaxLV ? $"Lv.Max" : $"Lv.{ curLv + 1}";
                labCostCur.text = StringUtil.StringNumFormat($"{_curLevelConf.maxInputProductStore}");
                labCostNext.text = StringUtil.StringNumFormat($"{_nextLevelConf.maxInputProductStore}");
                labGetCur.text = StringUtil.StringNumFormat($"{_curLevelConf.maxOutputProductStore}");
                labGetNext.text = StringUtil.StringNumFormat($"{_nextLevelConf.maxOutputProductStore}");
                labEfficiencyCur.text = StringUtil.StringNumFormat($"{_curLevelConf.produceOutputCount}");
                labEfficiencyNext.text = StringUtil.StringNumFormat($"{_nextLevelConf.produceOutputCount}");
                nBtnUpgrade.text = $"UPGRADE";
            }
            labProductTitle.text = $"{_factoryConf.factoryName}";
            labProductDesc.text = $"{_factoryConf.factoryText}";
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
