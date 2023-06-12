using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Profiling;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;


namespace UnityMiniGameFramework.Addressable
{
    [Serializable]
    public struct PooledInstanceProviderSettings
    {
        public float releaseTime;

        public PooledInstanceProviderSettings(float releaseTime = 10)
        {
            this.releaseTime = releaseTime;
        }
    }

    public interface IPooledActive
    {
        int priority { get; }
        void OnActive();
        void OnInactive();
    }

    public interface IPooledInstanceActive : IPooledActive
    {
        void OnActiveChanged(bool active);
        Action<GameObject> OnDestroy { set; }
    }

    public class PooledInstanceProvider : IInstanceProvider, IInitializableObject
    {
        private float m_ReleaseTime = 10;
        private PooledInstanceProviderBehaviour m_Behaviour;

        private List<Transform> m_CachedChildren = new List<Transform>();

        private Dictionary<GameObject, AsyncOperationHandle<GameObject>> m_InstanceObjectToPrefabHandle =
            new Dictionary<GameObject, AsyncOperationHandle<GameObject>>();

        private Dictionary<AsyncOperationHandle<GameObject>, InstancePool> m_InstanceObjectPools =
            new Dictionary<AsyncOperationHandle<GameObject>, InstancePool>();

        class InstanceInitAsyncOp : AsyncOperationBase<bool>
        {
            private Func<bool> m_CallBack;

            public void Init(Func<bool> callback)
            {
                m_CallBack = callback;
            }

            protected override void Execute()
            {
                if (m_CallBack != null)
                    Complete(m_CallBack(), true, "");
                else
                    Complete(true, true, "");
            }
        }

        public bool Initialize(string id, string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    var settings = JsonUtility.FromJson<PooledInstanceProviderSettings>(data);
                    m_ReleaseTime = settings.releaseTime;
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("Cannot get release time from json: {0}.", data);
                }
            }

