using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class CMWorker
    {
        protected CMWorkerConf _workerConf;
        public CMWorkerConf workerConf => _workerConf;

        protected LocalWorkerInfo _workerInfo;
        public LocalWorkerInfo workerInfo => _workerInfo;

        protected MapNPCObject _mapNpcObj;
        protected AIMoveProduct _movProdAI;
        public AIMoveProduct MovProdAI => _movProdAI;

        protected int _workerLevel;
        public int workerLevel => _workerLevel;

        protected int _maxCarryCount;
        public int maxCarryCount => _maxCarryCount;

        public bool Init(CMWorkerConf conf, LocalWorkerInfo info, int level)
        {
            _workerConf = conf;
            _workerInfo = info;
            _workerLevel = level;

            if(!conf.levelCarryCount.ContainsKey(_workerLevel))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMWorker init worker [{_workerConf.mapNpcName}] level [{_workerLevel}] carry count not exist, use level 1");
                _maxCarryCount = conf.levelCarryCount[1];
            }
            else
            {
                _maxCarryCount = conf.levelCarryCount[_workerLevel];
            }

            var mapNpcConf = UnityGameApp.Inst.MapManager.MapConf.getMapNPCConf(_workerConf.mapNpcName);
            if(mapNpcConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMWorker init worker config [{_workerConf.mapNpcName}] not exist");
                return false;
            }

            // create worker npc
            var unityNpcObj = UnityGameApp.Inst.UnityResource.LoadUnityPrefabObject(mapNpcConf.prefabName);
            unityNpcObj = UnityEngine.GameObject.Instantiate(unityNpcObj);
            var mgObj = unityNpcObj.GetComponent<UnityGameObjectBehaviour>();
            if (mgObj == null)
            {
                UnityEngine.GameObject.Destroy(unityNpcObj);
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMWorker init worker prefab [{mapNpcConf.prefabName}] without UnityGameObjectBehaviour");
                return false;
            }

            _mapNpcObj = mgObj.mgGameObject as MapNPCObject;
            if (_mapNpcObj == null)
            {
                mgObj.mgGameObject.Dispose();
                UnityEngine.GameObject.Destroy(unityNpcObj);
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMWorker init worker prefab [{mapNpcConf.prefabName}] not MapNPCObject");
                return false;
            }

            // add to scene
            unityNpcObj.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);

            if (_workerInfo.position == null)
            {
                // set born position
                unityNpcObj.transform.position = UnityGameApp.Inst.MainScene.implMap.getNamedBornPos(_workerConf.initSpawnPosName);
                _workerInfo.position = new JsonConfVector3()
                {
                    x = unityNpcObj.transform.position.x,
                    y = unityNpcObj.transform.position.y,
                    z = unityNpcObj.transform.position.z,
                };
            }
            else
            {
                // restore position
                unityNpcObj.transform.position = new UnityEngine.Vector3(_workerInfo.position.x, unityNpcObj.transform.position.y, _workerInfo.position.z);
            }

            var aiComp = _mapNpcObj.getComponent("AIActorControllerComp") as AIActorControllerComp;
            if(aiComp == null)
            {
                aiComp = new AIActorControllerComp();
                _mapNpcObj.AddComponent(aiComp);
            }

            _movProdAI = aiComp.GetAIState<AIMoveProduct>();
            if(_movProdAI == null)
            {
                _movProdAI = new AIMoveProduct(_mapNpcObj);
                _movProdAI.Init(new MapConfAIState()
                {
                    aiType = "AIMoveProduct",
                    targetName = null, // set by woker owner
                    aiStateConfName = null, // set by woker owner
                });
                aiComp.AddAIState(_movProdAI);
            }
            _movProdAI.SetWorker(this);

            return true;
        }
    }
}
