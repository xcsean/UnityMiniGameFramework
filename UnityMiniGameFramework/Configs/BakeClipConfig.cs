using System;
using System.Collections.Generic;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class ClipConf
    {
        public float BeginFrame;
        public float EndFrame;
        public bool IsLoop;
    }

    public class BakeClipConf
    {
        public Dictionary<string, ClipConf> clips;
        public int FrameRate;
    }


    [Serializable]
    public class BakeClipConfigs
    {
        public Dictionary<string, BakeClipConf> bakeAnimationClip { get; set; }
    }

    public class BakeClipConfig : JsonConfig
    {
        public override string type => "BakeClipConfig";

        public static BakeClipConfig create()
        {
            return new BakeClipConfig();
        }

        public BakeClipConfigs bakeClipConfs => (BakeClipConfigs) _conf;

        protected override object _JsonDeserialize(string confStr)
        {
            return JsonUtil.FromJson<BakeClipConfigs>(confStr);
        }

        public BakeClipConf getBakeClipConf(string clipName)
        {
            if (bakeClipConfs.bakeAnimationClip == null || !bakeClipConfs.bakeAnimationClip.ContainsKey(clipName))
                return null;
            return bakeClipConfs.bakeAnimationClip[clipName];
        }
    }
}