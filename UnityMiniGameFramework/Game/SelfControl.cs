using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class SelfLocalStorageData
    {
        public string mapHeroConfName { get; set; }
        public JsonConfVector3 position { get; set; }
    }

    public class SelfControl
    {
        bool _isInited;
        SelfLocalStorageData _selfLocalData;
        LocalStorageProvider _localStorage;

        MapHeroObject _selfMapHeroObj;
        InputActorControllerComp _inputControl;

        public SelfLocalStorageData selfLocalData => _selfLocalData;
        public MapHeroObject selfMapHero => _selfMapHeroObj;

        public SelfControl()
        {
            _localStorage = new LocalStorageProvider("localStore_Self");
            _isInited = false;
        }

        public void Init()
        {
            // TO DO : implement storage
            //_localStorage.startReadFrom();

            _onLocalStorageReaded();
        }

        protected void _onLocalStorageReaded()
        {
            //_selfLocalData = _localStorage.getUserData<SelfLocalStorageData>();
            _selfLocalData = new SelfLocalStorageData();
            _selfLocalData.mapHeroConfName = "testHero"; // for Debug ...

            _initSelfMapHero();
        }

        protected void _initSelfMapHero()
        {
            var heroConf = UnityGameApp.Inst.MapManager.MapConf.getMapHeroConf(_selfLocalData.mapHeroConfName);

            var unityHeroObj = UnityGameApp.Inst.UnityResource.CreateUnityPrefabObject(heroConf.prefabName);
            var mgObj = unityHeroObj.GetComponent<UnityGameObjectBehaviour>();
            if(mgObj == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"SelfControl init map hero prefab [{heroConf.prefabName}] without UnityGameObjectBehaviour");
                return;
            }

            _selfMapHeroObj = mgObj.mgGameObject as MapHeroObject;
            if (_selfMapHeroObj == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"SelfControl init map hero prefab [{heroConf.prefabName}] not MapHeroObject");
                return;
            }

            _inputControl = new InputActorControllerComp();
            _selfMapHeroObj.AddComponent(_inputControl);
            _inputControl.Init(null); // TO DO : add config

            // add to scene
            unityHeroObj.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);

            if(_selfLocalData.position == null)
            {
                // set born position
                unityHeroObj.transform.position = UnityGameApp.Inst.MainScene.implMap.getRandomBornPos();
            }
            else
            {
                // restore position
                unityHeroObj.transform.position = new UnityEngine.Vector3(_selfLocalData.position.x, _selfLocalData.position.y, _selfLocalData.position.z);
            }

            _isInited = true;
        }

        public void OnUpdate()
        {
            if(!_isInited)
            {
                return;
            }


        }
    }
}
