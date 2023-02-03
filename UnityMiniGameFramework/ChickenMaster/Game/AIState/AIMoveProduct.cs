using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniGameFramework;
using UnityEngine;
using GameObject = UnityEngine.GameObject;
using Debug = MiniGameFramework.Debug;

namespace UnityMiniGameFramework
{
    public class AIMoveProduct : AIState
    {
        //public enum WorkerState
        //{
        //    Idle,
        //    MovingToFetcing,
        //    Fetching,
        //    MovingToPut,
        //    Puting,
        //}

        public static readonly string TargetTrainStation = "trainStation";
        public static readonly string TargetStoreHouse = "storeHouse";

        public static AIMoveProduct create(ActorObject actor)
        {
            return new AIMoveProduct(actor);
        }

        protected RigibodyMoveAct _movAct;
        protected List<CMFactory> _factories;
        protected CMWorker _worker;

        protected string _targetName;

        protected Action _onUpdateWorker;

        protected bool _isPlayedActAni;
        protected bool _isPlayingAni;
        protected CMFactory _targetFactory;
        protected Transform _productBone;
        protected GameObject _productGameObject;
        protected string _productPrefabName = string.Empty;

        public AIMoveProduct(ActorObject actor) : base(actor)
        {
            _movAct = (actor as MapRoleObject).moveAct;

            _factories = new List<CMFactory>();
        }

        public override void Init(MapConfAIState conf)
        {
            base.Init(conf);
            _movAct.setMoveType(true);
            _setTargetName(conf.targetName);
        }

        override public void OnUpdate()
        {
            _onUpdateWorker();
        }

        public void SetTargetName(string t)
        {
            _setTargetName(t);
        }

        protected void _setTargetName(string t)
        {
            _targetName = t;

            if (_targetName == TargetTrainStation)
            {
                _onUpdateWorker = _onUpdateTrainStationIdle;
            }
            else if (_targetName == TargetStoreHouse)
            {
                _onUpdateWorker = _updateStoreHouseIdle;
            }
        }
        public void SetWorker(CMWorker worker)
        {
            _worker = worker;
            _productBone = _actor.unityGameObject.transform.Find(_worker.workerConf.productBone);
        }

        public void SyncFactories()
        {
            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            _factories = cmGame.GetFactories();

            // sort by product price
            _factories.Sort((CMFactory l, CMFactory r) =>
            {
                var lprodConf = cmGame.gameConf.getCMProductConf(l.factoryConf.outputProductName);
                var rprodConf = cmGame.gameConf.getCMProductConf(r.factoryConf.outputProductName);

                if(lprodConf == null || rprodConf == null)
                {
                    return 0;
                }

                if(lprodConf.price > rprodConf.price)
                {
                    return 0;
                }

                return 1;
            });
        }

        protected CMFactory _fetchFactoryByInput()
        {
            // first find empty input factory
            //for (int i = 0; i < _factories.Count; ++i)
            //{
            //    var fac = _factories[i];
            //    if (fac.currentProductInputStore <= _worker.maxCarryCount)
            //    {
            //        return fac;
            //    }
            //}

            //// then find most value product factory with more than one pack space
            //for (int i = 0; i < _factories.Count; ++i)
            //{
            //    var fac = _factories[i];
            //    if (fac.maxInputProductStore - fac.currentProductInputStore > _worker.maxCarryCount)
            //    {
            //        return fac;
            //    }
            //}

            // find empty factory with the most value product
            for (int i = _factories.Count - 1; i >= 0; --i)
            {
                var fac = _factories[i];
                if (fac.currentProductInputStore == 0)
                {
                    return fac;
                }
            }
            // then find most value product factory
            for (int i = _factories.Count - 1; i >= 0; --i)
            {
                var fac = _factories[i];
                if(fac.currentProductInputStore < fac.maxInputProductStore)
                {
                    return fac;
                }
            }

            return null;
        }

        protected CMFactory _fetchFactoryByOutput()
        {
            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            int maxValue = 0;
            CMFactory ret = null;
            for (int i = _factories.Count - 1; i >= 0; --i)
            {
                var fac = _factories[i];
                var prodConf = cmGame.gameConf.getCMProductConf(fac.factoryConf.outputProductName);
                if(prodConf == null)
                {
                    continue;
                }

                int carryCount = fac.currentProductOutputStore;
                if(carryCount > _worker.maxCarryCount)
                {
                    carryCount = _worker.maxCarryCount;
                }

                int totalValue = carryCount * prodConf.price;
                if (totalValue > maxValue)
                {
                    ret = fac;
                    maxValue = totalValue;
                }
            }

            return ret;
        }

