using System.Collections.Generic;
using UnityEngine;

namespace UnityMiniGameFramework
{
    public class UnityGameObjectPool
    {
        private static UnityGameObjectPool m_Instance;

        private Dictionary<string, List<UnityEngine.GameObject>> _dictionary =
            new Dictionary<string, List<GameObject>>();
        public static UnityGameObjectPool GetInstance()
        {
            if (m_Instance == null)
                m_Instance = new UnityGameObjectPool();
            return m_Instance;
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
                go = pool[pool.Count - 1];
                pool.RemoveAt(pool.Count - 1);
                var render = go.GetComponent<Renderer>();
                if (render != null)
                    render.enabled = true;
                else
                    go.SetActive(true);
                return go;
            }
        }

        public void PutUnityPrefabObject(string nameKey, UnityEngine.GameObject go)
        {
            if(!m_Instance._dictionary.ContainsKey(nameKey))
                m_Instance._dictionary.Add(nameKey,new List<GameObject>());
            var render = go.GetComponent<Renderer>();
            if (render != null)
                render.enabled = false;
            else
                go.SetActive(false);
            m_Instance._dictionary[nameKey].Add(go);
            go.transform.SetParent(UnityGameApp.Inst.CachePoolRoot.transform);
        }
    }
}