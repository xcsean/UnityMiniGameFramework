using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityMiniGameFramework.UISystem
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class AspectFullFitter : UIBehaviour,ILayoutController
    {
        private static readonly List<AspectFullFitter> s_AllFitters = new List<AspectFullFitter>();
        
        public static bool IsFull { get; private set; }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            ScreenAdapter.OnScreenChanged += OnScreenChanged;
        }

        private static void OnScreenChanged(float ratio)
        {
            IsFull = ratio < AspectFullFitterSettings.Instance.AspectRatio;
            InvokeChange();
        }

        private static void InvokeChange()
        {
            foreach (var fitter in s_AllFitters)
                fitter.UpdateRect(IsFull);
        }
        
        [Serializable]
        public class FullStateChangeEvent : UnityEvent<bool> { }

        [SerializeField]
        private FullStateChangeEvent m_StateChangeEvent = new FullStateChangeEvent();
        
        [NonSerialized]
        private RectTransform m_Rect;

        private bool m_DelayedSetDirty;
        private bool m_DoesParentExist;
        private DrivenRectTransformTracker m_Tracker;

        public FullStateChangeEvent StateChangeEvent
        {
            get => m_StateChangeEvent;
            set => m_StateChangeEvent = value;
        }

        private RectTransform RectTransform
        {
            get
            {
                if (!m_Rect)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        protected override void OnEnable()
        {
            m_DoesParentExist = RectTransform.parent;
            s_AllFitters.Add(this);
            SetDirty();
        }

        protected override void Start()
        {
            if (!IsComponentValidOnObject() || !DoesParentExists())
                enabled = false;
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
            s_AllFitters.Remove(this);
        }

        protected override void OnTransformParentChanged()
        {
            if (!IsActive()) return;
            m_DoesParentExist = RectTransform.parent;
            SetDirty();
        }

        protected virtual void Update()
        {
            if (m_DelayedSetDirty)
            {
                m_DelayedSetDirty = false;
                SetDirty();
            }
        }

        private void UpdateRect(bool isFull)
        {
            if (!IsActive() || !IsComponentValidOnObject())
                return;

            m_Tracker.Clear();

            if (!DoesParentExists())
                return;
            m_Tracker.Add(this, RectTransform,
                DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition |
                DrivenTransformProperties.SizeDelta | DrivenTransformProperties.Pivot);

            RectTransform.pivot = Vector2.one * 0.5f;
            RectTransform.anchoredPosition = Vector2.zero;
            if (!isFull)
            {
                RectTransform.anchorMin = new Vector2(0.5f, 0);
                RectTransform.anchorMax = new Vector2(0.5f, 1);
                RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, GetOriginWidth());
            }
            else
            {
                RectTransform.anchorMin = Vector2.zero;
                RectTransform.anchorMax = Vector2.one;
                RectTransform.offsetMin = RectTransform.offsetMax = Vector2.zero;
            }
            m_StateChangeEvent?.Invoke(isFull);
        }

        private float GetOriginWidth()
        {
            var scaler = GetComponentInParent<CanvasScaler>();
            if (!scaler) return Screen.width;
            return scaler.referenceResolution.x;
        }

        public virtual void SetLayoutHorizontal() {}

        public virtual void SetLayoutVertical() {}

        protected void SetDirty()
        {
            UpdateRect(IsFull);
        }

        public bool IsComponentValidOnObject()
        {
            var canvas = gameObject.GetComponent<Canvas>();
            if (canvas && canvas.isRootCanvas && canvas.renderMode != RenderMode.WorldSpace)
                return false;
            return true;
        }

        private bool DoesParentExists() => m_DoesParentExist;
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            m_DelayedSetDirty = true;
        }
#endif
    }
}