        protected void _onUpdateTrainStation()
        {
            // TO DO : move product from factory to train station

            // for Debug ...
            // direct sell product
            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            foreach (var fac in _factories)
            {
                if (fac == null)
                {
                    continue;
                }

                if (fac.localFacInfo.buildingOutputProduct == null)
                {
                    continue;
                }

                if (fac.localFacInfo.buildingOutputProduct.count > fac.factoryLevelConf.fetchPackCount)
                {
                    var outputProd = fac.localFacInfo.buildingOutputProduct;
                    var prodConf = cmGame.gameConf.getCMProductConf(outputProd.productName);
                    if (prodConf == null)
                    {
                        continue;
                    }
                    int fetchCount = fac.fetchProduct(fac.factoryLevelConf.fetchPackCount);
                    if (fetchCount > 0)
                    {
                        cmGame.Self.AddGold(prodConf.price * fetchCount);
                    }
                }
            }
        }


        //---------------- train station states -------------------------------
        protected void _onUpdateTrainStationIdle()
        {
            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);

            if (_worker.workerInfo.carryProducts.Count > 0)
            {
                // already carry, go to put

                if(cmGame.TrainStation.putPosition.isPositionIn(_actor.unityGameObject.transform.position))
                {
                    // already in putting position
                    _movAct.stop();

                    _onUpdateWorker = _updateTrainStationPutting;
                    _actor.animatorComponent.playAnimation(ActAnis.IdleAni);
                    _isPlayedActAni = false;
                    _isPlayingAni = false;
                }
                else
                {
                    _movAct.setMovingAni(_worker.workerConf.carryMovingAniName);
                    addProductGo(_worker.workerConf.productPrefab);
                    _movAct.moveOn(new List<UnityEngine.Vector3>() { cmGame.TrainStation.putPosition.randSpawnPos() }, 0.5f);
                    _movAct.speedUpBuff = GetSpeedUpBuff();
                    _onUpdateWorker = _updateTrainStationMovingToPut;
                }

                return;
            }

            // not carry, go to fetching

            // find best value factory
            _targetFactory = _fetchFactoryByOutput();
            if(_targetFactory == null)
            {
                // no factory
                return;
            }


            if (_targetFactory.outputFetchPosition.isPositionIn(_actor.unityGameObject.transform.position))
            {
                // already in fetching position
                _movAct.stop();

                _onUpdateWorker = _updateTrainStationFetching;
                _actor.animatorComponent.playAnimation(ActAnis.IdleAni);
                _isPlayedActAni = false;
                _isPlayingAni = false;
            }
            else
            {
                // go to fetching position

                _movAct.setMovingAni(ActAnis.RunAni);
                removeProductGo();
                _movAct.moveOn(new List<UnityEngine.Vector3>() { _targetFactory.outputFetchPosition.randSpawnPos() }, 0.5f);
                _movAct.speedUpBuff = GetSpeedUpBuff();
                _onUpdateWorker = _updateTrainStationMovingToFetching;
            }

        }

        protected void _updateTrainStationMovingToFetching()
        {
            if (_movAct.isMoving)
            {
                // still moving
                return;
            }

            _onUpdateWorker = _updateTrainStationFetching;
            _actor.animatorComponent.playAnimation(ActAnis.IdleAni);
            removeProductGo();
            _isPlayedActAni = false;
            _isPlayingAni = false;
        }

