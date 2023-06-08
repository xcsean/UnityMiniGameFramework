using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityMiniGameFramework.UISystem
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class UIFollow : UIBehaviour
    {
        private const float MINEST_DISTANCE = 0.0001f;

        public enum UIFollowMode
        {
            Transform,
            Position
        }

        [SerializeField]
        private UIFollowMode m_Mode = UIFollowMode.Transform;
        [SerializeField]
        private Transform m_Target;
        [SerializeField]
        private Vector3 m_Position;
        [SerializeField]
        private Camera m_Camera;
        [Range(MINEST_DISTANCE, 1)]
        [SerializeField]
        private float m_Interval = 1;
        [SerializeField]
        private bool m_ScaleByDistance = true;
        [SerializeField]
        private bool m_ScaleWithTarget = true;
        [SerializeField]
        private float m_Scale = 1;
        [SerializeField]
        private float m_MinScale = 0;
        [SerializeField]
        private float m_MaxScale = 1000;

        [SerializeField]
        private Vector2 m_Offset;
        [SerializeField]
        private int m_SortingOrder;

        [NonSerialized]
        private RectTransform m_RectTransform;
        [NonSerialized]
        private RectTransform m_Parent;
        [NonSerialized]
        private Canvas m_Canvas;
        [NonSerialized]
        private bool m_IsDirty;
        [NonSerialized]
        private bool m_IsLocalPointValid;
        [NonSerialized]
        private Vector2 m_LocalPoint;
        [NonSerialized]
        private float m_LocalScale;
        [NonSerialized]
        private int m_CurrentFrame;

        [NonSerialized]
        private Vector3 m_CachedTargetPosition;
        [NonSerialized]
        private Vector3 m_CachedTargetScale = Vector3.one;
        [NonSerialized]
        private Vector3 m_CachedCameraPosition;
        [NonSerialized]
        private Quaternion m_CachedCameraRotation;
        [NonSerialized]
        private bool m_Orthographic;
        [NonSerialized]
        private float m_FovOrSize;
        [NonSerialized]
        private Vector3 m_CachedLocalScale;

        [NonSerialized]
        private UIFollowSorter m_Sorter;

        public float Distance { get; private set; }

        internal bool HasChanged { get; private set; }

        public UIFollowMode Mode
        {
            get => m_Mode;
            set
            {
                if (SetPropertyUtility.SetStruct(ref m_Mode, value))
                    SetDirty();
            }
        }

        public Transform Target
        {
            get => m_Target;
            set
            {
                if (SetPropertyUtility.SetClass(ref m_Target, value))
                    SetDirty();
                Mode = UIFollowMode.Transform;
            }
        }

        public Vector3 Position
        {
            get { return m_Position; }
            set
            {
                if (SetPropertyUtility.SetStruct(ref m_Position, value))
                    SetDirty();
                Mode = UIFollowMode.Position;
            }
        }

        public Camera Camera
        {
            get
            {
                if (!m_Camera || !m_Camera.isActiveAndEnabled)
                {
                    var mc = Camera.main;
                    if (mc) m_Camera = mc;
                }
                return m_Camera;
            }
            set
            {
                if (SetPropertyUtility.SetClass(ref m_Camera, value))
                    SetDirty();
            }
        }

        public float Interval
        {
            get => m_Interval;
            set
            {
                if (SetPropertyUtility.SetStruct(ref m_Interval, value))
                    SetDirty();
            }
        }

        public bool ScaleByDistance
        {
            get => m_ScaleByDistance;
            set
            {
                if (SetPropertyUtility.SetStruct(ref m_ScaleByDistance, value))
                    SetDirty();
            }
        }

        public bool ScaleWithTarget
        {
            get => m_ScaleWithTarget;
            set
            {
                if (SetPropertyUtility.SetStruct(ref m_ScaleWithTarget, value))
                    SetDirty();
            }
        }

        public float Scale
        {
            get => m_Scale;
            set
            {
                if (SetPropertyUtility.SetStruct(ref m_Scale, value))
                    SetDirty();
            }
        }

        public Vector2 Offset
        {
            get => m_Offset;
            set
            {
                if (SetPropertyUtility.SetStruct(ref m_Offset, value))
                    SetDirty();
            }
        }

        public int SortingOrder
        {
            get => m_SortingOrder;
            set
            {
                if (SetPropertyUtility.SetStruct(ref m_SortingOrder, value))
                    SetDirty();
            }
        }

        public RectTransform RectTransform
        {
            get
            {
                if (!m_RectTransform)
                    m_RectTransform = GetComponent<RectTransform>();
                return m_RectTransform;
            }
        }

        protected override void OnEnable()
        {
            UpdateFollowStatus();
        }

        protected override void OnTransformParentChanged()
        {
            UpdateFollowStatus();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            SetDirty();
        }

        private void UpdateFollowStatus()
        {
            m_Parent = transform.parent as RectTransform;
            m_Canvas = GetComponentInParent<Canvas>();
            if (m_Parent && m_Canvas)
            {
                UIFollowTracker.TrackFollow(this);
                RegisterFollowSorter();
                SetDirty();
            }
            else
            {
                UIFollowTracker.UntrackFollow(this);
                UnregisterFollowSorter();
            }
        }

        private void RegisterFollowSorter()
        {
            var sorter = GetComponentInParent<UIFollowSorter>();
            if (m_Sorter != sorter)
            {
                UnregisterFollowSorter();
                m_Sorter = sorter;
                if (m_Sorter)
                    m_Sorter.Register(this);
            }
        }

        private void UnregisterFollowSorter()
        {
            if (!m_Sorter) return;
            m_Sorter.Unregister(this);
            m_Sorter = null;
        }

        protected override void OnDisable()
        {
            UIFollowTracker.UntrackFollow(this);
            UnregisterFollowSorter();
        }

        protected override void OnDestroy()
        {
            OnDisable();
        }

        private bool Validate(out Camera renderCamera, out Vector3 pos, out bool isUI)
        {
            renderCamera = Camera;
            pos = Vector3.zero;
            isUI = false;
            switch (m_Mode)
            {
                case UIFollowMode.Position:
                    pos = m_Position;
                    break;
                case UIFollowMode.Transform:
                    if (!m_Target) return false;
                    var targetCanvas = m_Target.GetComponentInParent<Canvas>();
                    if (targetCanvas) isUI = true;
                    renderCamera = targetCanvas ? GetCanvasRenderCamera(targetCanvas) : renderCamera;
                    pos = m_Target.position;
                    break;
                default:
                    return false;
            }
            return true;
        }

        private static Camera GetCanvasRenderCamera(Canvas canvas)
        {
            if (!canvas.isRootCanvas)
                canvas = canvas.rootCanvas;
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return null;
            return canvas.worldCamera;
        }

        private bool HasTransformChanged(Camera renderCamera)
        {
            var hasChanged = false;
            if (m_Mode == UIFollowMode.Transform && m_Target)
            {
                hasChanged |= m_Target.position != m_CachedTargetPosition;
                hasChanged |= m_Target.localScale != m_CachedTargetScale;
            }
            if (renderCamera)
            {
                hasChanged |= renderCamera.transform.position != m_CachedCameraPosition;
                hasChanged |= renderCamera.transform.rotation != m_CachedCameraRotation;
                hasChanged |= renderCamera.orthographic != m_Orthographic;
                hasChanged |= (renderCamera.orthographic ? renderCamera.orthographicSize : renderCamera.fieldOfView) != m_FovOrSize;
            }
            return hasChanged;
        }

        private void RecordTransforms(Camera renderCamera)
        {
            if (renderCamera)
            {
                m_CachedCameraPosition = renderCamera.transform.position;
                m_CachedCameraRotation = renderCamera.transform.rotation;
                m_Orthographic = renderCamera.orthographic;
                m_FovOrSize = m_Orthographic ? renderCamera.orthographicSize : renderCamera.fieldOfView;
            }
            if (m_Mode == UIFollowMode.Position)
                m_CachedTargetScale = Vector3.one;
            else if (m_Mode == UIFollowMode.Transform && m_Target)
            {
                m_CachedTargetPosition = m_Target.position;
                m_CachedTargetScale = m_Target.localScale;
            }
        }

        internal void OnCanvasesRender()
        {
            HasChanged = false;
            if (Validate(out Camera renderCamera, out Vector3 pos, out bool isUI))
            {
                if (m_IsDirty || HasTransformChanged(renderCamera))
                {
                    RecordTransforms(renderCamera);
                    UpdateDistance(renderCamera, pos);
                    CalculateDistanceScale(renderCamera, isUI);

                    var offset = CalculateOffset(isUI);
                    var worldPos = isUI ? pos : (pos + offset);
                    var screenPoint = RectTransformUtility.WorldToScreenPoint(renderCamera, worldPos);
                    m_IsLocalPointValid = RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Parent, screenPoint, GetCanvasRenderCamera(m_Canvas), out m_LocalPoint);
                    if (isUI) m_LocalPoint += new Vector2(offset.x, offset.y);
                    HasChanged = true;
                }
                else if (RectTransform.localScale != m_CachedLocalScale)
                {
                    m_CachedLocalScale = RectTransform.localScale;
                    m_IsLocalPointValid = true;
                }
            }
            if (m_IsLocalPointValid && (m_IsDirty || m_CurrentFrame != Time.frameCount))
            {
                RectTransform.anchorMin = RectTransform.anchorMax = Vector2.one * 0.5f;
                RectTransform.anchoredPosition = Vector2.Lerp(RectTransform.anchoredPosition, m_LocalPoint, m_Interval);
                RectTransform.localScale = m_LocalScale * (m_ScaleWithTarget ? m_CachedTargetScale : Vector3.one);
                m_CachedLocalScale = RectTransform.localScale;
                m_IsLocalPointValid = (RectTransform.anchoredPosition - m_LocalPoint).sqrMagnitude > MINEST_DISTANCE;
                m_CurrentFrame = Time.frameCount;
            }
            m_IsDirty = false;
        }

        private Vector3 CalculateOffset(bool isUI)
        {
            Vector3 offset = m_Offset;
            if (!isUI) offset = m_CachedCameraRotation * offset;
            return new Vector3(offset.x * m_CachedTargetScale.x, offset.y * m_CachedTargetScale.y, offset.z * m_CachedTargetScale.z);
        }

        private void CalculateDistanceScale(Camera renderCamera, bool isUI)
        {
            float scale = 1;
            if (renderCamera && !isUI && m_ScaleByDistance)
            {
                if (renderCamera.orthographic)
                    scale = renderCamera.orthographicSize;
                else
                    scale = Camera.FieldOfViewToFocalLength(renderCamera.fieldOfView, Distance);
            }
            m_LocalScale = Mathf.Min(Mathf.Max(m_Scale / scale, m_MinScale), m_MaxScale);
        }

        private void UpdateDistance(Camera renderCamera, Vector3 pos)
        {
            if (!renderCamera)
                Distance = pos.z;
            else
            {
                var vector = pos - renderCamera.transform.position;
                var project = Vector3.Project(vector, renderCamera.transform.forward);
                Distance = project.magnitude * (Vector3.Dot(project, renderCamera.transform.forward) > 0 ? 1 : -1);
            }
        }

        private void SetDirty()
        {
            m_IsDirty = true;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }
#endif
    }
}