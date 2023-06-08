using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityMiniGameFramework.UISystem
{
    [DisallowMultipleComponent]
    public sealed class UIFollowSorter : UIBehaviour
    {
        private readonly List<UIFollow> m_Follows = new List<UIFollow>();

        public enum Axis
        {
            X,
            Y,
            Z
        }

        [SerializeField]
        private Axis m_SortAxis = Axis.Z;

        public Axis SortAxis
        {
            get => m_SortAxis;
            set => m_SortAxis = value;
        }

        protected override void OnEnable()
        {
            UIFollowTracker.TrackSorter(this);
            for (var i = 0; i < transform.childCount; i++)
            {
                var follow = transform.GetChild(i).GetComponent<UIFollow>();
                if (follow) m_Follows.Add(follow);
            }
        }

        protected override void OnDisable()
        {
            UIFollowTracker.UntrackSorter(this);
            m_Follows.Clear();
        }

        internal void Register(UIFollow follow)
        {
            if (m_Follows.Contains(follow))
                return;
            m_Follows.Add(follow);
        }

        internal void Unregister(UIFollow follow)
        {
            m_Follows.Remove(follow);
        }

        private bool NeedSwap(UIFollow follow, UIFollow target)
        {
            if (follow.SortingOrder < target.SortingOrder)
                return true;
            if (follow.SortingOrder > target.SortingOrder)
                return false;
            
            switch (m_SortAxis)
            {
                case Axis.X:
                    return follow.transform.position.x > target.transform.position.x;
                case Axis.Y:
                    return follow.transform.position.y > target.transform.position.y;
            }

            return follow.Distance > target.Distance;
        }

        internal void OnCanvasesRender()
        {
            for (var i = 1; i < m_Follows.Count; i++)
            {
                var follow = m_Follows[i];
                for (var j = 0; j < i; j++)
                {
                    var checkFollow = m_Follows[j];
                    if (!follow.HasChanged && !checkFollow.HasChanged) continue;
                    if (NeedSwap(follow, checkFollow))
                    {
                        m_Follows.RemoveAt(i);
                        m_Follows.Insert(j, follow);
                        follow.transform.SetSiblingIndex(j);
                        break;
                    }
                }
            }
        }
    }
}