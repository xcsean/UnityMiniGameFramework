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

    public class Carriage
    {
        private UnityEngine.GameObject _gameObject;
        private Animator _animator;
        private Transform _nextCarriageNode;
        public Transform NextCarriageNode => _nextCarriageNode;
        private UnityEngine.GameObject _boxRoot;

        public Carriage(Transform parent)
        {
            _gameObject = UnityGameApp.Inst.UnityResource.LoadUnityPrefabObject("actor/Carriage");
            _gameObject = UnityEngine.GameObject.Instantiate(_gameObject);
            _gameObject.transform.SetParent(parent);
            _gameObject.transform.localPosition = Vector3.zero;
            _boxRoot = _gameObject.transform.Find("Dummy005/Bone002/boxes").gameObject;
            _nextCarriageNode = _gameObject.transform.Find("nextCarriage");
            _animator = _gameObject.GetComponent<Animator>();
        }
        

        public void ShowBox(bool isShow)
        {
            _boxRoot.SetActive(isShow);
        }

        public void PlayAnimation(bool isRun)
        {
            _animator.Play(isRun ? "run" : "stop");
        }
        
    }
    public class CMTrain
    {
        protected MapNPCObject _trainNpcObj;
        public MapNPCObject trainNpcObj => _trainNpcObj;

        protected long _timeToTrainArrival;
        public long timeToTrainArrival => _timeToTrainArrival;

        protected CMTrainStation _station;

        protected float _onboardTimeLeft;

        protected bool _isInited;
        private List<Carriage> _carriageList = new List<Carriage>();
        
        
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
            
            _trainNpcObj.moveAct.SetRotationAdd(new Vector3(0.0f, -90.0f, 0.0f));
            _trainNpcObj.moveAct.setMoveType(true);
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
            var CarriageNode =  _trainNpcObj.unityGameObject.transform.Find("CarriageNode");

            for (int i = 0; i < _station.currentLevelConf.TrainCarriageCount; i++)
            {
                var nextNode = new Carriage(CarriageNode);
                _carriageList.Add(nextNode);
                CarriageNode = nextNode.NextCarriageNode;
            }
            setBoxShow(false);
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

        public void setBoxShow(bool isShow)
        {
            foreach (var carriage in _carriageList)
            {
                carriage.ShowBox(isShow);
            }
        }

        private void carriagePlayAnimation(bool isRun)
        {
            foreach (var carriage in _carriageList)
            {
                carriage.PlayAnimation(isRun);
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
                setBoxShow(false);
                carriagePlayAnimation(true);
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
                    UnityGameApp.Inst.AudioManager.PlaySFXByAudioName("TrainCome");
                    carriagePlayAnimation(false);
                    _onboardTimeLeft = _station.trainStaionConf.trainOnboardTime;

                    if (_trainNpcObj.unityGameObject.transform.Find("Eff_Train_2").gameObject.activeSelf == false)
                    {
                        _trainNpcObj.unityGameObject.transform.Find("Eff_Train_2").gameObject.SetActive(true);
                    }
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
                    _station.TrySellTrainStaionProducts();

                    var nowTickMillsecond = (DateTime.Now.Ticks / 10000);
                    _station.trainStationInfo.NextTrainArrivalTime = nowTickMillsecond + (long)(_station.trainStaionConf.trainArriveTime * 1000);
                    _timeToTrainArrival = _station.trainStationInfo.NextTrainArrivalTime - nowTickMillsecond;
                    if(_timeToTrainArrival <=0)
                    {
                        _timeToTrainArrival = 1; // atleast 1
                    }
                    carriagePlayAnimation(true);
                    _trainNpcObj.moveAct.moveOn(new List<UnityEngine.Vector3>() { _station.trainMoveoutPos.transform.position }, 0.1f); // move out

                    if (_trainNpcObj.unityGameObject.transform.Find("Eff_Train_2").gameObject.activeSelf)
                    {
                        _trainNpcObj.unityGameObject.transform.Find("Eff_Train_2").gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
