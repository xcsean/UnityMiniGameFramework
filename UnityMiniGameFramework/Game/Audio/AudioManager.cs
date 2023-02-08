using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniGameFramework;
using UnityEngine;
using Debug = MiniGameFramework.Debug;
using GameObject = UnityEngine.GameObject;

namespace UnityMiniGameFramework
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance => _instance;
        private static AudioManager _instance;
        [SerializeField] private AudioSource m_bgmSource;
        [SerializeField] private GameObject m_audioObjOrigin;

        private List<AudioSource> m_activeAduioSources = new List<AudioSource>(20);
        private Stack<AudioSource> m_sfxSourcePool = new Stack<AudioSource>(30);

        private Dictionary<string, AudioClip> m_CachedAudioAssets = new Dictionary<string, AudioClip>();
        private string m_lastBGMName;
        private AudionConfig _config;

        public void Init()
        {
            _config = (AudionConfig) UnityGameApp.Inst.Conf.getConfig("audios");
            if (_config == null)
                Debug.DebugOutput(DebugTraceType.DTT_Error, "audiosConfig Init Fail");
        }

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            if (m_bgmSource == null)
                m_bgmSource = gameObject.AddComponent<AudioSource>();
            m_bgmSource.loop = true;
        }

        private void OnDestroy()
        {
            ReleaseAll();
        }

        private void ReleaseAll()
        {
            foreach (var clip in m_CachedAudioAssets.Values)
                Resources.UnloadAsset(clip);
            m_CachedAudioAssets.Clear();
        }

        public void PauseAll()
        {
            if (m_bgmSource.isPlaying)
            {
                m_bgmSource.Pause();
            }
            for (int i = 0; i < m_activeAduioSources.Count; i++)
            {
                m_activeAduioSources[i].Stop();
            }
        }

        public void ResumeAll()
        {
            m_bgmSource.UnPause();

        }

        public void PlayBGM(string clipName)
        {
            if (string.IsNullOrEmpty(clipName))
                return;
            if (m_bgmSource.isPlaying && m_lastBGMName == clipName)
                return;
            AudioClip clip;
            if (m_CachedAudioAssets.TryGetValue(clipName, out clip))
            {
            }
            else
            {
                clip = ((UnityResourceManager) UnityGameApp.Inst.Resource).LoadAudioClip(clipName);
                m_CachedAudioAssets.Add(clipName, clip);
            }

            m_bgmSource.clip = clip;
            m_bgmSource.Play();
            m_lastBGMName = clipName;
        }

        public void PlaySFXByAudioName(string audioName)
        {
            var config = _config.getAudioConfig(audioName);
            if (config == null)
                return;
            //PlaySFX(config.SrcPath);
            PlaySFX(config);
        }

        private void PlaySFX(AudionConf config)
        {
            if (config == null)
                return;
            AudioClip clip;
            string clipName = config.SrcPath;
            if (m_CachedAudioAssets.TryGetValue(clipName, out clip))
            {
            }
            else
            {
                clip = ((UnityResourceManager) UnityGameApp.Inst.Resource).LoadAudioClip(clipName);
                m_CachedAudioAssets.Add(clipName, clip);
            }

            AudioSource source = GetOneAudioSource();
            m_activeAduioSources.Add(source);
            source.clip = clip;
            if (!config.SpatialBlend.HasValue || config.SpatialBlend.Equals(0))
            {
                source.volume = 1;
                source.spatialBlend = 0;
                source.gameObject.transform.position = Vector3.zero;
                source.Play();
                return;
            }

            source.spatialBlend = (float) config.SpatialBlend;
            source.volume = config.Volume ?? 1;
            source.maxDistance = config.MaxDistance ?? 500;
            source.rolloffMode = AudioRolloffMode.Custom;
            string mapNode = config.MapRoot;
            string root = mapNode.Substring(0, mapNode.IndexOf('/'));
            var go = GameObject.Find(root);
            if (go != null)
            {
                string nodePath = mapNode.Substring(mapNode.IndexOf('/') + 1, mapNode.Length - root.Length - 1);
                var node = go.transform.Find(nodePath);
                if (node != null)
                {
                    source.gameObject.transform.position = node.transform.position;
                }
            }
            source.Play();
        }

        public void PlaySFX(string clipName, bool looping = false)
        {
            if (string.IsNullOrEmpty(clipName))
                return;
            AudioClip clip;
            if (m_CachedAudioAssets.TryGetValue(clipName, out clip))
            {
            }
            else
            {
                clip = ((UnityResourceManager) UnityGameApp.Inst.Resource).LoadAudioClip(clipName);
                m_CachedAudioAssets.Add(clipName, clip);
            }

            AudioSource source = GetOneAudioSource();
            m_activeAduioSources.Add(source);
            source.gameObject.transform.position = Vector3.zero;
            source.volume = 1;
            source.spatialBlend = 0;
            source.clip = clip;
            source.loop = looping;
            source.Play();
        }

        private AudioSource GetOneAudioSource()
        {
            return m_sfxSourcePool.Count > 0 ? m_sfxSourcePool.Pop() : AddNewAudioSource();
        }

        private AudioSource AddNewAudioSource()
        {
            var go = GameObject.Instantiate(m_audioObjOrigin, transform);
            return go.GetComponent<AudioSource>();
        }

        private void Update()
        {
            int i = 0;
            while (i < m_activeAduioSources.Count)
            {
                var source = m_activeAduioSources[i];
                if (CanAudioSourceBeRecycled(source))
                {
                    source.Stop();
                    m_activeAduioSources.Remove(source);
                    m_sfxSourcePool.Push(source);
                }
                else
                    i++;
            }
        }

        private bool CanAudioSourceBeRecycled(AudioSource audioSource)
        {
            return audioSource.clip == null || !audioSource.isPlaying ||
                   (!audioSource.loop && audioSource.time >= audioSource.clip.length);
        }
    }
}