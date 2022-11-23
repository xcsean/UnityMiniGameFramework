using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public struct GameAPPInitParameter
    {
        public string appConfigFileName;
        public string uiConfigName;
        public string preloaderUIConfName;
    }

    public enum GameAppInitStep
    {
        InitConfig = 1,
        InitManager,
        InitUI,
        LoadStartScene,
        EnterStartScene,
        LoadMainScene,
        EnterMainScene
    }

    public class GameApp
    {
        protected static GameApp _inst;
        public static GameApp Inst => _inst;

        public static void setInst(GameApp inst)
        {
            _inst = inst;
        }

        protected INetwork _net;
        public INetwork Net => _net;

        protected IFileSystem _file;
        public IFileSystem File => _file;

        protected IRandom _rand;
        public IRandom Rand => _rand;

        protected ConfigManager _conf;
        public ConfigManager Conf => _conf;

        protected UIManager _ui;
        public UIManager UI => _ui;

        protected SceneManager _sceneManager;
        public SceneManager SceneManager => _sceneManager;

        protected ResourceManager _resManager;
        public ResourceManager Resource => _resManager;

        protected GameAppInitStep _initStep;
        public GameAppInitStep currInitStep => _initStep;

        protected IScene _startScene;
        protected IScene _mainScene;

        virtual public bool Init(GameAPPInitParameter par)
        {
            // TO DO : change init step to async

            _initStep = GameAppInitStep.InitConfig;

            _createManagers();

            _regClasses();

            _initPreloaderUI(par.preloaderUIConfName);

            if(par.appConfigFileName == "" || par.appConfigFileName == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_System, "App init without app config");
            }
            else
            {
                _initConfigs(par.appConfigFileName);
            }

            _initStep = GameAppInitStep.InitManager;

            _initManagers();


            _initStep = GameAppInitStep.InitUI;

            if (par.uiConfigName == "" || par.uiConfigName == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_System, "App init without ui config");
            }
            else
            {
                _initUI(par.uiConfigName);
            }

            return true;
        }

        virtual public void OnUpdate()
        {
            _sceneManager.OnUpdate();

            if (_initStep == GameAppInitStep.LoadStartScene)
            {
                if(_startScene.loadStatus.done)
                {
                    _onStartSceneLoaded();

                    _initStep = GameAppInitStep.EnterStartScene;
                    //_sceneManager.changeScene(_startScene); // auto change
                }
            }
            else if (_initStep == GameAppInitStep.LoadMainScene)
            {
                if (_mainScene.loadStatus.done)
                {
                    _onMainSceneLoaded();

                    _initStep = GameAppInitStep.EnterMainScene;
                    //_sceneManager.changeScene(_mainScene); // auto change

                    // unload start scene
                    _startScene.UnloadAsync();
                }
            }
        }

        virtual protected void _onStartSceneLoaded()
        {

        }
        virtual protected void _onMainSceneLoaded()
        {

        }

        virtual protected void _createManagers()
        {
            _net = new Network();
            _conf = new ConfigManager();
            _ui = new UIManager();
        }

        virtual protected void _regClasses()
        {
            _conf.regConfigCreator("SceneManagerConfig", SceneManagerConfig.create);

            GameObjectManager.registerGameObjectComponentCreator("StateComponent", StateComponent.create);

        }

        virtual protected void _initPreloaderUI(string preloaderUIConfName)
        {
            if(preloaderUIConfName == "" || preloaderUIConfName == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_System, "App init without preloader config");
                return;
            }

            UIPanelConfig panelConf = new UIPanelConfig();
            panelConf.Init(preloaderUIConfName, "preloader");
            
            _ui.createUIPanelByConf(panelConf.uiPanelConf);

            if (_ui.preloaderPanel != null)
            {
                _ui.preloaderPanel.AddInitStep("initConfig");
                _ui.preloaderPanel.AddInitStep("initManager");
                _ui.preloaderPanel.AddInitStep("initUI");
                _ui.preloaderPanel.AddInitStep("loadStartScene");
            }
        }

        virtual protected void _initConfigs(string appConfigFileName)
        {
            if (_ui.preloaderPanel != null)
            {
                _ui.preloaderPanel.OnInitStep("initConfig");
            }

            _conf.InitAppConfig(appConfigFileName);
        }

        virtual protected void _initManagers()
        {
            if (_ui.preloaderPanel != null)
            {
                _ui.preloaderPanel.OnInitStep("initManager");
            }
        }

        virtual protected void _initUI(string uiConfigName)
        {
            if (_ui.preloaderPanel != null)
            {
                _ui.preloaderPanel.OnInitStep("initUI");
            }

            _ui.Init(uiConfigName);
        }

        virtual protected void _loadStartScene()
        {
            _initStep = GameAppInitStep.LoadStartScene;

            if (_ui.preloaderPanel != null)
            {
                _ui.preloaderPanel.OnInitStep("loadStartScene");
            }

            if (_sceneManager == null)
            {
                _initStep = GameAppInitStep.EnterStartScene;
                return;
            }

            _startScene = _sceneManager.createStartScene();
            _startScene.LoadAsync();
        }

        virtual public void LoadMainScene()
        {
            _initStep = GameAppInitStep.LoadMainScene;

            if (_sceneManager == null)
            {
                _initStep = GameAppInitStep.EnterMainScene;
                return;
            }

            _mainScene = _sceneManager.createMainScene();
            _mainScene.LoadAsync();
        }
    }
}
