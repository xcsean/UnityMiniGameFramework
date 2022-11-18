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

        protected ConfigManager _conf;
        public ConfigManager Conf => _conf;

        protected UIManager _ui;
        public UIManager UI => _ui;

        protected SceneManager _scene;
        public SceneManager Scene => _scene;

        virtual public bool Init(GameAPPInitParameter par)
        {
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

        virtual protected void _createManagers()
        {
            _net = new Network();
            _conf = new ConfigManager();
            _ui = new UIManager();
        }

        virtual protected void _regClasses()
        {
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
                _ui.preloaderPanel.AddInitStep("initUI");
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

        virtual protected void _initUI(string uiConfigName)
        {
            if (_ui.preloaderPanel != null)
            {
                _ui.preloaderPanel.OnInitStep("initUI");
            }

            _ui.Init(uiConfigName);
        }
    }
}
