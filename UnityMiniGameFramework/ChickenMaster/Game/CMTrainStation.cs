using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Debug = MiniGameFramework.Debug;

namespace UnityMiniGameFramework
{
    public class CMTrainStation
    {
        protected CMTrainStationConf _conf;
        public CMTrainStationConf trainStaionConf => _conf;

        protected CMTrainStationLevelConf _currentLevelConf;
        public CMTrainStationLevelConf currentLevelConf => _currentLevelConf;

        protected LocalTrainStationInfo _trainStationInfo;
        public LocalTrainStationInfo trainStationInfo => _trainStationInfo;

        protected List<CMWorker> _workers;
        public List<CMWorker> workers => _workers;

        protected MapBuildingObject _mapBuildingObj;
        public MapBuildingObject mapBuildingObj => _mapBuildingObj;

        protected SpawnPos _putPosition;
        public SpawnPos putPosition => _putPosition;
        protected SpawnPos _storePosition;
        public SpawnPos storePosition => _storePosition;

        protected UITrainStationPanel _uiTrainStation;
        public UITrainStationPanel uiTrainStation => _uiTrainStation;

        public int currTotalStoreCount => _currTotalStoreCount;
        protected int _currTotalStoreCount;

        protected CMTrain _train;
        public CMTrain train => _train;

        protected UnityEngine.GameObject _trainStartPos;
        public UnityEngine.GameObject trainStartPos => _trainStartPos;

        protected UnityEngine.GameObject _trainStopPos;
        public UnityEngine.GameObject trainStopPos => _trainStopPos;
        protected UnityEngine.GameObject _trainMoveoutPos;
        public UnityEngine.GameObject trainMoveoutPos => _trainMoveoutPos;
        private int _colConst;
        private int _rowConst;
        private int _layerCount;
        private float _boxLenght = 0.32f;
        private Vector3 _boxInitPos;
        private UITrainStationCapatityPanel _uiTrainstationCapatityPanel;

        public CMTrainStation()
        {
            _train = new CMTrain();
        }

        public void Init(LocalTrainStationInfo info)
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            _trainStationInfo = info;
            _conf = cmGame.gameConf.gameConfs.trainStationConf;
            if (!_conf.levelConfs.ContainsKey(_trainStationInfo.level))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMTrainStation level [{_trainStationInfo.level}] config not exist");
                return;
            }
            _currentLevelConf = _conf.levelConfs[_trainStationInfo.level];

            // building
            _mapBuildingObj = null;
            var map = (UnityGameApp.Inst.MainScene.map as Map);
            map.buildings.TryGetValue(_conf.mapBuildName, out _mapBuildingObj);
            if (_mapBuildingObj == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMTrainStation map building [{_conf.mapBuildName}] not exist");
                return;
            }

            _putPosition = map.getSpawnPosByObjectName(_conf.putPosName);
            if (_putPosition == null)
            {
                return;
            }
            _storePosition = map.getSpawnPosByObjectName(_conf.storePosName);
            if (_storePosition == null)
            {
                return;
            }

            var tr = map.unityGameObject.transform.Find(_conf.trainStartPosName);
            if (tr == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMTrainStation map train pos [{_conf.trainStartPosName}] not exist");
                return;
            }
            _trainStartPos = tr.gameObject;

            tr = map.unityGameObject.transform.Find(_conf.trainStopPosName);
            if (tr == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMTrainStation map train pos [{_conf.trainStopPosName}] not exist");
                return;
            }
            _trainStopPos = tr.gameObject;

            tr = map.unityGameObject.transform.Find(_conf.trainMoveoutPosName);
            if (tr == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMTrainStation map train pos [{_conf.trainMoveoutPosName}] not exist");
                return;
            }
            _trainMoveoutPos = tr.gameObject;

            // train
            _train.Init(this);

            // workers 
            if (info.trainStationWorkers.Count <= 0)
            {
                info.trainStationWorkers.Add(new LocalWorkerInfo()
                {
                    position = null,
                    carryProducts = new List<LocalPackProductInfo>()
                });

                cmGame.baseInfo.markDirty();
            }

            _workers = new List<CMWorker>();
            foreach (var workerInfo in info.trainStationWorkers)
            {
                CMWorker worker = new CMWorker();
                if (!worker.Init(_conf.workerConf, workerInfo, _trainStationInfo.level))
                {
                    continue;
                }

                worker.MovProdAI.SetTargetName(AIMoveProduct.TargetTrainStation);
                worker.MovProdAI.SyncFactories();
                _workers.Add(worker);
            }