        protected void _updateTrainStationFetching()
        {
            // wait fetch
            if(_targetFactory == null)
            {
                // error 
                _onUpdateWorker = _onUpdateTrainStationIdle;
                return;
            }

            // playing fetch animation
            if (!_isPlayedActAni)
            {
                if (!_isPlayingAni)
                {
                    _actor.animatorComponent.playAnimation(_worker.workerConf.fetchingAniName);
                    addProductGo(_worker.workerConf.productPrefab);
                    _isPlayingAni = true;
                }
                else
                {
                    if (_actor.animatorComponent.unityAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
                    {
                        _actor.animatorComponent.playAnimation(ActAnis.IdleAni);
                        _isPlayedActAni = true;
                    }
                }
            }

            if (!_isPlayedActAni)
            {
                return;
            }

            // fetch
            int fetchedCount = _targetFactory.fetchProduct(_worker.maxCarryCount);
            if (fetchedCount > 0)
            {
                _worker.workerInfo.carryProducts.Add(new LocalPackProductInfo()
                {
                    productName = _targetFactory.factoryConf.outputProductName,
                    count = fetchedCount
                });


                // go to putting position

                var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
                _movAct.setMovingAni(_worker.workerConf.carryMovingAniName);
                addProductGo(_worker.workerConf.productPrefab);
                _movAct.moveOn(new List<UnityEngine.Vector3>() { cmGame.TrainStation.putPosition.randSpawnPos() }, 0.5f);
                _movAct.speedUpBuff = GetSpeedUpBuff();
                _onUpdateWorker = _updateTrainStationMovingToPut;
            }
        }

        protected void _updateTrainStationMovingToPut()
        {
            if (_movAct.isMoving)
            {
                // still moving
                return;
            }

            _onUpdateWorker = _updateTrainStationPutting;
            _actor.animatorComponent.playAnimation(ActAnis.IdleAni);
            _isPlayedActAni = false;
            _isPlayingAni = false;
        }

        protected void _updateTrainStationPutting()
        {
            if (_worker.workerInfo.carryProducts.Count <= 0)
            {
                // error
                _onUpdateWorker = _onUpdateTrainStationIdle;
                return;
            }

            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            if (cmGame.TrainStation.currTotalStoreCount >= cmGame.TrainStation.currentLevelConf.MaxstoreCount)
            {
                // no space for input, wait
                return;
            }

            // playing putting animation
            if (!_isPlayedActAni)
            {
                if (!_isPlayingAni)
                {
                    _actor.animatorComponent.playAnimation(_worker.workerConf.putAniName);
                    _isPlayingAni = true;
                }
                else
                {
                    if (_actor.animatorComponent.unityAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
                    {
                        _actor.animatorComponent.playAnimation(ActAnis.IdleAni);
                        _isPlayedActAni = true;
                    }
                }
            }

            if (!_isPlayedActAni)
            {
                return;
            }

            _worker.workerInfo.carryProducts = cmGame.TrainStation.PutProducts(_worker.workerInfo.carryProducts);

            cmGame.baseInfo.markDirty();

            _onUpdateWorker = _onUpdateTrainStationIdle;
        }


        //---------------- store house states ---------------------------------
        protected void _updateStoreHouseIdle()
        {
            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);

            if (_worker.workerInfo.carryProducts.Count > 0)
            {
                // already carry, go to put

                _onUpdateWorker = _updateStoreHouseMovingToPut;
            }
            else
            {
                // not carry, go to fetching

                if(cmGame.StoreHouse.fetchPosition.isPositionIn(_actor.unityGameObject.transform.position))
                {
                    // already in fetching position
                    _movAct.stop();

                    _onUpdateWorker = _updateStoreHouseFetching;
                    _actor.animatorComponent.playAnimation(ActAnis.IdleAni);
                    _isPlayedActAni = false;
                    _isPlayingAni = false;
                }
                else
                {
                    // go to fetching position

                    _movAct.setMovingAni(ActAnis.RunAni);
                    _movAct.moveOn(new List<UnityEngine.Vector3>() { cmGame.StoreHouse.fetchPosition.randSpawnPos() }, 0.5f);
                    _movAct.speedUpBuff = GetSpeedUpBuff();
                    removeProductGo();
                    _onUpdateWorker = _updateStoreHouseMovingToFetching;
                }

            }

        }

        protected void _updateStoreHouseMovingToFetching()
        {
            if(_movAct.isMoving)
            {
                // still moving
                return;
            }

            _onUpdateWorker = _updateStoreHouseFetching;
            _actor.animatorComponent.playAnimation(ActAnis.IdleAni);
            _isPlayedActAni = false;
            _isPlayingAni = false;
        }

        protected void _updateStoreHouseFetching()
        {
            // wait fetch

            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            if (cmGame.StoreHouse.storeHouseInfo.storeCount <= 0)
            {
                return;
            }

            // playing fetch animation
            if(!_isPlayedActAni)
            {
                if(!_isPlayingAni)
                {
                    _actor.animatorComponent.playAnimation(_worker.workerConf.fetchingAniName);
                    _isPlayingAni = true;
                    addProductGo(_worker.workerConf.productPrefab);
                }
                else
                {
                    if(_actor.animatorComponent.unityAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
                    {
                        _actor.animatorComponent.playAnimation(ActAnis.IdleAni);
                        _isPlayedActAni = true;
                    }
                }
            }

            if(!_isPlayedActAni)
            {
                return;
            }

            // fetch
            int fetchedCount = cmGame.StoreHouse.TryFetchStoreProduct(_worker.maxCarryCount);
            if(fetchedCount > 0)
            {
                _worker.workerInfo.carryProducts.Add(new LocalPackProductInfo()
                {
                    productName = cmGame.StoreHouse.storeHouseConf.storeProductName,
                    count = fetchedCount
                });

                _onUpdateWorker = _updateStoreHouseMovingToPut;
                _targetFactory = null;
            }
        }

        protected void _updateStoreHouseMovingToPut()
        {
            if(_targetFactory == null)
            {
                // find target factory
                _targetFactory = _fetchFactoryByInput();

                if (_targetFactory == null)
                {
                    // no empty input factory
                    return;
                }
                else
                {
                    _movAct.setMovingAni(_worker.workerConf.carryMovingAniName);
                    addProductGo(_worker.workerConf.productPrefab);
                    _movAct.moveOn(new List<UnityEngine.Vector3>() { _targetFactory.inputPutPosition.randSpawnPos() }, 0.5f);
                    _movAct.speedUpBuff = GetSpeedUpBuff();
                }
            }
            // 有目标但目标input已经满了时
            if (_targetFactory != null)
            {
                if (_targetFactory.currentProductInputStore >= _targetFactory.maxInputProductStore)
                {
                    var fac = _fetchFactoryByInput();
                    if (fac != null && fac.factoryName != _targetFactory.factoryName)
                    {
                        _targetFactory = fac;
                        _movAct.setMovingAni(_worker.workerConf.carryMovingAniName);
                        addProductGo(_worker.workerConf.productPrefab);
                        _movAct.moveOn(new List<UnityEngine.Vector3>() { _targetFactory.inputPutPosition.randSpawnPos() }, 0.5f);
                        _movAct.speedUpBuff = GetSpeedUpBuff();
                    }
                }
            }

            if (_movAct.isMoving)
            {
                // still moving
                return;
            }

            _onUpdateWorker = _updateStoreHousePutting;
            _actor.animatorComponent.playAnimation(ActAnis.IdleAni);
            _isPlayedActAni = false;
            _isPlayingAni = false;
        }

        protected void addProductGo(string _prefab)
        {
            if (_productPrefabName == _prefab)
                return;
            if (_productGameObject != null) 
                removeProductGo();
            _productGameObject = UnityGameObjectPool.GetInstance().GetUnityPrefabObject(_prefab);
            _productPrefabName = _prefab;
            if (_productGameObject != null)
            {
                _productGameObject.transform.SetParent(_productBone);
                var pos = _worker.workerConf.productPos;
                _productGameObject.transform.localPosition = new Vector3(pos.x, pos.y, pos.z);
                var rotation = _worker.workerConf.productRotation;
                if (rotation != null)
                {
                    _productGameObject.transform.localRotation =
                        UnityEngine.Quaternion.Euler(rotation.x, rotation.y, rotation.z);
                }
            }
                
        }

        protected void removeProductGo()
        {
            if (_productPrefabName == string.Empty || _productGameObject == null)
                return;
            UnityGameObjectPool.GetInstance().PutUnityPrefabObject(_productPrefabName, _productGameObject);
            _productGameObject = null;
            _productPrefabName = string.Empty;
        }
        
        protected void _updateStoreHousePutting()
        {
            if(_targetFactory == null)
            {
                // error
                _onUpdateWorker = _updateStoreHouseIdle;
                return;
            }

            if(_worker.workerInfo.carryProducts.Count <= 0)
            {
                // error
                _onUpdateWorker = _updateStoreHouseIdle;
                return;
            }

            if(_targetFactory.currentProductInputStore >= _targetFactory.maxInputProductStore)
            {
                // no space for input, wait
                return;
            }

            // playing putting animation
            if (!_isPlayedActAni)
            {
                if (!_isPlayingAni)
                {
                    _actor.animatorComponent.playAnimation(_worker.workerConf.putAniName);
                    _isPlayingAni = true;
                }
                else
                {
                    if (_actor.animatorComponent.unityAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
                    {
                        _actor.animatorComponent.playAnimation(ActAnis.IdleAni);
                        _isPlayedActAni = true;
                    }
                }
            }

            if (!_isPlayedActAni)
            {
                return;
            }

            var toFillProd = new LocalPackProductInfo()
            {
                productName = _targetFactory.factoryConf.inputProductName,
                count = 0
            };
            foreach(var prod in _worker.workerInfo.carryProducts)
            {
                if(prod.productName != toFillProd.productName)
                {
                    // err, ignore
                    continue;
                }

                toFillProd.count += prod.count;
            }

            _targetFactory.fillProduct(toFillProd);
            _worker.workerInfo.carryProducts.Clear();

            if(toFillProd.count > 0)
            {
                // product left, give back to worker
                _worker.workerInfo.carryProducts.Add(toFillProd);
            }

            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            cmGame.baseInfo.markDirty();

            _onUpdateWorker = _updateStoreHouseIdle;
        }

        private float GetSpeedUpBuff()
        {
            float buff = 0f;
            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            var bi = cmGame.baseInfo.getData() as LocalBaseInfo;
            long nowMillisecond = (long)(DateTime.Now.Ticks / 10000);

            switch (_targetName)
            {
                case "storeHouse":
                    buff = bi.buffs.storehouseProterSpeed < nowMillisecond ? 0f : 1f;
                    break;
                case "trainStation":
                    buff = bi.buffs.trainProterSpeed < nowMillisecond ? 0f : 1f;
                    break;

                default:
                    break;
            }

            return buff;
        }
    }
}
