using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class CMStoreHouse
    {
        protected CMStoreHouseConf _conf;
        public CMStoreHouseConf storeHouseConf => _conf;

        protected CMStoreHouseLevelConf _currentLevelConf;
        public CMStoreHouseLevelConf currentLevelConf => _currentLevelConf;

        protected LocalStoreHouseInfo _storeHouseInfo;
        public LocalStoreHouseInfo storeHouseInfo => _storeHouseInfo;

        protected List<CMWorker> _workers;
        public List<CMWorker> workers => _workers;

        protected MapBuildingObject _mapBuildingObj;
        public MapBuildingObject mapBuildingObj => _mapBuildingObj;

        protected SpawnPos _fetchPosition;
        public SpawnPos fetchPosition => _fetchPosition;
        protected SpawnPos _storePosition;
        public SpawnPos storePosition => _storePosition;

        UIStoreHousePanel _uiStoreHouse;

        private UIStorehouseCapacityPanel _uiStorehouseCapacityPanel;

        public void Init(LocalStoreHouseInfo info)
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            _storeHouseInfo = info;
            _conf = cmGame.gameConf.gameConfs.storeHouseConf;
            if(!_conf.levelConfs.ContainsKey(_storeHouseInfo.level))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMStoreHouse level [{_storeHouseInfo.level}] config not exist");
                return;
            }
            _currentLevelConf = _conf.levelConfs[_storeHouseInfo.level];

            _mapBuildingObj = null;
            var map = (UnityGameApp.Inst.MainScene.map as Map);
            map.buildings.TryGetValue(_conf.mapBuildName, out _mapBuildingObj);
            if (_mapBuildingObj == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMStoreHouse map building [{_conf.mapBuildName}] not exist");
                return;
            }

            _fetchPosition = map.getSpawnPosByObjectName(_conf.fetchingPosName);
            if (_fetchPosition == null)
            {
                return;
            }
            _storePosition = map.getSpawnPosByObjectName(_conf.storePosName);
            if (_storePosition == null)
            {
                return;
            }

            InitCapacityUI();

            if (info.storeHouseWorkers.Count <= 0)
            {
                info.storeHouseWorkers.Add(new LocalWorkerInfo()
                {
                    position = null,
                    carryProducts = new List<LocalPackProductInfo>()
                });

                cmGame.baseInfo.markDirty();
            }

            _workers = new List<CMWorker>();
            foreach (var workerInfo in info.storeHouseWorkers)
            {
                CMWorker worker = new CMWorker();
                if(!worker.Init(_conf.workerConf, workerInfo, _storeHouseInfo.level))
                {
                    continue;
                }

                worker.MovProdAI.SetTargetName(AIMoveProduct.TargetStoreHouse);
                worker.MovProdAI.SyncFactories();
                _workers.Add(worker);
            }
        }

        public void SyncWorkerFactories()
        {
            foreach(var worker in _workers)
            {
                worker.MovProdAI.SyncFactories();
            }
        }

        public void OnUpdate()
        {
            if (_uiStorehouseCapacityPanel != null)
            {
                var screenPos = UnityGameApp.Inst.ScreenToUIPos((UnityGameApp.Inst.MainScene.camera as UnityGameCamera).worldToScreenPos(_mapBuildingObj.unityGameObject.transform.position));
                _uiStorehouseCapacityPanel.setPoisition((int)screenPos.x, (int)screenPos.y - 200);
            }
        }

        public void setUIPanel(UIStoreHousePanel ui)
        {
            _uiStoreHouse = ui;
        }

        public int TryFetchStoreProduct(int count)
        {
            if(_storeHouseInfo.storeCount <= 0)
            {
                return 0;
            }

            int fetchCount = count;
            if(fetchCount > _storeHouseInfo.storeCount)
            {
                fetchCount = _storeHouseInfo.storeCount;
            }

            _storeHouseInfo.storeCount -= fetchCount;

            if (_uiStorehouseCapacityPanel != null)
            {
                _uiStorehouseCapacityPanel.DoUpdateInputStore(_storeHouseInfo.storeCount, -fetchCount);
            }

            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.baseInfo.markDirty();

            if(_uiStoreHouse != null)
            {
                _uiStoreHouse.refreshInfo();
            }

            return fetchCount;
        }

        public void TryFillStoreProduct(LocalPackProductInfo info)
        {
            if (info.productName != _conf.storeProductName)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"store house [{_conf.mapBuildName}] fill product [{info.productName}] but expect [{_conf.storeProductName}]");
                return;
            }

            if (info.count <= 0)
            {
                return;
            }

            if (_storeHouseInfo.storeCount >= _currentLevelConf.MaxstoreCount)
            {
                return;
            }

            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);

            int toFill = _currentLevelConf.MaxstoreCount - _storeHouseInfo.storeCount;
            if (toFill > info.count)
            {
                toFill = info.count;
            }

            info.count -= toFill;

            _storeHouseInfo.storeCount += toFill;

            if (_uiStorehouseCapacityPanel != null)
            {
                _uiStorehouseCapacityPanel.DoUpdateInputStore(_storeHouseInfo.storeCount, toFill);
            }

            cmGame.baseInfo.markDirty();
            cmGame.uiMainPanel.refreshMeat();
        }

        public bool TryUpgrade()
        {
            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;

            //if (_localFacInfo.level >= cmGame.Self.userLevel)
            //{
            //    // can't bigger than user level
            //    return false;
            //}

            if (!_conf.levelConfs.ContainsKey(_storeHouseInfo.level + 1))
            {
                // for Debug ...
                cmGame.uiMainPanel.NofityMessage(CMGNotifyType.CMG_ERROR, "already max level !");

                return false;
            }

            int upgradeGold = _currentLevelConf.upgradeGoldCost;
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
                _storeHouseInfo.level = _storeHouseInfo.level + 1;
                _currentLevelConf = _conf.levelConfs[_storeHouseInfo.level];

                foreach(var worker in _workers)
                {
                    worker.OnUpgradeLevel(_storeHouseInfo.level);
                }

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

        /// <summary>
        /// 仓库建筑头顶容量显示
        /// </summary>
        protected void InitCapacityUI()
        {
            _uiStorehouseCapacityPanel = UnityGameApp.Inst.UI.createUIPanel("StorehouseCapacityUI") as UIStorehouseCapacityPanel;
            _uiStorehouseCapacityPanel.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.uiRootObject).unityGameObject.transform);
            _uiStorehouseCapacityPanel.showUI();
        }
    }
}
