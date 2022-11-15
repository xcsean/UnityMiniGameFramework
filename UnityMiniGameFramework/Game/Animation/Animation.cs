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

        public string aniSlotName => "";
        public string aniName => "";
    }
}
