using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public class SceneConf
    {
        public string name { get; set; }

        public string sceneFileName { get; set; }
        public string mainUIPanelName { get; set; }
        public string mapName { get; set; }

        public string rootObjectName { get; set; }
        public string mainCameraName { get; set; }
        public string uiRootName { get; set; }
        public string mapRootName { get; set; }

        public bool? changeOnLoaded { get; set; }

        public bool? isAdditive { get; set; }
    }

    public class SceneManagerConf
    {
        public Dictionary<string, SceneConf> sceneConfs { get; set; }

        public string startScene { get; set; }
        public string mainScene { get; set; }
    }

    public class SceneManagerConfig : JsonConfig
    {
        override public string type => "SceneManagerConfig";
        public static SceneManagerConfig create()
        {
            return new SceneManagerConfig();
        }

        public SceneManagerConf sceneManagerConf => (SceneManagerConf)_conf;

        override protected object _JsonDeserialize(string confStr)
        {
            return JsonSerializer.Deserialize<SceneManagerConf>(confStr);
        }

        public SceneConf getStartSceneConf()
        {
            if(sceneManagerConf == null)
            {
                return null;
            }

            if(sceneManagerConf.sceneConfs.ContainsKey(sceneManagerConf.startScene))
            {
                return sceneManagerConf.sceneConfs[sceneManagerConf.startScene];
            }

            return null;
        }

        public SceneConf getMainSceneConf()
        {
            if (sceneManagerConf == null)
            {
                return null;
            }

            if (sceneManagerConf.sceneConfs.ContainsKey(sceneManagerConf.mainScene))
            {
                return sceneManagerConf.sceneConfs[sceneManagerConf.mainScene];
            }

            return null;
        }

        public SceneConf getSceneConf(string sceneName)
        {
            if (sceneManagerConf == null)
            {
                return null;
            }

            if (sceneManagerConf.sceneConfs.ContainsKey(sceneName))
            {
                return sceneManagerConf.sceneConfs[sceneName];
            }

            return null;
        }
    }
}