            m_Behaviour = new GameObject(nameof(PooledInstanceProviderBehaviour))
                .AddComponent<PooledInstanceProviderBehaviour>();
            m_Behaviour.Init(this);
            return true;
        }

        public AsyncOperationHandle<bool> InitializeAsync(ResourceManager rm, string id, string data)
        {
            var initOp = new InstanceInitAsyncOp();
            initOp.Init(() => Initialize(id, data));
            return rm.StartOperation(initOp, default);
        }

        public GameObject ProvideInstance(ResourceManager resourceManager,
            AsyncOperationHandle<GameObject> prefabHandle,
            InstantiationParameters instantiateParameters)
        {
            Profiler.BeginSample("PooledInstanceProvider.ProvideInstance()");
            if (!m_InstanceObjectPools.TryGetValue(prefabHandle, out var pool))
                m_InstanceObjectPools.Add(prefabHandle,
                    pool = new InstancePool(Addressables.ResourceManager, prefabHandle, m_Behaviour.transform));

            var result = pool.ProvideInstance(instantiateParameters);
            m_InstanceObjectToPrefabHandle.Add(result, prefabHandle);
            Profiler.EndSample();
            return result;
        }

        public void ReleaseInstance(ResourceManager resourceManager, GameObject instance)
        {
            Profiler.BeginSample("PooledInstanceProvider.ReleaseInstance()");
            if (instance != null)
            {
                instance.GetComponentsInChildren(true, m_CachedChildren);
                for (var i = m_CachedChildren.Count - 1; i >= 0; i--)
                {
                    var target = m_CachedChildren[i].gameObject;
                    if (m_InstanceObjectToPrefabHandle.TryGetValue(target, out var handle))
                    {
                        m_InstanceObjectToPrefabHandle.Remove(target);
                        if (m_InstanceObjectPools.TryGetValue(handle, out var pool))
                        {
                            m_CachedChildren.RemoveAt(i);
                            pool.ReleaseInstance(target);
                        }
                        else resourceManager.Release(handle);
                    }
                }

                if (m_CachedChildren.Contains(instance.transform))
                {
                    Debug.LogWarningFormat("Releasing unknown GameObject {0} to PooledInstanceProvider.", instance);
                    Destroy(instance);
                }
            }
            else if (m_InstanceObjectToPrefabHandle.TryGetValue(instance, out var handle))
            {
                m_InstanceObjectToPrefabHandle.Remove(instance);
                if (m_InstanceObjectPools.TryGetValue(handle, out var pool))
                    pool.ReleaseInstance(instance);
                else
                    resourceManager.Release(handle);
            }
            else
                Debug.LogWarningFormat("Releasing unknown GameObject {0} to PooledInstanceProvider.", instance);

            Profiler.EndSample();
        }

        private static void Destroy(GameObject instance)
        {
            if (instance == null)
                return;
            if (Application.isPlaying)
                Object.Destroy(instance);
            else
                Object.DestroyImmediate(instance);
        }

        internal void Update(bool flush = false)
        {
            var releaseTime = flush ? -1 : m_ReleaseTime;
            foreach (var pool in m_InstanceObjectPools.Values)
                pool.Update(releaseTime);
        }

        class InstancePool
        {
            struct GameObjectInstance
            {
                public GameObject instance;
                public bool isDontDestroy;
            }

            private static readonly List<IPooledInstanceActive> s_CacheActives = new List<IPooledInstanceActive>();

            private static readonly Comparison<IPooledInstanceActive> s_ActiveComparison =
                (active1, active2) => active2.priority.CompareTo(active1.priority);

            private readonly ResourceManager m_ResourceManager;
            private readonly AsyncOperationHandle<GameObject> m_PrefabHandle;
            private readonly Transform m_Transform;

            private readonly HashSet<GameObject> m_InstanceHashes = new HashSet<GameObject>();
            private readonly Stack<GameObjectInstance> m_Instances = new Stack<GameObjectInstance>();
            private float m_LastRefTime = Time.unscaledTime;
            private float m_LastReleaseTime;

            private Action<GameObject> m_OnDestroyAction;

            private readonly Dictionary<GameObject, IPooledInstanceActive> m_CacheInstanceActives =
                new Dictionary<GameObject, IPooledInstanceActive>();

            public InstancePool(ResourceManager resourceManager, AsyncOperationHandle<GameObject> prefabHandle,
                Transform transform)
            {
                m_ResourceManager = resourceManager;
                m_PrefabHandle = prefabHandle;
                m_Transform = transform;

                m_OnDestroyAction = Inactivate;
            }

            private GameObjectInstance Get()
            {
                m_LastRefTime = Time.unscaledTime;
                return m_Instances.Pop();
            }

            private void Put(GameObject gameObject)
            {
                m_LastRefTime = Time.unscaledTime;
                m_Instances.Push(new GameObjectInstance
                {
                    instance = gameObject,
                    isDontDestroy = gameObject.scene.path == "DontDestroyOnLoad"
                });
            }

            public GameObject ProvideInstance(InstantiationParameters instantiationParameters)
            {
                GameObject gameObject;
                if (m_Instances.Count > 0)
                {
                    var goInstance = Get();
                    m_ResourceManager.Release(m_PrefabHandle);

                    gameObject = goInstance.instance;
                    m_InstanceHashes.Remove(gameObject);
                    gameObject.transform.SetParent(instantiationParameters.Parent,
                        instantiationParameters.InstantiateInWorldPosition);
                    if (instantiationParameters.SetPositionRotation)
                        gameObject.transform.SetPositionAndRotation(instantiationParameters.Position,
                            instantiationParameters.Rotation);

                    if (instantiationParameters.Parent == null && !goInstance.isDontDestroy)
                        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
                }
                else gameObject = instantiationParameters.Instantiate(m_PrefabHandle.Result);

                Activate(gameObject);
                return gameObject;
            }

            public void ReleaseInstance(GameObject instance)
            {
                if (instance && m_Transform)
                {
                    if (!m_InstanceHashes.Add(instance))
                        return;
                    Put(instance);
                    Inactivate(instance);
                    instance.transform.SetParent(m_Transform, false);
                }
                else
                {
                    m_ResourceManager.Release(m_PrefabHandle);
                    Destroy(instance);
                }
            }

            private void Activate(GameObject instance)
            {
                Profiler.BeginSample("PooledInstanceProvider.Activate()");
                instance.GetComponents(s_CacheActives);
                s_CacheActives.Sort(s_ActiveComparison);
                if (s_CacheActives.Count > 0)
                {
                    var handle = s_CacheActives[0];
                    m_CacheInstanceActives.Add(instance, handle);
                    try
                    {
                        handle.OnDestroy = m_OnDestroyAction;
                        handle.OnActiveChanged(true);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }
                }
                else instance.SetActive(true);

                instance.SendMessage("OnInstanceActive", SendMessageOptions.DontRequireReceiver);
                Profiler.EndSample();
            }

            private void Inactivate(GameObject instance)
            {
                Profiler.BeginSample("PooledInstanceProvider.Inactivate()");
                if (instance) instance.SendMessage("OnInstanceInactive", SendMessageOptions.DontRequireReceiver);
                if (m_CacheInstanceActives.TryGetValue(instance, out var handle))
                {
                    try
                    {
                        if (handle != null)
                        {
                            handle.OnDestroy = null;
                            handle.OnActiveChanged(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }

                    m_CacheInstanceActives.Remove(instance);
                }
                else if (instance)
                    instance.SetActive(false);

                Profiler.EndSample();
            }

            internal bool Update(float releaseTime)
            {
                if (m_Instances.Count > 0)
                {
                    if ((m_Instances.Count > 1 && Time.unscaledTime - m_LastReleaseTime > releaseTime) ||
                        Time.unscaledTime - m_LastRefTime > (1f / m_Instances.Count) * releaseTime)
                    {
                        m_LastReleaseTime = m_LastRefTime = Time.unscaledTime;
                        var instance = m_Instances.Pop();
                        m_ResourceManager.Release(m_PrefabHandle);
                        m_InstanceHashes.Remove(instance.instance);
                        Destroy(instance.instance);
                    }
                }

                return m_Instances.Count > 0;
            }
        }
    }
}