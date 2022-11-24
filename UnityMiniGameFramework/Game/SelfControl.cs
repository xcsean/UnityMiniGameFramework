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
        UIMainPanel _uiMainPanel;
        RigibodyMoveAct _movAct;

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

            _uiMainPanel = UnityGameApp.Inst.UI.getUIPanel("MainUI") as UIMainPanel;

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

            _movAct = new RigibodyMoveAct(_selfMapHeroObj);
            _selfMapHeroObj.actionComponent.AddAction(_movAct);

            // add to scene
            unityHeroObj.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);

            if(_selfLocalData.position == null)
            {                
                // set born position
                //unityHeroObj.transform.position = UnityGameApp.Inst.MainScene.implMap.getRandomBornPos();
                unityHeroObj.transform.position = UnityGameApp.Inst.MainScene.implMap.getNamedBornPos("b1");
            }
            else
            {
                // restore position
                unityHeroObj.transform.position = new UnityEngine.Vector3(_selfLocalData.position.x, _selfLocalData.position.y, _selfLocalData.position.z);
            }

            UnityGameApp.Inst.MainScene.camera.follow(_selfMapHeroObj);

            _isInited = true;
        }

        public void OnUpdate()
        {
            if(!_isInited)
            {
                return;
            }

            if(_uiMainPanel.Joystick.isMoving)
            {
                // TO DO : use rigibody mov
                //_movAct.moveTo(_selfMapHeroObj.unityGameObject.transform.position + _uiMainPanel.Joystick.movVector3 * 10.0f);
                _movAct.moveToward(_uiMainPanel.Joystick.movVector3);
            }
            else if(_movAct.isMoving)
            {
                _movAct.stop();
            }
        }
    }
}
