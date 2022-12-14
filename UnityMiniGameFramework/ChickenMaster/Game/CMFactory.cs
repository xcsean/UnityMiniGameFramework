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
        CMFactoryConf factoryConf => _factoryConf;

        protected string _factoryName;
        public string factoryName => _factoryName;

        public LocalFactoryInfo localFacInfo => _localFacInfo;
        protected LocalFactoryInfo _localFacInfo;

        public float produceCD => _produceCD;
        protected float _produceCD;
        public int maxInputProductStore => _maxInputProductStore;
        protected int _maxInputProductStore;
        public int maxOutputProductStore => _maxOutputProductStore;
        protected int _maxOutputProductStore;

        public int currentProductInputStore => _currentProductInputStore;
        protected int _currentProductInputStore;
        public int currentProductOutputStore => _currentProductOutputStore;
        protected int _currentProductOutputStore;

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

            _produceCD = _factoryConf.produceCD + _factoryConf.produceCDAddPerLevel * _localFacInfo.level;
            _maxInputProductStore = _factoryConf.maxInputProductStore + _factoryConf.maxInputProductStoreAddPerLevel * _localFacInfo.level;
            _maxOutputProductStore = _factoryConf.maxOutputProductStore + _factoryConf.maxOutputProductStoreAddPerLevel * _localFacInfo.level;

            _refreshInfo();

            _currentCD = _produceCD;
            _produceVer = 0;

            return true;
        }


        virtual protected void _refreshInfo()
        {
            _currentProductInputStore = 0;
            _currentProductOutputStore = 0;

            foreach (var info in _localFacInfo.buildingInputProducts)
            {
                _currentProductInputStore += info.count;
            }
            foreach (var info in _localFacInfo.buildingOutputProducts)
            {
                _currentProductOutputStore += info.count;
            }
        }
        public int getUpgradeGoldCost()
        {
            int upgradeGold = 0;
            _factoryConf.upgradeGoldCostPerLevel.TryGetValue(_localFacInfo.level, out upgradeGold);
            return upgradeGold;
        }

        public bool TryUpgrade()
        {
            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;

            //if (_localFacInfo.level >= cmGame.Self.userLevel)
            //{
            //    // can't bigger than user level
            //    return false;
            //}

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
            if (info.count <= 0)
            {
                return;
            }

            if (_currentProductInputStore >= _maxInputProductStore)
            {
                return;
            }

            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);

            int toFill = _maxInputProductStore - _currentProductInputStore;
            if (toFill > info.count)
            {
                toFill = info.count;
            }

            info.count -= toFill;
            // TO DO : find product index and fill

            LocalPackProductInfo facInfo = null;
            for (int i = 0; i < _localFacInfo.buildingInputProducts.Count; ++i)
            {
                if (_localFacInfo.buildingInputProducts[i].productName == info.productName)
                {
                    facInfo = _localFacInfo.buildingInputProducts[i];
                }
            }

            if (facInfo == null)
            {
                _localFacInfo.buildingInputProducts.Add(new LocalPackProductInfo()
                {
                    productName = info.productName,
                    count = toFill
                });
            }
            else
            {
                facInfo.count += toFill;
            }

            _currentProductInputStore += toFill;

            cmGame.baseInfo.markDirty();
            cmGame.uiMainPanel.refreshMeat();
        }

        public virtual int fetchProduct(string productName, int value)
        {
            LocalPackProductInfo facInfo = null;
            for (int i = 0; i < _localFacInfo.buildingOutputProducts.Count; ++i)
            {
                if (_localFacInfo.buildingOutputProducts[i].productName == productName)
                {
                    facInfo = _localFacInfo.buildingOutputProducts[i];
                }
            }

            if (facInfo == null)
            {
                return 0;
            }

            int fetchCount = value;
            if(fetchCount > facInfo.count)
            {
                fetchCount = facInfo.count;
            }

            facInfo.count -= fetchCount;
            _currentProductOutputStore -= fetchCount;

            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            cmGame.baseInfo.markDirty();

            return fetchCount;
        }

        protected virtual bool _produceProduct(LocalPackProductInfo inputProd, CMProductMakerConf prodMakerConf)
        {
            if (inputProd.count <= 0)
            {
                return false;
            }

            if (_currentProductOutputStore >= _maxOutputProductStore)
            {
                return false;
            }

            int produceCount = prodMakerConf.produceOutputCount;
            if (inputProd.count < prodMakerConf.costInputCount)
            {
                produceCount = inputProd.count * prodMakerConf.produceOutputCount / prodMakerConf.costInputCount;
                if (produceCount <= 0)
                {
                    produceCount = 1;
                }
            }

            int spaceLeft = _maxOutputProductStore - _currentProductOutputStore;

            if (produceCount > spaceLeft)
            {
                produceCount = spaceLeft;
            }

            int realCostInputCount = produceCount * prodMakerConf.costInputCount / prodMakerConf.produceOutputCount;
            if (realCostInputCount >= inputProd.count)
            {
                inputProd.count = 0;
            }
            else
            {
                inputProd.count -= realCostInputCount;
            }

            LocalPackProductInfo facInfo = null;
            for (int i = 0; i < _localFacInfo.buildingOutputProducts.Count; ++i)
            {
                if (_localFacInfo.buildingOutputProducts[i].productName == prodMakerConf.outputProductName)
                {
                    facInfo = _localFacInfo.buildingOutputProducts[i];
                }
            }

            if (facInfo == null)
            {
                _localFacInfo.buildingOutputProducts.Add(new LocalPackProductInfo()
                {
                    productName = prodMakerConf.outputProductName,
                    count = produceCount
                });
            }
            else
            {
                facInfo.count += produceCount;
            }

            _currentProductInputStore -= realCostInputCount;
            _currentProductOutputStore += produceCount;

            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            cmGame.baseInfo.markDirty();

            return true;
        }

        protected virtual void _doProduce()
        {
            bool changed = false;
            for (int i = 0; i < _localFacInfo.buildingInputProducts.Count; ++i)
            {
                var inputProd = _localFacInfo.buildingInputProducts[i];
                CMProductMakerConf prodMakerConf = null;
                foreach (var conf in _factoryConf.productMaker)
                {
                    if (inputProd.productName == conf.inputProductName)
                    {
                        prodMakerConf = conf;
                        break;
                    }
                }

                if (prodMakerConf == null)
                {
                    // TO DO : error 
                    continue;
                }

                if (_produceProduct(inputProd, prodMakerConf))
                {
                    changed = true;
                }
            }

            if (changed)
            {
                _refreshInfo();
                _produceVer++;
            }
        }

        public void OnUpdate()
        {
            if (_currentProductInputStore <= 0)
            {
                return;
            }

            _currentCD -= UnityEngine.Time.deltaTime;
            if (_currentCD > 0)
            {
                return;
            }

            _currentCD = _produceCD;

            _doProduce();
        }
    }
}
