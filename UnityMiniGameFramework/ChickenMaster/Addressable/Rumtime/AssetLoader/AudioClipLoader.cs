using System;
using UnityEngine;

namespace UnityMiniGameFramework.Addressable
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioClipLoader : AssetLoader<AudioClip>
    {
        [SerializeField]
        private bool m_PlayOnAwake = true;

        [NonSerialized] private AudioSource m_Source;

        public bool PlayOnAwake
        {
            get => m_PlayOnAwake;
            set => m_PlayOnAwake = value;
        }

        public AudioSource Source
        {
            get
            {
                if (!m_Source)
                    m_Source = GetComponent<AudioSource>();
                return m_Source;
            }
        }
        protected override void OnComplete(AudioClip result)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            Source.clip = result;
            if (m_PlayOnAwake)
                Source.Play();
        }
    }
}