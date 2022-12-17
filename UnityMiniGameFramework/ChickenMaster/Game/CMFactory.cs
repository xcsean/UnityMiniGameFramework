using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class CMFactory
    {
        CMFactoryConf _factoryConf;
        public CMFactoryConf factoryConf => _factoryConf;

        CMFactoryLevelConf _factoryLevelConf;
        public CMFactoryLevelConf factoryLevelConf => _factoryLevelConf;

        protected string _factoryName;
        public string factoryName => _factoryName;

        public LocalFactoryInfo localFacInfo => _localFacInfo;
        protected LocalFactoryInfo _localFacInfo;

        public float produceCD => _factoryLevelConf.produceCD;
        public int maxInputProductStore => _factoryLevelConf.maxInputProductStore;
        public int maxOutputProductStore => _factoryLevelConf.maxOutputProductStore;

        public int currentProductInputStore => _localFacInfo.buildingInputProduct == null? 0 : _localFacInfo.buildingInputProduct.count;
        public int currentProductOutputStore => _localFacInfo.buildingOutputProduct == null ? 0 : _localFacInfo.buildingOutputProduct.count;

        public float currentCD => _currentCD;
        protected float _currentCD;

        protected int _produceVer;
        public int produceVer => _produceVer;

        public bool Init(LocalFactoryInfo facInfo)
        {
            _factoryName = facInfo.mapBuildName;
            _localFacInfo = facInfo;

            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            _factoryConf = cmGame.gameConf.getCMFactoryConf(_factoryName);
            if(_factoryConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMFactory [{_factoryName}] init config not exist");
                return false;
            }

            if(!_factoryConf.levelConfs.TryGetValue(_localFacInfo.level, out _factoryLevelConf))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMFactory [{_factoryName}] level [{_localFacInfo.level}] config not exist");
                return false;
            }

            _currentCD = produceCD;
            _produceVer = 0;

            return true;
        }

        public int getUpgradeGoldCost()
        {
            return _factoryLevelConf.upgradeGoldCost;
        }

        public bool TryUpgrade()
        {
            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;

            //if (_localFacInfo.level >= cmGame.Self.userLevel)
            //{
            //    // can't bigger than user level
            //    return false;
            //}

            if(!_factoryConf.levelConfs.ContainsKey(_localFacInfo.level + 1))
            {
                return false;
            }

            int upgradeGold = getUpgradeGoldCost();
            if (upgradeGold <= 0)
            {
                // no more level
                return false;
            }

            if (cmGame.Self.TrySubGold(upgradeGold))
            {
                // upgrade 
                _localFacInfo.level = _localFacInfo.level + 1;
                _factoryLevelConf = _factoryConf.levelConfs[_localFacInfo.level];
                cmGame.baseInfo.markDirty();
            }
            else
            {
                // TO DO : not enough gold
                return false;
            }

            return true;
        }

        public virtual void fillProduct(LocalPackProductInfo info)
        {
            if (info.productName != _factoryConf.inputProductName)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"factory [{_factoryConf.mapBuildName}] fill product [{info.productName}] but expect [{_factoryConf.inputProductName}]");
                return;
            }

            if (info.count <= 0)
            {
                return;
            }

            if (currentProductInputStore >= maxInputProductStore)
            {
                return;
            }

            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);

            int toFill = maxInputProductStore - currentProductInputStore;
            if (toFill > info.count)
            {
                toFill = info.count;
            }

            info.count -= toFill;
            // TO DO : find product index and fill

            if(_localFacInfo.buildingInputProduct == null)
            {
                _localFacInfo.buildingInputProduct = new LocalPackProductInfo()
                {
                    productName = info.productName,
                    count = toFill
                };
            }
            else
            {
                _localFacInfo.buildingInputProduct.count += toFill;
            }

            cmGame.baseInfo.markDirty();
            cmGame.uiMainPanel.refreshMeat();
        }

        public virtual int fetchProduct(string productName, int value)
        {
            if (_localFacInfo.buildingOutputProduct == null)
            {
                return 0;
            }

            int fetchCount = value;
            if(fetchCount > _localFacInfo.buildingOutputProduct.count)
            {
                fetchCount = _localFacInfo.buildingOutputProduct.count;
            }

            _localFacInfo.buildingOutputProduct.count -= fetchCount;

            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            cmGame.baseInfo.markDirty();

            return fetchCount;
        }

        protected virtual bool _produceProduct()
        {
            if (currentProductInputStore <= 0)
            {
                return false;
            }

            if (currentProductOutputStore >= maxOutputProductStore)
            {
                return false;
            }

            int produceCount = _factoryLevelConf.produceOutputCount;
            if (_localFacInfo.buildingInputProduct.count < _factoryLevelConf.costInputCount)
            {
                produceCount = _localFacInfo.buildingInputProduct.count * _factoryLevelConf.produceOutputCount / _factoryLevelConf.costInputCount;
                if (produceCount <= 0)
                {
                    produceCount = 1;
                }
            }

            int spaceLeft = maxOutputProductStore - currentProductOutputStore;

            if (produceCount > spaceLeft)
            {
                produceCount = spaceLeft;
            }

            int realCostInputCount = produceCount * _factoryLevelConf.costInputCount / _factoryLevelConf.produceOutputCount;
            if (realCostInputCount >= _localFacInfo.buildingInputProduct.count)
            {
                _localFacInfo.buildingInputProduct.count = 0;
            }
            else
            {
                _localFacInfo.buildingInputProduct.count -= realCostInputCount;
            }


            if (_localFacInfo.buildingOutputProduct == null)
            {
                _localFacInfo.buildingOutputProduct = new LocalPackProductInfo()
                {
                    productName = _factoryConf.outputProductName,
                    count = produceCount
                };
            }
            else
            {
                _localFacInfo.buildingOutputProduct.count += produceCount;
            }

            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            cmGame.baseInfo.markDirty();

            return true;
        }

        protected virtual void _doProduce()
        {
            if (_produceProduct())
            {
                _produceVer++;
            }
        }

        public void OnUpdate()
        {
            if (currentProductInputStore <= 0)
            {
                return;
            }

            _currentCD -= UnityEngine.Time.deltaTime;
            if (_currentCD > 0)
            {
                return;
            }

            _currentCD = produceCD;

            _doProduce();
        }
    }
}
