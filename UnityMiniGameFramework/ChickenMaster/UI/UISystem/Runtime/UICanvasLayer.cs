using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityMiniGameFramework.UISystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class UICanvasLayer : UIBehaviour
    {
        [SerializeField]
        private UICanvasType m_CanvasType = UICanvasType.Auto;
        [SerializeField]
        private int m_BaseOrder;

        private CanvasGroup m_CanvasGroup;
        private readonly List<UIView> m_Views = new List<UIView>();
        internal bool dirty { get; set; }
        private int m_CachedOrder;

        public UICanvasType CanvasType => m_CanvasType;

        public int BaseOrder
        {
            get => m_BaseOrder;
            set
            {
                if (m_BaseOrder == value)
                    return;
                m_BaseOrder = value;
                dirty = true;
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

        public int CurrentOrder => m_BaseOrder + m_CachedOrder;

        internal UIPanel FrontedMaskPanel { get; private set; }

        internal UIPanel FrontedTransparentMaskPanel { get; private set; }

        internal void HideLayer()
        {
            CanvasGroup.alpha = 0;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
        }
        internal void ShowLayer()
        {
            CanvasGroup.alpha = 1;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
        }

        internal void SetInteractable(bool isInteract)
        {
            CanvasGroup.blocksRaycasts = isInteract;
            CanvasGroup.interactable = isInteract;
        }

        internal void ShowView(UIView view)
        {
            view.transform.SetParent(transform, false);
            m_Views.Add(view);
            view.Show();
            dirty = true;
        }

        internal void HideView(UIView view)
        {
            m_Views.Remove(view);
            view.Hide();
            dirty = true;
        }

        internal void DestroyAllViews(bool force = false)
        {
            for (var i = m_Views.Count - 1; i >= 0; i--)
            {
                var view = m_Views[i];
                if (!view.AutoClose && !force)
                    continue;
                view.Destroy();
            }
        }

        internal void HideAllViews()
        {
            foreach (var view in m_Views)
                view.Hide(true);
        }

        internal void ShowAllViews()
        {
            foreach (var view in m_Views)
                view.Show(true);
        }

        private int GetPanelOrder(int order)
        {
            if (order < 0) order = 0;
            order = (order + 1) * 2;
            if (order > m_CachedOrder)
                m_CachedOrder = order;
            return m_BaseOrder + order;
        }

        private void ResetCache()
        {
            m_CachedOrder = 0;
            FrontedMaskPanel = null;
            FrontedTransparentMaskPanel = null;
        }

        internal UIView FindView(string viewName)
        {
            foreach (var view in m_Views)
            {
                if (view.name.Contains(viewName))
                    return view;
            }

            return null;
        }

        internal List<UIView> GetAllViews()
        {
            return m_Views;
        }
        
        internal int HideBehindOpaquePanel()
        {
            var index = -1;
            for (var i = m_Views.Count - 1; i >= 0; i--)
            {
                var view = m_Views[i];
                if (i > index)
                {
                    view.Show(true);
                    if (view is UIPanel)
                    {
                        UIPanel panel = (UIPanel) view;
                        if(panel.Transparent)
                            index = i;
                    }
                        
                }
                else view.Hide(true);
            }

            return index;
        }

        internal int OnWillRenderCanvases(int start)
        {
            if (!dirty)
                return CurrentOrder;
            dirty = false;

            ResetCache();
            var index = 0;
            for (var i = start; i < m_Views.Count; i++)
            {
                var view = m_Views[i];
                if (!(view is UIPanel panel) || !view.IsVisible) continue;
                var order = m_CanvasType == UICanvasType.Fixed ? panel.Order : index;
                order = GetPanelOrder(order);
                panel.SortingOrder = order;

                if (panel.Transparent)
                {
                    switch (panel.MaskType)
                    {
                        case UIMaskType.Default:
                            FrontedMaskPanel = panel;
                            break;
                        case UIMaskType.Transparency:
                            FrontedTransparentMaskPanel = panel;
                            break;
                    }
                }

                index++;
            }

            return CurrentOrder;
        }
    }
}