using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace UnityMiniGameFramework
{
    public class Audio
    {
        protected AudioClip _audioClip;
        
        public AudioClip aniClip => _audioClip;
        
        public string audioName => "";
    }
}
