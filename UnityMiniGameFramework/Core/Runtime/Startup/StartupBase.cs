using System;
using UnityEngine;

namespace UnityMiniGameFramework
{
    public abstract class StartupBase : ScriptableObject
    {
        public static readonly Comparison<StartupBase> startupComparer = StartupComparer;

        [SerializeField, HideInInspector]
        private int m_Order;

        public int order
        {
            get { return m_Order; }
            set { m_Order = value; }
        }

        private static int StartupComparer(StartupBase lt, StartupBase rt)
        {
            return lt.order.CompareTo(rt.order);
        }
    }
}