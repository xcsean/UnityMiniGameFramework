using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace UnityMiniGameFramework
{
    public class Animation
    {
        protected AnimationClip _aniClip;
        
        public AnimationClip aniClip => _aniClip;

        protected string _aniClipName;
        public string aniClipName => _aniClipName;

        protected string _aniSlotName;
        public string aniSlotName => _aniSlotName;
        protected string _aniName;
        public string aniName => _aniName;

        protected int _slotIndex;
        public int slotIndex => _slotIndex;

        public Animation(AniConf conf, AnimationClip clip, int slot)
        {
            _aniClip = clip;
            _aniName = conf.name;
            _aniClipName = conf.aniFile;
            _aniSlotName = conf.aniSlot;
            if (_aniSlotName == null)
            {
                _aniSlotName = "";
            }
            _slotIndex = slot;
        }
    }
}
