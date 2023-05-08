using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class AnimationManager
    {
        protected AnimationConfig _aniConf;
        public AnimationConfig AnimationConfs => _aniConf;

        private BakeClipConfig _clipConfig;

        public BakeClipConfig ClipConfig
        {
            get => _clipConfig;
        }

        public void Init()
        {
            _aniConf = (AnimationConfig) UnityGameApp.Inst.Conf.getConfig("animations");
            _clipConfig = (BakeClipConfig) UnityGameApp.Inst.Conf.getConfig("bakeClips");
        }
    }
}