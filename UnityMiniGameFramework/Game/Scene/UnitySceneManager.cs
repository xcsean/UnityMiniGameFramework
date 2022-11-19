using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityMiniGameFramework
{
    public class UnitySceneManager : MiniGameFramework.SceneManager
    {
        protected UnityEngine.SceneManagement.Scene _unityBaseScene;
        public UnityEngine.SceneManagement.Scene unityBaseScene => _unityBaseScene;

        override public void Init()
        {
            base.Init();

            _unityBaseScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        }

        override protected IScene _createScene(SceneConf conf)
        {
            return new Scene(conf);
        }

        override public List<string> gatherSceneDependFiles(string sceneName)
        {
            throw new NotImplementedException();
        }
    }
}
