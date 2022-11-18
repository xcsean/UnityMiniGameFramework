using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public class SceneManager
    {
        protected IScene _currentScene;
        public IScene currentScene => _currentScene;

        public void Init()
        {

        }

        public IScene getScene(string sceneName)
        {
            return null;
        }

        public IScene createScene(string sceneName)
        {
            return null;
        }

        public void changeScene(IScene s)
        {

        }

        public void disposeScene(IScene scene)
        {

        }

        public List<string> gatherSceneDependFiles(string sceneName)
        {
            //if (!_uiConf.uiConf.UIPanels.ContainsKey(panelName))
            //{
            //    Debug.DebugOutput(DebugTraceType.DTT_Error, $"gatherUIPanelDependFiles ({panelName}) config not exist");
            //    return null;
            //}

            //List<string> listFiles = new List<string>();
            //listFiles.Add(_uiConf.uiConf.UIPanels[panelName]); // panel config file

            // TO DO : add other files, textures, etc.

            //return listFiles;

            return null;
        }
    }
}
