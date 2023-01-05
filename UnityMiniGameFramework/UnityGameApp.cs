using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UnityGameAppUpdateBehaviour : MonoBehaviour
    {

        protected virtual void Update()
        {
            GameApp.Inst.OnUpdate();
        }

        protected virtual void FixedUpdate()
        {
        }

        protected virtual void LateUpdate()
        {
            UnityGameApp.Inst.OnPostUpdate();
        }

        void OnApplicationFocus(bool hasFocus)
        {
            //isPaused = !hasFocus;
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                UnityGameApp.Inst.OnAppSuspended();
            }
            else
            {
                UnityGameApp.Inst.OnAppResume();
            }
        }
    }

    public class UnityGameAppBehaviour : MonoBehaviour
    {
        public string appConfigFileName;
        public string uiConfigName;
        public string preloaderUIConfName;

        public PanelSettings unityUIPanelSettings;

        static void dbgOutput(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }
        static void dbgError(string msg)
        {
            UnityEngine.Debug.LogError(msg);
        }

        protected virtual void Awake()
        {
            MiniGameFramework.Debug.Init(dbgOutput, dbgError);

            UnityGameApp.setInst(new UnityGameApp());
            initGameAppPlatform();

            UnityGameApp.Inst.unityUIPanelSettings = unityUIPanelSettings;

            GameAPPInitParameter InitParameter = new GameAPPInitParameter
            {
                appConfigFileName = appConfigFileName,
                uiConfigName = uiConfigName,
                preloaderUIConfName = preloaderUIConfName
            };
            UnityGameApp.Inst.Init(InitParameter);
            Application.targetFrameRate = 60;

            Application.quitting += UnityGameApp.Inst.OnAppExit;
        }

        protected virtual void initGameAppPlatform()
        {
            GameApp.Inst.Platform = PlatformEnum.PlatformEditor;
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
            GameApp.Inst.OnUpdate();
        }

        protected virtual void FixedUpdate()
        {
        }

        protected virtual void LateUpdate()
        {
            UnityGameApp.Inst.OnPostUpdate();
        }

        void OnApplicationFocus(bool hasFocus)
        {
            //isPaused = !hasFocus;
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                UnityGameApp.Inst.OnAppSuspended();
            }
            else
            {
                UnityGameApp.Inst.OnAppResume();
            }
        }
    }

    public class UnityGameApp : GameApp
    {
        new public static UnityGameApp Inst => (UnityGameApp)_inst;

        public PanelSettings unityUIPanelSettings;

        public UnityResourceManager UnityResource => (UnityResourceManager)_resManager;
        public Scene MainScene => (Scene)_mainScene;
        public Scene StartScene => (Scene)_startScene;

        protected AnimationManager _aniManager;
        public AnimationManager AniManager => _aniManager;

        protected AudioManager _audManager;
        public AudioManager AudioManager => _audManager;

        protected VFXManager _vfxManager;
        public VFXManager VFXManager => _vfxManager;

        protected CharacterManager _chaManager;
        public CharacterManager CharacterManager => _chaManager;

        protected MapManager _mapManager;
        public MapManager MapManager => _mapManager;

        protected WeaponManager _weaponManager;
        public WeaponManager WeaponManager => _weaponManager;

        protected DataManager _datamanager;
        public DataManager Datas => _datamanager;

        protected AIStateManager _aiStateManager;
        public AIStateManager AIStates => _aiStateManager;

        protected UIInputManager _uiInputManager;
        public UIInputManager UIInputManager => _uiInputManager;

        protected UnityNetworkClient _netClient;
        public UnityNetworkClient NetClient => _netClient;

        protected UnityRESTFulClient _restfulClient;
        public UnityRESTFulClient RESTFulClient => _restfulClient;

        protected Queue<Action> _nextFramePostUpdateCall;

        protected HashSet<Action> _updateCall;

        public void regNextFramePostUpdateCall(Action a)
        {
            _nextFramePostUpdateCall.Enqueue(a);
        }
        public void OnPostUpdate()
        {
            while (_nextFramePostUpdateCall.Count > 0)
            {
                Action a = _nextFramePostUpdateCall.Dequeue();
                a.Invoke();
            }
        }
        public void addUpdateCall(Action a)
        {
            _updateCall.Add(a);
        }
        public void removeUpdateCall(Action a)
        {
            _updateCall.Remove(a);
        }

        override public bool Init(GameAPPInitParameter par)
        {
            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"UnityGameApp start initializing...");

            GameApp.CreatGame = ChickenMasterGame.create;

            _nextFramePostUpdateCall = new Queue<Action>();
            _updateCall = new HashSet<Action>();

            // os implement assignment
            _file = new UnityProjFileSystem();
            _rand = new Randomness();

            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"OS implement initialized.");

            base.Init(par);

            _initNetwork();

            _loadStartScene();

            return true;
        }
        override public void OnUpdate()
        {
            base.OnUpdate();

            _vfxManager.OnUpdate(UnityEngine.Time.deltaTime);
            _weaponManager.OnUpdate(UnityEngine.Time.deltaTime);

            foreach (var a in _updateCall)
            {
                a();
            }
        }


        override public void OnAppSuspended()
        {
            if (_datamanager != null)
            {
                _datamanager.localUserData.writeBack();
            }
        }

        override public void OnAppResume()
        {

        }

        override public void OnAppExit()
        {
            if (_datamanager != null)
            {
                _datamanager.localUserData.writeBack();
            }

        }

        override protected void _createManagers()
        {
            base._createManagers();

            // new managers
            _datamanager = new DataManager();
            _aniManager = new AnimationManager();
            _audManager = new AudioManager();
            _vfxManager = new VFXManager();
            _chaManager = new CharacterManager();
            _sceneManager = new UnitySceneManager();
            _resManager = new UnityResourceManager();
            _mapManager = new MapManager();
            _weaponManager = new WeaponManager();
            _aiStateManager = new AIStateManager();
            _uiInputManager = new UIInputManager();

            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"app managers created.");
        }

        override protected void _regClasses()
        {
            base._regClasses();

            // reg config create
            _conf.regConfigCreator("AnimationConfig", AnimationConfig.create);
            _conf.regConfigCreator("CharacterConfigs", CharacterConfigs.create);
            _conf.regConfigCreator("MapConfig", MapConfig.create);
            _conf.regConfigCreator("NetWorkConfig", NetWorkConfig.create);
            _conf.regConfigCreator("VFXConfig", VFXConfig.create);
            _conf.regConfigCreator("WeaponConfig", WeaponConfig.create);
            _conf.regConfigCreator("CMGameConfig", CMGameConfig.create);

            // reg component
            GameObjectManager.registerGameObjectComponentCreator("ActionComponent", ActionComponent.create);
            GameObjectManager.registerGameObjectComponentCreator("AnimatorComponent", AnimatorComponent.create);
            GameObjectManager.registerGameObjectComponentCreator("AIActorControllerComp", AIActorControllerComp.create);

            // reg object
            GameObjectManager.registerGameObjectCreator("MGGameObject", MGGameObject.create);
            GameObjectManager.registerGameObjectCreator("UnityGameCamera", UnityGameCamera.create);
            GameObjectManager.registerGameObjectCreator("ActorObject", ActorObject.create);
            GameObjectManager.registerGameObjectCreator("Map", Map.create);
            GameObjectManager.registerGameObjectCreator("MapNPCObject", MapNPCObject.create);
            GameObjectManager.registerGameObjectCreator("MapHeroObject", MapHeroObject.create);
            GameObjectManager.registerGameObjectCreator("MapMonsterObject", MapMonsterObject.create);
            GameObjectManager.registerGameObjectCreator("MapBuildingObject", MapBuildingObject.create);
            GameObjectManager.registerGameObjectCreator("MapVehicleObject", MapVehicleObject.create);
            GameObjectManager.registerGameObjectCreator("GunObject", GunObject.create);

            // reg vfx objects
            _vfxManager.registerVfxObjectCreator("VFXObjectBase", VFXObjectBase.create);
            _vfxManager.registerVfxObjectCreator("VFXFootprintObject", VFXFootprintObject.create);
            _vfxManager.registerVfxObjectCreator("VFXLinerObject", VFXLinerObject.create);
            _vfxManager.registerVfxObjectCreator("VFXRangeCricle", VFXRangeCricle.create);

            // reg levels
            _mapManager.registerMapLevelCreator("CMShootingLevel", CMShootingLevel.create);

            // reg ai states
            _aiStateManager.registerAIStateObjectCreator("AITrace", AITrace.create);
            _aiStateManager.registerAIStateObjectCreator("AITryAttack", AITryAttack.create);
            _aiStateManager.registerAIStateObjectCreator("AIMoveOnPath", AIMoveOnPath.create);
            _aiStateManager.registerAIStateObjectCreator("AIMoveProduct", AIMoveProduct.create);

            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"objects registed.");
        }

        override protected void _initPreloaderUI(string preloaderUIConfName)
        {
            base._initPreloaderUI(preloaderUIConfName);

            if (_ui.preloaderPanel != null)
            {
                _ui.preloaderPanel.AddInitStep("initNetwork");
            }
        }

        override protected void _initConfigs(string appConfigFileName)
        {
            // init config
            base._initConfigs(appConfigFileName);

            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"app config initialized.");
        }

        override protected void _initManagers()
        {
            base._initManagers();

            _datamanager.Init();
            _aniManager.Init();
            _chaManager.Init();
            _sceneManager.Init();
            _resManager.Init();
            _mapManager.Init();
            _vfxManager.Init();
            _weaponManager.Init();
        }

        override protected void _initUI(string uiConfigName)
        {
            base._initUI(uiConfigName);

            // reg ui panel creator
            _ui.regUIPanelCreator("preloader", UIPreloaderPanel.create);
            _ui.regUIPanelCreator("UIMainPanel", UIMainPanel.create);
            _ui.regUIPanelCreator("UIPanelStartMain", UIPanelStartMain.create);
            _ui.regUIPanelCreator("UIUpgradePanel", UIUpgradePanel.create);
            _ui.regUIPanelCreator("UILevelEntryPanel", UILevelEntryPanel.create);
            _ui.regUIPanelCreator("UILevelMainPanel", UILevelMainPanel.create);
            _ui.regUIPanelCreator("UIEggPanel", UIEggPanel.create);
            _ui.regUIPanelCreator("UIHeroPanel", UIHeroPanel.create);
            _ui.regUIPanelCreator("UITrainStationPanel", UITrainStationPanel.create);
            _ui.regUIPanelCreator("UIStoreHousePanel", UIStoreHousePanel.create);
            _ui.regUIPanelCreator("UIFlagMainPanel", UIFlagMainPanel.create);
            _ui.regUIPanelCreator("UIFactory1Panel", UIFactory1Panel.create);
            _ui.regUIPanelCreator("UIFactory2Panel", UIFactory2Panel.create);

            _ui.regUIPanelCreator("UICommonFactoryPanel", UICommonFactoryPanel.create);
            _ui.regUIPanelCreator("UIProduceProgressPanel", UIProduceProgressPanel.create);
            _ui.regUIPanelCreator("UIGameMainPanel", UIGameMainPanel.create);
            _ui.regUIPanelCreator("UIOpeningCartoonPanel", UIOpeningCartoonPanel.create);
            _ui.regUIPanelCreator("UITowerHeroPanel", UITowerHeroPanel.create);
            _ui.regUIPanelCreator("UIWeaponAscendPanel", UIWeaponAscendPanel.create);
            _ui.regUIPanelCreator("UIStorehouseCapacityPanel", UIStorehouseCapacityPanel.create);
            
            _ui.regUIPanelCreator("UIDoubleAttackPanel", UIDoubleAttackPanel.create);
            _ui.regUIPanelCreator("UIDoubleExpPanel", UIDoubleExpPanel.create);
            _ui.regUIPanelCreator("UIGetSkillPanel", UIGetSkillPanel.create);
            _ui.regUIPanelCreator("UISkillUpgradePanel", UISkillUpgradePanel.create);
            _ui.regUIPanelCreator("UIPassRewardPanel", UIPassRewardPanel.create);
            _ui.regUIPanelCreator("UIOfflineRewardPanel", UIOfflineRewardPanel.create);

            // reg ui control creator
            _ui.regUIObjectCreator("UIObject", UIObject.create);
            _ui.regUIObjectCreator("UIJoyStickControl", UIJoyStickControl.create);
            _ui.regUIObjectCreator("UILevelStateControl", UILevelStateControl.create);
            _ui.regUIObjectCreator("UIFactoryControl", UIFactoryControl.create);
            _ui.regUIObjectCreator("UIHeroControl", UIHeroControl.create);
        }

        protected void _initNetwork()
        {
            if (_ui.preloaderPanel != null)
            {
                _ui.preloaderPanel.OnInitStep("initNetwork");
            }

            // network client
            NetWorkConfig conf = (NetWorkConfig)_conf.getConfig("network"); // get network config
            if (conf != null)
            {
                _netClient = new UnityNetworkClient();
                _netClient.Init(conf.netConf);

                if (conf.netConf.restfulConf != null)
                {
                    _restfulClient = new UnityRESTFulClient();
                    _restfulClient.Init(conf.netConf.restfulConf.url);
                }

                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"network initialized.");
            }
            else
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"no network.");
            }

        }

        override protected void _onMainSceneLoaded()
        {
            _uiInputManager.Init();
        }
    }
}