            foreach(var prod in info.storeProducts)
            {
                _currTotalStoreCount += prod.count;
            }
            
            var boxCollider = _storePosition.spawnObject.GetComponent<BoxCollider>();
            if (!boxCollider)
                return;
            Vector3 size = boxCollider.size;
            float length = size.x;
            float width = size.z;
            _colConst = (int) Math.Floor(length / _boxLenght);
            _rowConst = (int) Math.Floor(width / _boxLenght);
            _layerCount = _rowConst * _colConst;
            _boxInitPos = new Vector3(_boxLenght * (_colConst / 2), 0, _boxLenght * (_rowConst / 2));

            InitCapacityUI();
        }

        public void OnUpdate()
        {
            _train.OnUpdate();

            if (_uiTrainstationCapatityPanel != null)
            {
                var screenPos = UnityGameApp.Inst.ScreenToUIPos((UnityGameApp.Inst.MainScene.camera as UnityGameCamera).worldToScreenPos(_mapBuildingObj.unityGameObject.transform.position));
                _uiTrainstationCapatityPanel.setPoisition(screenPos.x - 140, screenPos.y - 150);
            }
        }

        public void SyncWorkerFactories()
        {
            foreach (var worker in _workers)
            {
                worker.MovProdAI.SyncFactories();
            }
        }

        public void setUIPanel(UITrainStationPanel ui)
        {
            _uiTrainStation = ui;
        }

        public List<LocalPackProductInfo> PutProducts(List<LocalPackProductInfo> prods)
        {
            bool changed = false;
            for (int i = 0; i < prods.Count; ++i)
            {
                int spaceLeft = _currentLevelConf.MaxstoreCount - _currTotalStoreCount;
                if (spaceLeft <= 0)
                {
                    break;
                }

                var inputProd = prods[i];

                int inputValue = inputProd.count;
                if (inputValue > spaceLeft)
                {
                    inputValue = spaceLeft;
                    inputProd.count -= inputValue;
                }
                else
                {
                    prods.RemoveAt(i);
                    --i;
                }

                _trainStationInfo.storeProducts.Add(inputProd);
                
                _currTotalStoreCount += inputValue;
                if (_uiTrainstationCapatityPanel != null)
                {
                    _uiTrainstationCapatityPanel.DoUpdateInputStore(_currTotalStoreCount, inputValue);
                }

                changed = true;
            }

            if (changed)
            {
                var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
                cmGame.baseInfo.markDirty();

                if (_uiTrainStation != null)
                {
                    _uiTrainStation.refreshInfo();
                }
                UpdateProductBox();
            }

            return prods;
        }

        private void UpdateProductBox()
        {
            var storeTransform = _storePosition.spawnObject.transform;
            int boxNum = _trainStationInfo.storeProducts.Count;
            int childCount = storeTransform.childCount;
            if (boxNum == childCount)
                return;
            for (int i = childCount - 1; i >= boxNum; i--)
            {
                var box = storeTransform.GetChild(i).gameObject;
                UnityGameObjectPool.GetInstance().PutUnityPrefabObject("box/OutputStoreProduct", box);
            }

            childCount = storeTransform.childCount;
            for (int i = childCount; i < boxNum; i++)
            {
                var obj = UnityGameObjectPool.GetInstance().GetUnityPrefabObject("box/OutputStoreProduct");
                obj.transform.SetParent(storeTransform);
                obj.transform.localEulerAngles = Vector3.zero;
            }

            for (int i = 0; i < storeTransform.childCount; i++)
            {
                int height = i / _layerCount;
                int index = i % _layerCount;
                int row = index % _rowConst;
                int col = index / _colConst;
                Vector3 pos = Vector3.zero;
                pos.x = _boxInitPos.x - _boxLenght * row;
                pos.y = _boxInitPos.y + _boxLenght * height;
                pos.z = _boxInitPos.z - _boxLenght * col;
                var childTf = storeTransform.GetChild(i);
                childTf.localPosition = pos;
            }
        }
        
        public void TrySellTrainStaionProducts()
        {
            if (_trainStationInfo.storeProducts.Count > 0)
            {
                var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                int goldAdd = 0;
                int useSpace = 0;

                for (int i=0; i< _trainStationInfo.storeProducts.Count; ++i)
                {
                    var prod = _trainStationInfo.storeProducts[i];
                    var prodConf = cmGame.gameConf.getCMProductConf(prod.productName);
                    if (prodConf == null)
                    {
                        // error, ignore
                        continue;
                    }

                    int putCount = _currentLevelConf.maxSellCountPerRound - useSpace;
                    if(putCount >= prod.count)
                    {
                        putCount = prod.count;
                        _trainStationInfo.storeProducts.RemoveAt(i);
                        --i;
                    }
                    else
                    {
                        prod.count -= putCount;
                    }

                    useSpace += putCount;
                    _currTotalStoreCount -= putCount;
                    if (_uiTrainstationCapatityPanel != null)
                    {
                        _uiTrainstationCapatityPanel.DoUpdateInputStore(_currTotalStoreCount, putCount);
                    }

                    goldAdd += putCount * prodConf.price;

                    if(useSpace >= _currentLevelConf.maxSellCountPerRound)
                    {
                        break;
                    }
                }

                if (goldAdd > 0)
                {
                    cmGame.Self.AddGold(goldAdd);
                    UpdateProductBox();
                    _train.setBoxShow(true);

                    showGetResInfoUI(goldAdd);
                }

                cmGame.baseInfo.markDirty();

                if (_uiTrainStation != null)
                {
                    _uiTrainStation.refreshInfo();
                }
            }
            else
            {
                // no products


            }

        }

        public bool TryUpgrade()
        {
            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;

            //if (_localFacInfo.level >= cmGame.Self.userLevel)
            //{
            //    // can't bigger than user level
            //    return false;
            //}

            if (!_conf.levelConfs.ContainsKey(_trainStationInfo.level + 1))
            {
                // for Debug ...
                cmGame.ShowTips(CMGNotifyType.CMG_ERROR, "already max level !");

                return false;
            }

            int upgradeGold = _currentLevelConf.upgradeGoldCost;
            if (upgradeGold <= 0)
            {
                // no more level

                // for Debug ...
                cmGame.ShowTips(CMGNotifyType.CMG_ERROR, "already max level !");

                return false;
            }

            if (cmGame.Self.TrySubGold(upgradeGold))
            {
                // upgrade 
                _trainStationInfo.level = _trainStationInfo.level + 1;
                _currentLevelConf = _conf.levelConfs[_trainStationInfo.level];

                foreach (var worker in _workers)
                {
                    worker.OnUpgradeLevel(_trainStationInfo.level);
                }

                cmGame.baseInfo.markDirty();

                UnityGameApp.Inst.RESTFulClient.Report(UnityGameApp.Inst.AnalysisMgr.GetPointData12($"建筑[{_conf.mapBuildName}]等级{_trainStationInfo.level}"));
            }
            else
            {
                // TO DO : not enough gold

                // for Debug ...
                cmGame.ShowTips(CMGNotifyType.CMG_ERROR, "insuffcient gold !");
                return false;
            }

            return true;
        }

        public bool CallTrainNow()
        {
            // TO DO : call train by watch Ad.
            
            var nowTickMillsecond = (DateTime.Now.Ticks / 10000);
            trainStationInfo.NextTrainArrivalTime = nowTickMillsecond;
            train.CallTrainBack();
            return true;
        }

        /// <summary>
        /// 火车站建筑头顶容量显示
        /// </summary>
        protected void InitCapacityUI()
        {
            _uiTrainstationCapatityPanel = UnityGameApp.Inst.UI.createNewUIPanel("TrainStationCapacityUI") as UITrainStationCapatityPanel;
            _uiTrainstationCapatityPanel.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.uiRootObject).unityGameObject.transform);
            _uiTrainstationCapatityPanel.RefreshInfo(this);
            _uiTrainstationCapatityPanel.showUI();
        }
        
        /// <summary>
        /// 售卖金币飘字动效
        /// </summary>
        protected void showGetResInfoUI(int count)
        {
            if (count <= 0)
            {
                return;
            }
            var resPopup = UnityGameApp.Inst.UI.createUIPanel("TrainStationGoldPopupUI") as UITrainStationGoldPopupPanel;
            resPopup.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.uiRootObject).unityGameObject.transform);
            resPopup.SetResInfo(count, 0, _storePosition.spawnObject.transform);
        }
    }
}
