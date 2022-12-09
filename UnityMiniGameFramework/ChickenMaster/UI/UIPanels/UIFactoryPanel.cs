using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    abstract public class UIFactoryPanel : UIPopupPanel
    {
        protected UIFactoryControl _factoryCtrl;
        public UIFactoryControl factoryCtrl => _factoryCtrl;

        CMFactoryConf _factoryConf;
        CMFactoryConf factoryConf => _factoryConf;

        virtual public string factoryName => "";

        protected LocalFactoryInfo _localFacInfo;
        protected float _produceCD;
        protected int _maxInputProductStore;
        protected int _maxOutputProductStore;

        protected int _currentProductInputStore;
        protected int _currentProductOutputStore;

        protected float _currentCD;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _factoryCtrl = this._uiObjects["FactoryUIControl"] as UIFactoryControl;

            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);

            _localFacInfo = cmGame.GetLocalFactoryInfo(factoryName);

            if(_localFacInfo == null)
            {
                // TO DO : show ruined factory

                // for Debug ...
                _localFacInfo = cmGame.AddLocalFactoryInfo(new LocalFactoryInfo() 
                {
                    mapBuildName = factoryName,
                    level = 1,
                    buildingInputProducts = new List<LocalPackProductInfo>(),
                    buildingOutputProducts = new List<LocalPackProductInfo>()
                });
            }

            _factoryConf = cmGame.gameConf.getCMFactoryConf(factoryName);

            _produceCD = _factoryConf.produceCD + _factoryConf.produceCDAddPerLevel * _localFacInfo.level;
            _maxInputProductStore = _factoryConf.maxInputProductStore + _factoryConf.maxInputProductStoreAddPerLevel * _localFacInfo.level;
            _maxOutputProductStore = _factoryConf.maxOutputProductStore + _factoryConf.maxOutputProductStoreAddPerLevel * _localFacInfo.level;

            _refreshInfo();

            _currentCD = _produceCD;
            UnityGameApp.Inst.addUpdateCall(this.OnUpdate);
        }

        virtual protected void _refreshInfo()
        {
            _currentProductInputStore = 0;
            _currentProductOutputStore = 0;

            _factoryCtrl.CD.text = $"CD: {_produceCD}";
            _factoryCtrl.Level.text = $"Level: {_localFacInfo.level}";

            if (_localFacInfo.buildingInputProducts.Count > 0)
            {
                // for Debug ...
                _factoryCtrl.inputNumber.text = $"{_localFacInfo.buildingInputProducts[0].productName}: {_localFacInfo.buildingInputProducts[0].count}";
            }
            if (_localFacInfo.buildingOutputProducts.Count > 0)
            {
                // for Debug ...
                _factoryCtrl.outputNumber.text = $"{_localFacInfo.buildingOutputProducts[0].productName}: {_localFacInfo.buildingOutputProducts[0].count}";
            }

            foreach(var info in _localFacInfo.buildingInputProducts)
            {
                _currentProductInputStore += info.count;
            }
            foreach (var info in _localFacInfo.buildingOutputProducts)
            {
                _currentProductOutputStore += info.count;
            }
        }

        protected virtual void _fillProduct(LocalPackProductInfo info)
        {
            if(info.count <= 0)
            {
                return;
            }

            if(_currentProductInputStore >= _maxInputProductStore)
            {
                return;
            }

            int toFill = _maxInputProductStore - _currentProductInputStore;
            if (toFill > info.count)
            {
                toFill = info.count;
            }

            info.count -= toFill;
            // TO DO : find product index and fill

            LocalPackProductInfo facInfo = null;
            for(int i=0; i< _localFacInfo.buildingInputProducts.Count; ++i)
            {
                if(_localFacInfo.buildingInputProducts[i].productName == info.productName)
                {
                    facInfo = _localFacInfo.buildingInputProducts[i];
                }
            }

            if(facInfo == null)
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

            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            cmGame.baseInfo.markDirty();
        }

        public override void showUI()
        {
            base.showUI();

            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);

            // for Debug ...
            var meatInfo = cmGame.Self.GetBackpackProductInfo("meat");
            if(meatInfo != null)
            {
                _fillProduct(meatInfo);

                _refreshInfo();
            }
        }

        protected virtual bool _produceProduct(LocalPackProductInfo inputProd, CMProductMakerConf prodMakerConf)
        {
            if(inputProd.count <= 0)
            {
                return false;
            }

            if (_currentProductOutputStore >= _maxOutputProductStore)
            {
                return false;
            }

            int produceCount = prodMakerConf.produceOutputCount;
            if(inputProd.count < prodMakerConf.costInputCount)
            {
                produceCount = inputProd.count * prodMakerConf.produceOutputCount / prodMakerConf.costInputCount;
                if(produceCount <= 0)
                {
                    produceCount = 1;
                }
            }

            int spaceLeft = _maxInputProductStore - _currentProductInputStore;

            if(produceCount > spaceLeft)
            {
                produceCount = spaceLeft;
            }

            int realCostInputCount = produceCount * prodMakerConf.costInputCount / prodMakerConf.produceOutputCount;
            if(realCostInputCount >= inputProd.count)
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
            for(int i=0; i< _localFacInfo.buildingInputProducts.Count; ++i)
            {
                var inputProd = _localFacInfo.buildingInputProducts[i];
                CMProductMakerConf prodMakerConf = null;
                foreach (var conf in _factoryConf.productMaker)
                {
                    if(inputProd.productName == conf.inputProductName)
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

            if(changed)
            {
                _refreshInfo();
            }
        }

        protected void OnUpdate()
        {
            if(_currentProductInputStore <= 0)
            {
                _factoryCtrl.ProduceProgeress.value = 0.0f;
                return;
            }

            _factoryCtrl.ProduceProgeress.value = (1.0f - (_currentCD / _produceCD))*100;

            _currentCD -= UnityEngine.Time.deltaTime;
            if(_currentCD > 0)
            {
                return;
            }

            _currentCD = _produceCD;

            _doProduce();
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
