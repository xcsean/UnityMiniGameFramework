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

            UnityGameApp.Inst.unityUIPanelSettings = unityUIPanelSettings;

            GameAPPInitParameter InitParameter = new GameAPPInitParameter
            {
                appConfigFileName = appConfigFileName,
                uiConfigName = uiConfigName,
                preloaderUIConfName = preloaderUIConfName
            };
            UnityGameApp.Inst.Init(InitParameter);
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
    }

    public class UnityGameApp : GameApp
    {
        new public static UnityGameApp Inst => (UnityGameApp)_inst;

        public PanelSettings unityUIPanelSettings;

        public UnityResourceManager UnityResource => (UnityResourceManager)_resManager;
        public Scene MainScene => (Scene)_mainScene;

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

        protected UnityNetworkClient _netClient;
        public UnityNetworkClient NetClient => _netClient;

        protected SelfControl _self;
        public SelfControl Self => _self;

        protected Queue<Action> _nextFramePostUpdateCall;

        public void regNextFramePostUpdateCall(Action a)
        {
            _nextFramePostUpdateCall.Enqueue(a);
        }
        public void OnPostUpdate()
        {
            while(_nextFramePostUpdateCall.Count > 0)
            {
                Action a = _nextFramePostUpdateCall.Dequeue();
                a.Invoke();
            }
        }

        override public bool Init(GameAPPInitParameter par)
        {
            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"UnityGameApp start initializing...");

            _nextFramePostUpdateCall = new Queue<Action>();

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

            _self.OnUpdate();
        }
        override protected void _onMainSceneLoaded()
        {
            base._onMainSceneLoaded();

            _initSelfControl();
        }

        override protected void _createManagers()
        {
            base._createManagers();

            // new managers
            _aniManager = new AnimationManager();
            _audManager = new AudioManager();
            _vfxManager = new VFXManager();
            _chaManager = new CharacterManager();
            _sceneManager = new UnitySceneManager();
            _resManager = new UnityResourceManager();
            _mapManager = new MapManager();
            _weaponManager = new WeaponManager();

            _self = new SelfControl();

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

            // reg component
            GameObjectManager.registerGameObjectComponentCreator("ActionComponent", ActionComponent.create);
            GameObjectManager.registerGameObjectComponentCreator("AnimatorComponent", AnimatorComponent.create);

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

            // reg ui control creator
            _ui.regUIObjectCreator("UIObject", UIObject.create);
            _ui.regUIObjectCreator("UIJoyStickControl", UIJoyStickControl.create);
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

                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"network initialized.");
            }
            else
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"no network.");
            }
        }

        protected void _initSelfControl()
        {
            _self.Init();

        }
    }
}
