using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace UnityMiniGameFramework.Addressable
{
    [ExecuteAlways]
    public abstract class AssetLoader<T> : MonoBehaviour, IAssetLoader where T : Object
    {
        [SerializeField] private AssetReferenceT<T> m_Reference;
        [SerializeField] public bool isNeedReset = false;

        private object m_CacheKey;
        private object m_CurrentKey;
        private AsyncOperationHandle<T> m_Handle;
        private bool m_IsDone = true;


        public virtual AssetReferenceT<T> Reference
        {
            get => m_Reference;
            set => m_Reference = value;
        }

        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                OnComplete(Reference?.editorAsset);
            else
#endif
            LoadAsync(m_CacheKey ?? Reference);
        }

        public bool IsValid() => isActiveAndEnabled && m_CacheKey != null && IsValid(m_CacheKey);

        private static bool IsValid(object key)
        {
            return key switch
            {
                string keyStr => !string.IsNullOrEmpty(keyStr),
                IKeyEvaluator {RuntimeKey: string target} => !string.IsNullOrEmpty(target),
                _ => key != null
            };
        }

        public void LoadAsync(object key)
        {
            m_CacheKey = key;
            if (!IsValid())
            {
                OnUnloaded();
                return;
            }

            if (!m_IsDone || m_CacheKey == m_CurrentKey)
                return;
            m_IsDone = false;
            m_CurrentKey = m_CacheKey;
            OnLoadBegin();
            Addressables.LoadAssetAsync<T>(m_CurrentKey).Completed += OnLoadComplete;
        }

        private void OnLoadComplete(AsyncOperationHandle<T> handle)
        {
            m_IsDone = true;
            Unload();
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                m_Handle = handle;
                if (this && isActiveAndEnabled)
                    OnComplete(handle.Result);
                else
                    Unload();
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} 上资源加载失败，请检查key或者prefab的设置");
                OnFailed();
            }

            if (!(this && isActiveAndEnabled))
                return;
            LoadAsync(m_CacheKey);
        }

        protected abstract void OnComplete(T result);

        protected virtual void OnLoadBegin()
        {
        }

        protected virtual void OnFailed()
        {
        }

        private void Unload()
        {
            if (!m_Handle.IsValid() || !m_Handle.IsDone)
                return;
            Addressables.Release(m_Handle);
            m_Handle = default;
            if (!this) return;
            if (!isActiveAndEnabled)
                m_CurrentKey = null;
            OnUnloaded();
        }

        protected virtual void OnUnloaded()
        {
        }

        protected virtual void OnDisable()
        {
            Unload();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            OnEnable();
        }
#endif
    }
}