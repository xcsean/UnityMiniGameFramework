using System;
using MiniGameFramework;
using UnityEngine;
using UnityEngine.Events;
using UnityMiniGameFramework.Addressable;
using UnityEngine.AddressableAssets;

namespace UnityMiniGameFramework.UISystem
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public class UIView : PooledInstanceBehaviour
    {
        [SerializeField]
        private bool m_AutoClose = true;
        [SerializeField]
        private UnityEvent<bool> m_OnVisibleChanged = new UnityEvent<bool>();

        private CanvasGroup m_CanvasGroup;

        public override int priority => 100;

        public bool AutoClose => m_AutoClose;

        public virtual UICanvasType CanvasType => UICanvasType.Follow;

        public bool IsVisible { get; private set; }

        public UnityEvent<bool> OnVisibleChanged
        {
            get => m_OnVisibleChanged;
            set => m_OnVisibleChanged = value;
        }

        public CanvasGroup CanvasGroup
        {
            get
            {
                if (!m_CanvasGroup)
                    m_CanvasGroup = GetComponent<CanvasGroup>();
                return m_CanvasGroup;
            }
        }

        protected virtual void Awake()
        {
            Hide();
        }

        protected override void OnActiveChanged(bool active)
        {
            if (active)
                UIManager.OpenView(this);
            else
                UIManager.CloseView(this);
        }

        internal virtual void Show(bool withoutAnim = false)
        {
            if (IsVisible) return;
            IsVisible = true;
            RawShow();
            CanvasGroup.blocksRaycasts = true;
            if (TryGetComponent(out AspectFullFitter fitter))
                fitter.enabled = true;
            m_OnVisibleChanged?.Invoke(true);
        }

        internal virtual void Hide(bool withoutAnim = false)
        {
            if (!IsVisible) return;
            IsVisible = false;
            RawHide();
            CanvasGroup.blocksRaycasts = false;
            if (TryGetComponent(out AspectFullFitter fitter))
                fitter.enabled = false;
            m_OnVisibleChanged?.Invoke(false);
        }

        internal void RawHide()
        {
            CanvasGroup.alpha = 0;
        }

        internal void RawShow()
        {
            CanvasGroup.alpha = 1;
        }

        internal virtual void Destroy()
        {
            Addressables.ReleaseInstance(gameObject);
        }
    }
}