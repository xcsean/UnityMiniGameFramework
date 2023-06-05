using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityMiniGameFramework.UISystem
{
    
    using Animation = UnityEngine.Animation;
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    public sealed class UIMask : UIBehaviour
    {
        private Canvas m_Canvas;
        private CanvasGroup m_CanvasGroup;

        private bool m_IsOpening = false;
        private const string OPEN_ANIM_NAME = "Open";
        private const string CLOSE_ANIM_NAME = "Close";
        [SerializeField] private Animation m_Animation;
        [SerializeField] private AnimationClip m_OpenAnimClip;
        [SerializeField] private AnimationClip m_CloseAnimClip;
        bool m_IsInteractable = true;
        public Canvas Canvas
        {
            get
            {
                if (!m_Canvas)
                    m_Canvas = GetComponent<Canvas>();
                return m_Canvas;
            }
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
        public Animation Animation
        {
            get
            {
                if (!m_Animation)
                    m_Animation = GetComponent<Animation>();
                return m_Animation;
            }
        }

        internal UIPanel Panel { get; private set; }

        internal int SortingOrder
        {
            get => Canvas.sortingOrder;
            set
            {
                Canvas.overrideSorting = true;
                Canvas.sortingOrder = value;
            }
        }

        internal void InitPanel(UIPanel panel)
        {
            Panel = panel;
            if (panel)
            {
                SortingOrder = panel.SortingOrder - 1;
                Show();
            }
            else Hide();
        }

        private void Show()
        {
            if (!Animation) { ShowForce(); return; }
            var clip = Animation.GetClip(OPEN_ANIM_NAME);
            if (!clip) clip = m_OpenAnimClip;
            if (!clip) { ShowForce(); return; }
            //Debug.LogWarning("Mask: Show");
            if (!m_IsOpening)
            {
                m_IsOpening = true;
                Animation.AddClip(clip, OPEN_ANIM_NAME);
                Debug.Log("debugGuide UIMASK open");
                Animation.Play(OPEN_ANIM_NAME, PlayMode.StopAll);
                Animation.Sample();
            }
        }

        internal void Hide()
        {
            m_IsOpening = false;
            HideForce();
            //if (!Animation) { HideForce(); return; }
            //var clip = Animation.GetClip(CLOSE_ANIM_NAME);
            //if (!clip) clip = m_CloseAnimClip;
            //if (!clip) { HideForce(); return; }
            ////Debug.LogWarning("Mask: Hide");
            //if (m_IsOpening)
            //{
            //    m_IsOpening = false;
            //    Animation.AddClip(clip, CLOSE_ANIM_NAME);
            //    Animation.Play(CLOSE_ANIM_NAME, PlayMode.StopAll);
            //}
        }

        void ShowForce()
        {
            StopAnimation();
            CanvasGroup.alpha = 1;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
        }
        void HideForce()
        {
            StopAnimation();
            CanvasGroup.alpha = 0;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
        }

        public void SetInteractable(bool isInteractable)
        {
            m_IsInteractable = isInteractable;
        }

        void StopAnimation()
        {
            if (Animation && Animation.isPlaying)
            {
                Animation.Stop();
            }
        }

        public void OnMaskClick()
        {
            if(!m_IsInteractable)
                return;
            Debug.Log("OnMaskClick1");
            if (!Animation || !Animation.isPlaying)
            {
                Debug.Log("OnMaskClick2");
                Panel.OnPanelMaskClick();
            }
        }
    }
}