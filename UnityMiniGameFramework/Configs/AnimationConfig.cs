using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Text.Json;
using System.Threading.Tasks;

using MiniGameFramework;
using UnityEngine;

namespace UnityMiniGameFramework
{
    public class AniEventConf
    {
        public string name { get; set; }
        public float length { get; set; }
    }

    public class AniConf
    {
        public string name { get; set; }
        public string aniFile { get; set; }
        public string aniSlot { get; set; }

        public List<AniEventConf> evetns { get; set; }
        // TO DO : add configs
    }

    public class AnimatorConf
    {
        public string SkeltonRootName { get; set; }
        public Dictionary<string, AniConf> Animations { get; set; }

        // TO DO : add configs
    }

    [Serializable]
    public class AnimationsConf
    {
        public Dictionary<string, AnimatorConf> Animators { get; set; }
    }

    public class AnimationConfig : JsonConfig
    {
        override public string type => "AnimationConfig";
        public static AnimationConfig create()
        {
            return new AnimationConfig();
        }
        public AnimationsConf aniConf => (AnimationsConf)_conf;

        override protected object _JsonDeserialize(string confStr)
        {
            //return JsonSerializer.Deserialize<AnimationsConf>(confStr);
            return JsonUtil.FromJson<AnimationsConf>(confStr);
        }


        public AnimatorConf getAnimatorConf(string animatorName)
        {
            if (aniConf.Animators == null || !aniConf.Animators.ContainsKey(animatorName))
            {
                return null;
            }
            return aniConf.Animators[animatorName];
        }
    }
}
