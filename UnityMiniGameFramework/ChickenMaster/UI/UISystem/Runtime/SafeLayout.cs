using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace UnityMiniGameFramework.UISystem
{
    [ExecuteInEditMode, RequireComponent(typeof(RectTransform))]
    public class SafeLayout : UIBehaviour, ILayoutController
    {
        [SerializeField] private RectOffset m_Padding;

        [NonSerialized] private RectTransform m_Rect;
        [NonSerialized] private Canvas m_Canvas;

        private DrivenRectTransformTracker m_Tracker;

        private RectTransform RectTransform
        {
            get
            {
                if (!m_Rect)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        private void ResetCanvas() => m_Canvas = GetComponentInParent<Canvas>();

        protected override void OnEnable()
        {
            base.OnEnable();
            ResetCanvas();
            SetDirty();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
            base.OnDisable();
        }

        public void SetLayoutHorizontal()
        {
            m_Tracker.Clear();
            m_Tracker.Add(this, RectTransform, DrivenTransformProperties.All);
        }

        public void SetLayoutVertical()
        {
            RectTransform.offsetMin = Vector2.zero;
            RectTransform.offsetMax = Vector2.zero;
            RectTransform.pivot = Vector2.one * 0.5f;
            RectTransform.localScale = Vector3.one;
            RectTransform.rotation = Quaternion.identity;

            var safeArea = GetSafeArea();
            RectTransform.anchorMin = new Vector2(Mathf.Max(safeArea.x / Screen.width, 0),
                Mathf.Max(safeArea.y / Screen.height, 0));
            RectTransform.anchorMax = new Vector2(Mathf.Min((safeArea.x + safeArea.width) / Screen.width, 1),
                Mathf.Min((safeArea.y + safeArea.height) / Screen.height, 1));
        }

        private Rect GetSafeArea()
        {
            var safeArea = Screen.safeArea;
            var padding = new Vector4(m_Padding.left, m_Padding.bottom, m_Padding.horizontal, m_Padding.vertical) *
                          m_Canvas.scaleFactor;
            safeArea.Set(safeArea.xMin + padding.x, safeArea.yMin + padding.y, safeArea.width - padding.z,
                safeArea.height - padding.w);
            return safeArea;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

        protected override void OnTransformParentChanged()
        {
            ResetCanvas();
            SetDirty();
        }

        public void SetDirty()
        {
            if (IsActive())
                LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }
#endif
    }
}