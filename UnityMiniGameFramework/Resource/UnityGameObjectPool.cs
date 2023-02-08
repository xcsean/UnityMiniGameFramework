using System.Collections.Generic;
using MiniGameFramework;
using UnityEngine;
using Debug = MiniGameFramework.Debug;
using GameObject = UnityEngine.GameObject;

namespace UnityMiniGameFramework
{
    public class UnityGameObjectPool
    {
        private static UnityGameObjectPool m_Instance;
        private const float m_ReleaseTime = 10f;

        struct ObjectSaveInfo
        {
            public UnityEngine.GameObject go;
            public float releaseCountDown;

            public ObjectSaveInfo(GameObject Go, float Time)
            {
                go = Go;
                releaseCountDown = Time;
            }
        }

        private Dictionary<string, List<ObjectSaveInfo>> _dictionary = new Dictionary<string, List<ObjectSaveInfo>>();
        private static Renderer[] _renderers = new Renderer[] { };

        public static UnityGameObjectPool GetInstance()
        {
            if (m_Instance == null)
                m_Instance = new UnityGameObjectPool();
            return m_Instance;
        }

        private UnityGameObjectPool()
        {
            UnityGameApp.Inst.addUpdateCall(onUpdate);
        }

        private void onUpdate()
        {
            foreach (var element in _dictionary.Values)
            {
                for (int i = element.Count - 1; i >= 0; i--)
                {
                    var info = element[i];
                    if (Time.unscaledTime - info.releaseCountDown - m_ReleaseTime >= 0.0f)
                    {
                        element.RemoveAt(i);
                        GameObject.Destroy(info.go);
                    }
                }
            }
        }

        public UnityEngine.GameObject GetUnityPrefabObject(string nameKey)
        {
            if (!m_Instance._dictionary.ContainsKey(nameKey))
            {
                var go = UnityGameApp.Inst.UnityResource.LoadUnityPrefabObject(nameKey);
                return GameObject.Instantiate(go);
            }
            else
            {
                var pool = m_Instance._dictionary[nameKey];
                GameObject go;
                if (pool.Count == 0)
                {
                    go = UnityGameApp.Inst.UnityResource.LoadUnityPrefabObject(nameKey);
                    return GameObject.Instantiate(go);
                }

                ObjectSaveInfo info = pool[pool.Count - 1];
                go = info.go;
                pool.RemoveAt(pool.Count - 1);
                _renderers = info.go.GetComponentsInChildren<Renderer>();
                if (_renderers != null)
                {
                    foreach (var render in _renderers)
                    {
                        render.enabled = true;
                    }
                }
                else
                    go.SetActive(true);
                return go;
            }
        }

        public void PutUnityPrefabObject(string nameKey, UnityEngine.GameObject go)
        {
            if (go == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"PutUnityPrefabObject gameObject is null");
                return;
            }

            if (!m_Instance._dictionary.ContainsKey(nameKey))
                m_Instance._dictionary.Add(nameKey, new List<ObjectSaveInfo>());
            _renderers = go.GetComponentsInChildren<Renderer>();
            if (_renderers != null)
            {
                foreach (var render in _renderers)
                {
                    render.enabled = false;
                }
            }
            else
                go.SetActive(false);
            m_Instance._dictionary[nameKey].Add(new ObjectSaveInfo(go, Time.unscaledTime));
            go.transform.SetParent(UnityGameApp.Inst.CachePoolRoot.transform);
            go.transform.localPosition = Vector3.zero;
        }
    }
}