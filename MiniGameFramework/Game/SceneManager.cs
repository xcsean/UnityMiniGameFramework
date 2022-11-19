using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    abstract public class SceneManager
    {
        protected IScene _currentScene;
        public IScene currentScene => _currentScene;

        protected SceneManagerConfig _conf;
        public SceneManagerConfig config => _conf;

        protected Dictionary<string, IScene> _scenes;

        public SceneManager()
        {
            _scenes = new Dictionary<string, IScene>();
        }

        virtual public void Init()
        {
            _conf = (SceneManagerConfig)GameApp.Inst.Conf.getConfig("scenes");
            if(_conf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"SceneManager init without scenes config");
            }
        }

        virtual public IScene getScene(string sceneName)
        {
            if(_scenes.ContainsKey(sceneName))
            {
                return _scenes[sceneName];
            }

            return null;
        }

        virtual public IScene createScene(string sceneName)
        {
            var sceneConf = _conf.getSceneConf(sceneName);
            if (sceneConf == null)
            {
                return null;
            }

            IScene newScene = _createScene(sceneConf);

            _scenes[newScene.name] = newScene;

            return newScene;
        }

        virtual public IScene createStartScene()
        {
            var startSceneConf = _conf.getStartSceneConf();
            if (startSceneConf == null)
            {
                return null;
            }

            IScene newScene = _createScene(startSceneConf);

            _scenes[newScene.name] = newScene;

            return newScene;
        }

        virtual protected IScene _createScene(SceneConf conf)
        {
            throw new NotImplementedException();
        }

        virtual public void changeScene(IScene s)
        {
            if(_currentScene != null)
            {
                _currentScene.OnHide();
            }

            _currentScene = s;
            _currentScene.OnShow();
        }

        virtual public void onDisposeScene(IScene scene)
        {
            if(_scenes.ContainsKey(scene.name))
            {
                _scenes.Remove(scene.name);
            }
        }

        virtual public List<string> gatherSceneDependFiles(string sceneName)
        {
            throw new NotImplementedException();
        }
    }
}
