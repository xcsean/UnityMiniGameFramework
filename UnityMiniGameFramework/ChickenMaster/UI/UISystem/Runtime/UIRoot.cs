using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnityMiniGameFramework.UISystem
{
    using Scene = UnityEngine.SceneManagement.Scene;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    public sealed class UIRoot : UIBehaviour
    {
        [SerializeField] private bool m_Optimize;
        [SerializeField] private LayerMask m_LayerMask;
        [SerializeField] private UIMask m_Mask;
        [SerializeField] private UIMask m_TransparentMask;
        [SerializeField] private EmptyGraphic m_Blocker;

        private Canvas m_Canvas;
        private CanvasGroup m_CanvasGroup;

        private readonly List<UICanvasLayer> m_Layers = new List<UICanvasLayer>();

        private readonly Dictionary<UICanvasType, UICanvasLayer> m_CanvasLayers =
            new Dictionary<UICanvasType, UICanvasLayer>();

        private static readonly Comparison<UICanvasLayer> s_LayerComparison =
            (l1, l2) => l1.CanvasType.CompareTo(l2.CanvasType);

        public LayerMask layerMask
        {
            get => m_LayerMask;
            set => m_LayerMask = value;
        }

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

        protected override void Awake()
        {
            DontDestroyOnLoad(gameObject);
            UIManager.root = this;
        }

        protected override void Start()
        {
            InitCanvasLayers();
        }

        protected override void OnEnable()
        {
            Canvas.willRenderCanvases += OnWillRenderCanvases;
            SceneManager.SceneLoaded += OnSceneLoaded;
            GameManager.onApplicationReset += OnReset;
        }

        protected override void OnDisable()
        {
            Canvas.willRenderCanvases -= OnWillRenderCanvases;
            SceneManager.SceneLoaded -= OnSceneLoaded;
            GameManager.onApplicationReset -= OnReset;
        }

        private void InitCanvasLayers()
        {
            GetComponentsInChildren(true, m_Layers);
            m_Layers.Sort(s_LayerComparison);
            foreach (var layer in m_Layers)
            {
                if (m_CanvasLayers.ContainsKey(layer.CanvasType))
                    Destroy(layer.gameObject);
                else
                    m_CanvasLayers.Add(layer.CanvasType, layer);
            }
        }

        private UICanvasLayer GetCanvasLayer(UICanvasType canvasType) =>
            m_CanvasLayers.TryGetValue(canvasType, out var layer) ? layer : null;

        public void SetCanvasLayerHide(UICanvasType canvasType)
        {
            UICanvasLayer canvasLayer = GetCanvasLayer(canvasType);
            canvasLayer.HideLayer();
        }

        public void SetCanvasLayerShow(UICanvasType canvasType)
        {
            UICanvasLayer canvasLayer = GetCanvasLayer(canvasType);
            canvasLayer.ShowLayer();
        }

        internal void ShowView(UIView view)
        {
            var layer = GetCanvasLayer(view.CanvasType);
            if (!layer)
                throw new NullReferenceException($"Can't find layer of type {view.CanvasType}");
            layer.ShowView(view);
        }

        internal void HideView(UIView view)
        {
            var layer = GetCanvasLayer(view.CanvasType);
            if (!layer)
                throw new NullReferenceException($"Can't find layer of type {view.CanvasType}");
            layer.HideView(view);
        }

        internal void HideMask(UIView view)
        {
            if (!(view is UIPanel panel) || !panel.Transparent || panel.MaskType != UIMaskType.Default)
                return;
            m_Mask.Hide();
        }

        internal void ShowAllViews()
        {
            foreach (var layer in m_Layers)
                layer.ShowAllViews();
        }

        internal void HideAllViews()
        {
            foreach (var layer in m_Layers)
            {
                layer.HideAllViews();
            }
        }

        internal UIView FindView(string viewName)
        {
            foreach (var layer in m_Layers)
            {
                var view = layer.FindView(viewName);
                if (view) return view;
            }

            return default;
        }

        internal List<UIView> GetAllViews(UICanvasType canvasType)
        {
            foreach (var layer in m_Layers)
            {
                if (layer.CanvasType != canvasType) continue;
                return layer.GetAllViews();
            }

            return default;
        }

        private void OnSceneLoaded(bool isSuccess, object address, Scene _, bool isAdditive)
        {
            if (!isSuccess || isAdditive) return;
            DestroyAllViews();
        }

        private IEnumerator OnReset()
        {
            for (var i = m_Layers.Count - 1; i >= 0; i--)
                m_Layers[i].DestroyAllViews(true);
            yield break;
        }

        internal void DestroyAllViews(UICanvasType type = UICanvasType.Follow)
        {
            for (var i = m_Layers.Count - 1; i >= 0; i--)
            {
                var layer = m_Layers[i];
                if (layer.CanvasType < type)
                    continue;

                layer.DestroyAllViews();
            }
        }

        public void HideCanvasLayer(UICanvasType canvasType)
        {
            for (var i = m_Layers.Count - 1; i >= 0; i--)
            {
                if (m_Layers[i].CanvasType == canvasType)
                {
                    m_Layers[i].DestroyAllViews();
                    break;
                }
            }
        }

        public void HideAllLayer()
        {
            for (var i = m_Layers.Count - 1; i >= 0; i--)
            {
                m_Layers[i].HideLayer();
            }
        }

        public void ShowAllLayer()
        {
            for (var i = m_Layers.Count - 1; i >= 0; i--)
            {
                m_Layers[i].ShowLayer();
            }
        }

        public void SetInteractable(bool isInteract)
        {
            for (var i = m_Layers.Count - 1; i >= 0; i--)
            {
                m_Layers[i].SetInteractable(isInteract);
            }

            //BlockClick(!isInteract);
            m_Mask.SetInteractable(isInteract);
        }

        private bool IsLayerDirty()
        {
            foreach (var layer in m_Layers)
            {
                if (layer.dirty)
                    return true;
            }

            return false;
        }

        private void OnWillRenderCanvases()
        {
            if (!IsLayerDirty())
                return;
            UpdateOrder(true);
        }

        void UpdateOrder(bool isNeedUpdateMask = false)
        {
            int layerIndex = 0, opaqueIndex = 0;
            if (m_Optimize)
            {
                opaqueIndex = -1;
                for (var i = m_Layers.Count - 1; i >= 0; i--)
                {
                    var layer = m_Layers[i];
                    if (opaqueIndex > -1)
                        layer.HideAllViews();
                    else
                    {
                        opaqueIndex = layer.HideBehindOpaquePanel();
                        if (opaqueIndex > -1)
                            layerIndex = i;
                    }
                }

                if (opaqueIndex == -1)
                    opaqueIndex = 0;
                if (Camera.main)
                    Camera.main.cullingMask = layerIndex > 0 ? 0 : m_LayerMask.value;
            }

            var order = 0;
            UIPanel maskPanel = null, transparentPanel = null;
            for (var i = layerIndex; i < m_Layers.Count; i++)
            {
                var layer = m_Layers[i];
                while (order > layer.BaseOrder)
                {
                    if (layer.BaseOrder <= 0)
                        layer.BaseOrder = 1;
                    layer.BaseOrder *= 2;
                }

                order = layer.OnWillRenderCanvases(i == layerIndex ? opaqueIndex : 0);
                if (layer.FrontedMaskPanel)
                    maskPanel = layer.FrontedMaskPanel;
                if (layer.FrontedTransparentMaskPanel)
                    transparentPanel = layer.FrontedTransparentMaskPanel;
            }

            if (isNeedUpdateMask)
                UpdateMask(maskPanel, transparentPanel);
        }

        private void UpdateMask(UIPanel maskPanel, UIPanel transparentPanel)
        {
            m_Mask.InitPanel(maskPanel);
            m_TransparentMask.InitPanel(transparentPanel);
        }

        public void BlockClick(bool isBlock)
        {
            Debug.Log("debugGuide UIRoot BlockClick:" + (isBlock ? "true" : "false"));
            m_Blocker.enabled = isBlock;
        }
    }
}