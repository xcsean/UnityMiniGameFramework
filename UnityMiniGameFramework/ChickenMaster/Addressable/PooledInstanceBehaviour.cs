using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityMiniGameFramework.Addressable
{
    public class PooledInstanceBehaviour : MonoBehaviour, IPooledInstanceActive
    {
        private Action<GameObject> m_OnDestroy;
        private readonly List<IPooledActive> m_InstanceActives = new List<IPooledActive>();
        private static readonly Comparison<IPooledActive> s_ActiveComparison = (Comparison<IPooledActive>) ((active1, active2) => active2.priority.CompareTo(active1.priority));

        public virtual int priority => 0;

        Action<GameObject> IPooledInstanceActive.OnDestroy
        {
            set => this.m_OnDestroy = value;
        }

        void IPooledInstanceActive.OnActiveChanged(bool active)
        {
            if (active)
            {
                this.GetComponents<IPooledActive>(this.m_InstanceActives);
                this.m_InstanceActives.Sort(PooledInstanceBehaviour.s_ActiveComparison);
                foreach (IPooledActive instanceActive in this.m_InstanceActives)
                    instanceActive.OnActive();
            }
            else
            {
                foreach (IPooledActive instanceActive in this.m_InstanceActives)
                    instanceActive.OnInactive();
            }
            this.OnActiveChanged(active);
        }

        public virtual void OnActive()
        {
        }

        public virtual void OnInactive()
        {
        }

        protected virtual void OnActiveChanged(bool active) => this.gameObject.SetActive(active);

        protected virtual void OnDestroy()
        {
            Action<GameObject> onDestroy = this.m_OnDestroy;
            if (onDestroy == null)
                return;
            onDestroy(this.gameObject);
        }
    }
}