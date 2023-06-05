using System.Collections;
using UnityMiniGameFramework.Addressable;
using UnityEngine;
using UnityEngine.Events;

namespace UnityMiniGameFramework.UISystem
{
    using Animation = UnityEngine.Animation;
    [RequireComponent(typeof(Canvas))]
    public sealed class UIPanel : UIView, IReleaseProvider
    {
        [SerializeField] private UICanvasType m_CanvasType = UICanvasType.Auto;
        [SerializeField] private bool m_Transparent = true;
        [SerializeField] private UIMaskType m_MaskType = UIMaskType.Default;
        [SerializeField] private int m_Order;
        [SerializeField] private bool m_MaskEvent;
        [SerializeField] private UnityEvent m_OnMaskClick = new UnityEvent();

        [SerializeField] private bool m_BeginHideMaskWhenClose;
        [SerializeField] private Animation m_Animation;
        [SerializeField] private AnimationClip m_OpenAnimClip;
        [SerializeField] private AnimationClip m_CloseAnimClip;

        private const string OPEN_ANIM_NAME = "Open";
        private const string CLOSE_ANIM_NAME = "Close";

        public override UICanvasType CanvasType => m_CanvasType;

        public bool Transparent => m_Transparent;

        public UIMaskType MaskType => m_MaskType;

        public int Order => m_Order;

        public UnityEvent OnMaskClick
        {
            get => m_OnMaskClick;
            set => m_OnMaskClick = value;
        }

        public int SortingOrder
        {
            get => Canvas.sortingOrder;
            set
            {
                Canvas.overrideSorting = true;
                Canvas.sortingOrder = value;
            }
        }

        private Canvas m_Canvas;

        public Canvas Canvas
        {
            get
            {
                if (!m_Canvas)
                    m_Canvas = GetComponent<Canvas>();
                return m_Canvas;
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

        /// <summary>点击关闭按钮之后，是否渐隐 Mask </summary>
        public bool BeginHideMaskWhenClose
        {
            get => m_BeginHideMaskWhenClose;
            set => m_BeginHideMaskWhenClose = value;
        }

        public AnimationClip OpenAnimClip
        {
            get => m_OpenAnimClip;
            set => m_OpenAnimClip = value;
        }

        public AnimationClip CloseAnimClip
        {
            get => m_CloseAnimClip;
            set => m_CloseAnimClip = value;
        }

        internal void OnPanelMaskClick()
        {
            if (Animation && Animation.isPlaying)
                return;
            if (m_MaskEvent)
                m_OnMaskClick?.Invoke();
            else
                Destroy();
        }

        internal override void Show(bool withoutAnim = false)
        {
            if (IsVisible) return;
            base.Show(withoutAnim);
            if (withoutAnim || !Animation) return;
            var clip = Animation.GetClip(OPEN_ANIM_NAME);
            if (!clip) clip = m_OpenAnimClip;
            if (!clip) return;
            Animation.AddClip(clip, OPEN_ANIM_NAME);
            Animation.Play(OPEN_ANIM_NAME, PlayMode.StopAll);
            Animation.Sample();
        }

        void IReleaseProvider.Release() => Destroy();

        internal override void Destroy()
        {
            if (!Animation)
            {
                base.Destroy();
                return;
            }

            var clip = Animation.GetClip(CLOSE_ANIM_NAME);
            if (!clip) clip = m_CloseAnimClip;
            if (!clip)
            {
                base.Destroy();
                return;
            }

            if (!Animation.isPlaying)
            {
                Animation.AddClip(clip, CLOSE_ANIM_NAME);
                StartCoroutine(WaitForAnimation());
            }

            if (BeginHideMaskWhenClose)
                UIManager.HideMask(this);
        }

        private IEnumerator WaitForAnimation()
        {
            Animation.Play(CLOSE_ANIM_NAME, PlayMode.StopAll);
            yield return new WaitWhile(() => Animation.isPlaying);
            Hide();
            base.Destroy();
        }
    }
}