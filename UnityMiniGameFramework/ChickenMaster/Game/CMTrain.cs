using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class CMTrain
    {
        protected MapNPCObject _trainNpcObj;
        public MapNPCObject trainNpcObj => _trainNpcObj;

        protected long _timeToTrainArrival;
        public long timeToTrainArrival => _timeToTrainArrival;

        protected CMTrainStation _station;

        protected float _onboardTimeLeft;

        protected bool _isInited;

        public bool Init(CMTrainStation s)
        {
            _station = s;

            // train
            var mapNpcConf = UnityGameApp.Inst.MapManager.MapConf.getMapNPCConf(_station.trainStaionConf.trainMapNpcName);
            if (mapNpcConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMTrainStation init train config [{_station.trainStaionConf.trainMapNpcName}] not exist");
                return false;
            }

            // create train npc
            var unityNpcObj = UnityGameApp.Inst.UnityResource.LoadUnityPrefabObject(mapNpcConf.prefabName);
            unityNpcObj = UnityEngine.GameObject.Instantiate(unityNpcObj);
            var mgObj = unityNpcObj.GetComponent<UnityGameObjectBehaviour>();
            if (mgObj == null)
            {
                UnityEngine.GameObject.Destroy(unityNpcObj);
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMTrainStation init train prefab [{mapNpcConf.prefabName}] without UnityGameObjectBehaviour");
                return false;
            }

            _trainNpcObj = mgObj.mgGameObject as MapNPCObject;
            if (_trainNpcObj == null)
            {
                mgObj.mgGameObject.Dispose();
                UnityEngine.GameObject.Destroy(unityNpcObj);
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMTrainStation init train prefab [{mapNpcConf.prefabName}] not MapNPCObject");
                return false;
            }

            _initTrain();

            // add train to scene
            unityNpcObj.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);

            _isInited = true;

            return true;
        }

        protected void _initTrain()
        {
            var nowTickMillsecond = (DateTime.Now.Ticks / 10000);
            _timeToTrainArrival = _station.trainStationInfo.NextTrainArrivalTime - nowTickMillsecond;

            if (_timeToTrainArrival > 0)
            {
                _trainNpcObj.unityGameObject.transform.forward = _station.trainStartPos.transform.forward;
                _trainNpcObj.unityGameObject.SetActive(false); // hide train
                _onboardTimeLeft = 0;
            }
            else
            {
                //_trainNpcObj.moveAct.directSetPosition(_station.trainStopPos.transform.position);
                //_trainNpcObj.moveAct.directSetPosition(_station.trainStartPos.transform.forward);
                //_onboardTimeLeft = _station.trainStaionConf.trainOnboardTime;
                //_trainNpcObj.unityGameObject.SetActive(true); // show train

                // train move to stop
                _trainNpcObj.moveAct.directSetPosition(_station.trainStartPos.transform.position); // set to start pos
                _trainNpcObj.unityGameObject.SetActive(true); // show train
                _trainNpcObj.moveAct.moveOn(new List<UnityEngine.Vector3>() { _station.trainStopPos.transform.position }, 0.1f); // move to stop

                _timeToTrainArrival = 0;
                _onboardTimeLeft = 0;

            }

        }

        public void OnUpdate()
        {
            if(!_isInited)
            {
                return;
            }

            if (_timeToTrainArrival > 0)
            {
                if(!_trainNpcObj.moveAct.isMoving)
                {
                    _trainNpcObj.unityGameObject.SetActive(false); // hide train
                }

                // waiting train arrive

                var nowTickMillsecond = (DateTime.Now.Ticks / 10000);
                _timeToTrainArrival = _station.trainStationInfo.NextTrainArrivalTime - nowTickMillsecond;
                if (_timeToTrainArrival > 0)
                {
                    return;
                }

                _timeToTrainArrival = 0;
                _station.trainStationInfo.NextTrainArrivalTime = nowTickMillsecond + (long)(_station.trainStaionConf.trainArriveTime * 1000);

                // train move to stop
                _trainNpcObj.moveAct.directSetPosition(_station.trainStartPos.transform.position); // set to start pos
                _trainNpcObj.unityGameObject.SetActive(true); // show train
                _trainNpcObj.moveAct.moveOn(new List<UnityEngine.Vector3>() { _station.trainStopPos.transform.position }, 0.1f); // move to stop

                return;
            }

            // train start arrive

            if(_onboardTimeLeft <= 0)
            {
                if (_trainNpcObj.moveAct.isMoving)
                {
                    // still moving
                    return;
                }
                else
                {
                    // arrived, start onboard
                    _onboardTimeLeft = _station.trainStaionConf.trainOnboardTime;
                }
            }

            // train arrived
            if(_onboardTimeLeft > 0)
            {
                // TO DO: show onboard animation

                _onboardTimeLeft -= UnityEngine.Time.deltaTime;

                if(_onboardTimeLeft <= 0)
                {
                    // onbard finish, sell item and leave
                    if (_station.trainStationInfo.storeProducts.Count > 0)
                    {
                        var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                        int goldAdd = 0;
                        foreach (var prod in _station.trainStationInfo.storeProducts)
                        {
                            var prodConf = cmGame.gameConf.getCMProductConf(prod.productName);
                            if (prodConf == null)
                            {
                                // error, ignore
                                continue;
                            }

                            goldAdd += prod.count * prodConf.price;
                        }

                        if (goldAdd > 0)
                        {
                            cmGame.Self.AddGold(goldAdd);
                        }

                        _station.ClearStoreProducts();

                        cmGame.baseInfo.markDirty();

                        if (_station.uiTrainStation != null)
                        {
                            _station.uiTrainStation.refreshInfo();
                        }
                    }
                    else
                    {
                        // no products


                    }

                    var nowTickMillsecond = (DateTime.Now.Ticks / 10000);
                    _station.trainStationInfo.NextTrainArrivalTime = nowTickMillsecond + (long)(_station.trainStaionConf.trainArriveTime * 1000);
                    _timeToTrainArrival = _station.trainStationInfo.NextTrainArrivalTime - nowTickMillsecond;
                    if(_timeToTrainArrival <=0)
                    {
                        _timeToTrainArrival = 1; // atleast 1
                    }
                    _trainNpcObj.moveAct.moveOn(new List<UnityEngine.Vector3>() { _station.trainMoveoutPos.transform.position }, 0.1f); // move out
                }
            }
        }
    }
}
