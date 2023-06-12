using System.Collections.Generic;
using UnityEngine;

namespace UnityMiniGameFramework.Addressable
{
    public class PooledInstanceProviderBehaviour : MonoBehaviour
    {
        private static readonly List<PooledInstanceProviderBehaviour> s_AllBehaviours =
            new List<PooledInstanceProviderBehaviour>();
        
        private PooledInstanceProvider m_Provider;

        public void Init(PooledInstanceProvider provider)
        {
            m_Provider = provider;
            DontDestroyOnLoad(gameObject);
            s_AllBehaviours.Add(this);
        }

        private void Update()
        {
            m_Provider?.Update();
        }

        private void OnDestroy()
        {
            s_AllBehaviours.Remove(this);
            FlushInternal();
        }

        private void FlushInternal()
        {
            m_Provider?.Update(true);
        }

        public static void Flush()
        {
            foreach (var behaviour in s_AllBehaviours)
                behaviour.FlushInternal();
        }
    }
}