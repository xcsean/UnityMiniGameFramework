using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;
using UnityEngine;

namespace UnityMiniGameFramework
{
    public class UnityGameAppBehaviour : MonoBehaviour
    {
        public string appConfigFileName;
        public string uiConfigName;
        public string preloaderUIConfName;

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

            GameApp.setInst(new UnityGameApp());

            GameAPPInitParameter InitParameter = new GameAPPInitParameter
            {
                appConfigFileName = appConfigFileName,
                uiConfigName = uiConfigName,
                preloaderUIConfName = preloaderUIConfName
            };
            GameApp.Inst.Init(InitParameter);
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
        }
    }

    public class UnityGameApp : GameApp
    {
        new public static UnityGameApp Inst => (UnityGameApp)_inst;

        protected AnimationManager _aniManager;
        public AnimationManager AniManager => _aniManager;

        protected AudioManager _audManager;
        public AudioManager AudioManager => _audManager;

        protected VFXManager _vfxManager;
        public VFXManager VFXManager => _vfxManager;

        protected CharacterManager _chaManager;
        public CharacterManager CharacterManager => _chaManager;

        protected UnityNetworkClient _netClient;
        public UnityNetworkClient NetClient => _netClient;

        override public bool Init(GameAPPInitParameter par)
        {
            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"UnityGameApp start initializing...");
            
            // os implement assignment
            _file = new UnityProjFileSystem();

            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"OS implement initialized.");

            base.Init(par);

            _initNetwork();

            _loadStartScene();

            return true;
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

            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"app managers created.");
        }

        override protected void _regClasses()
        {
            base._regClasses();
            
            // reg config create
            _conf.regConfigCreator("CharacterConfigs", CharacterConfigs.create);
            _conf.regConfigCreator("NetWorkConfig", NetWorkConfig.create);

            // reg component
            GameObjectManager.registerGameObjectComponentCreator("ActionComponent", ActionComponent.create);
            GameObjectManager.registerGameObjectComponentCreator("AnimatorComponent", AnimatorComponent.create);
            GameObjectManager.registerGameObjectComponentCreator("AudioComponent", AudioComponent.create);
            GameObjectManager.registerGameObjectComponentCreator("VFXComponent", VFXComponent.create);

            // reg object
            GameObjectManager.registerGameObjectCreator("MGGameObject", MGGameObject.create);
            GameObjectManager.registerGameObjectCreator("ActorObject", ActorObject.create);
            GameObjectManager.registerGameObjectCreator("CharacterObject", CharacterObject.create);

            // reg ui class
            _ui.regUIPanelCreator("UIPanelStartMain", UIPanelStartMain.create);


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

            //_aniManager.Init();
            _chaManager.Init();
            _sceneManager.Init();
        }

        override protected void _initUI(string uiConfigName)
        {
            base._initUI(uiConfigName);

            // reg ui panel creator
            _ui.regUIPanelCreator("preloader", UIPreloaderPanel.create);
            _ui.regUIPanelCreator("main", UIMainPanel.create);
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
    }
}
