using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Debug = MiniGameFramework.Debug;
using GameObject = UnityEngine.GameObject;

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

        protected MapBuildingObject _mapBuildingObj;
        public MapBuildingObject mapBuildingObj => _mapBuildingObj;

        protected SpawnPos _inputPutPosition;
        public SpawnPos inputPutPosition => _inputPutPosition;
        protected SpawnPos _inputStorePosition;
        public SpawnPos inputStorePosition => _inputStorePosition;
        protected SpawnPos _outputFetchPosition;
        public SpawnPos outputFetchPosition => _outputFetchPosition;
        protected SpawnPos _outputStorePosition;
        public SpawnPos outputStorePosition => _outputStorePosition;

        public LocalFactoryInfo localFacInfo => _localFacInfo;
        protected LocalFactoryInfo _localFacInfo;
        // 头顶生产进度条
        protected UIProduceProgressPanel _produceProgressPanel;
        protected UIProduceProgressPanel produceProgressPanel => _produceProgressPanel;

        public float produceCD => _factoryLevelConf.produceCD;
        public int maxInputProductStore => _factoryLevelConf.maxInputProductStore;
        public int maxOutputProductStore => _factoryLevelConf.maxOutputProductStore;

        public int currentProductInputStore => _localFacInfo.buildingInputProduct == null ? 0 : _localFacInfo.buildingInputProduct.count;
        public int currentProductOutputStore => _localFacInfo.buildingOutputProduct == null ? 0 : _localFacInfo.buildingOutputProduct.count;

        public float currentCD => _currentCD;
        protected float _currentCD;

        protected int _produceVer;
        public int produceVer => _produceVer;
        private UnityEngine.GameObject _outPutGo;
        private UnityEngine.GameObject _inPutGo;

        public bool Init(LocalFactoryInfo facInfo)
        {
            _factoryName = facInfo.mapBuildName;
            _localFacInfo = facInfo;

            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            _factoryConf = cmGame.gameConf.getCMFactoryConf(_factoryName);
            if (_factoryConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMFactory [{_factoryName}] init config not exist");
                return false;
            }

            if (!_factoryConf.levelConfs.TryGetValue(_localFacInfo.level, out _factoryLevelConf))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMFactory [{_factoryName}] level [{_localFacInfo.level}] config not exist");
                return false;
            }

            _mapBuildingObj = null;
            var map = (UnityGameApp.Inst.MainScene.map as Map);
            map.buildings.TryGetValue(_factoryConf.mapBuildName, out _mapBuildingObj);
            if (_mapBuildingObj == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMFactory map building [{_factoryConf.mapBuildName}] not exist");
                return false;
            }

            _inputPutPosition = map.getSpawnPosByObjectName(_factoryConf.inputPutPosName);
            if (_inputPutPosition == null)
            {
                return false;
            }
            _inputStorePosition = map.getSpawnPosByObjectName(_factoryConf.inputStorePosName);
            if (_inputStorePosition == null)
            {
                return false;
            }

            _outputFetchPosition = map.getSpawnPosByObjectName(_factoryConf.outputFetchingPosName);
            if (_outputFetchPosition == null)
            {
                return false;
            }
            _outputStorePosition = map.getSpawnPosByObjectName(_factoryConf.outputStorePosName);
            if (_outputStorePosition == null)
            {
                return false;
            }

            _currentCD = produceCD;
            _produceVer = 0;
            
            //todo:data与control需要分离
            //todo:箱子初始化显示
            
            InitProduceProgressUI();
            return true;
        }

        /// <summary>
        /// 生产进度条
        /// </summary>
        protected void InitProduceProgressUI()
        {
            // init produce progress UI
            _produceProgressPanel = UnityGameApp.Inst.UI.createUIPanel("ProduceProgressUI") as UIProduceProgressPanel;
            _produceProgressPanel.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.uiRootObject).unityGameObject.transform);
            _produceProgressPanel.RefreshInfo(this);
            _produceProgressPanel.showUI();
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

            if (!_factoryConf.levelConfs.ContainsKey(_localFacInfo.level + 1))
            {
                // for Debug ...
                cmGame.uiMainPanel.NofityMessage(CMGNotifyType.CMG_ERROR, "already max level !");

                return false;
            }

            int upgradeGold = getUpgradeGoldCost();
            if (upgradeGold <= 0)
            {
                // no more level

                // for Debug ...
                cmGame.uiMainPanel.NofityMessage(CMGNotifyType.CMG_ERROR, "already max level !");

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

                // for Debug ...
                cmGame.uiMainPanel.NofityMessage(CMGNotifyType.CMG_ERROR, "insuffcient gold !");
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

            if (_localFacInfo.buildingInputProduct == null)
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

            if (produceProgressPanel != null)
            {
                produceProgressPanel.DoUpdateInputStore(_localFacInfo.buildingInputProduct.count, toFill);
            }

            _updateProductBox(factoryConf.inputStorePrefabPath, ref _inPutGo,
                currentProductInputStore / ((ChickenMasterGame) UnityGameApp.Inst.Game).StoreHouse.currentLevelConf
                .fetchPackCount, _inputStorePosition);

            Debug.DebugOutput(DebugTraceType.DTT_Debug, $"仓库到工厂，增加原料数量：{toFill}，原料总数量：{_localFacInfo.buildingInputProduct.count}");
            cmGame.baseInfo.markDirty();
            cmGame.uiMainPanel.refreshMeat();
        }

        public virtual int fetchProduct(int value)
        {
            if (_localFacInfo.buildingOutputProduct == null)
            {
                return 0;
            }

            int fetchCount = value;
            if (fetchCount > _localFacInfo.buildingOutputProduct.count)
            {
                fetchCount = _localFacInfo.buildingOutputProduct.count;
            }

            _localFacInfo.buildingOutputProduct.count -= fetchCount;

            if (produceProgressPanel != null)
            {
                produceProgressPanel.DoUpdateOutStore(_localFacInfo.buildingOutputProduct.count, -fetchCount);
            }

            Debug.DebugOutput(DebugTraceType.DTT_Debug, $"{_localFacInfo.level}级工厂到车站，搬运数量：{fetchCount}，工厂剩余数量：{_localFacInfo.buildingOutputProduct.count}");
            _updateProductBox(factoryConf.outputStorePrefabPath, ref _outPutGo,
                currentProductOutputStore / _factoryLevelConf.fetchPackCount, _outputStorePosition);
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

            if (produceProgressPanel != null)
            {
                // 判空
                if (_localFacInfo.buildingOutputProduct != null)
                    produceProgressPanel.DoUpdateInputStore(_localFacInfo.buildingOutputProduct.count, -realCostInputCount);
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

            if (produceProgressPanel != null)
            {
                produceProgressPanel.DoUpdatePruduceGoods(_localFacInfo.buildingOutputProduct.count, produceCount);
            }

            Debug.DebugOutput(DebugTraceType.DTT_Debug, $"{_localFacInfo.level}级工厂原料数量：{currentProductInputStore}，产出数量：{produceCount}，产出总数量：{currentProductOutputStore}");

            _updateProductBox(factoryConf.inputStorePrefabPath, ref _inPutGo,
                currentProductInputStore / ((ChickenMasterGame) UnityGameApp.Inst.Game).StoreHouse.currentLevelConf
                .fetchPackCount, _inputStorePosition);
            _updateProductBox(factoryConf.outputStorePrefabPath, ref _outPutGo,
                currentProductOutputStore / _factoryLevelConf.fetchPackCount, _outputStorePosition);
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

        protected virtual void _updateProductBox(string boxPrefabPath, ref GameObject boxObject, int boxNum,SpawnPos spawnPos)
        {
            var putTf = spawnPos.spawnObject.transform;
            int childCount = putTf.childCount;
            if (childCount == boxNum)
                return;
            if (!boxObject)
            {
                boxObject = UnityGameApp.Inst.UnityResource.LoadUnityPrefabObject(boxPrefabPath);
            }

            // remove 
            for (int i = childCount - 1; i >= boxNum; i--)
            {
                var box = putTf.GetChild(i).gameObject;
                GameObject.Destroy(box);
            }

            childCount = putTf.childCount;
            // add
            for (int i = childCount; i < boxNum; i++)
            {
                var obj = GameObject.Instantiate(boxObject);
                obj.transform.SetParent(putTf);
                obj.transform.localEulerAngles = Vector3.zero;
            }
            var boxCollider = spawnPos.spawnObject.GetComponent<BoxCollider>();
            if(!boxCollider)
                return;
            Vector3 size = boxCollider.size;
            float boxLenght = 0.32f;
            float length = size.x;
            float width = size.z;
            int colConst = (int) Math.Floor(length / boxLenght);
            int rowConst = (int) Math.Floor(width / boxLenght);
            int layerCount = colConst * rowConst;
            Vector3 initPos = new Vector3(boxLenght, 0, boxLenght);
            for (int i = 0; i < putTf.childCount; i++)
            {
                int height = i / layerCount;
                int index = i % layerCount;
                int row = index % rowConst;
                int col = index / colConst;
                Vector3 pos = Vector3.zero;
                pos.x = initPos.x - boxLenght * row;
                pos.y = initPos.y + boxLenght * height;
                pos.z = initPos.z - boxLenght * col;
                var childTf = putTf.GetChild(i);
                childTf.localPosition = pos;
            }
        }
        
        public void OnUpdate()
        {
            if (_produceProgressPanel != null)
            {
                var screenPos = UnityGameApp.Inst.ScreenToUIPos((UnityGameApp.Inst.MainScene.camera as UnityGameCamera).worldToScreenPos(_mapBuildingObj.unityGameObject.transform.position));
                _produceProgressPanel.setPoisition((int)screenPos.x, (int)screenPos.y - 200); // todo offsetY, set to config
            }

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
