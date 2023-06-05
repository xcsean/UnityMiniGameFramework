using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityMiniGameFramework.UISystem
{
    public static class UIManager
    {
        public static UIRoot root { get; internal set; }

        [RuntimeInitializeOnLoadMethod]
        private static void UpdateDefaultMaterial()
        {
            var shader = Shader.Find("UI/Default");
            if (!shader) return;
            Canvas.GetDefaultCanvasMaterial().shader = shader;
        }

        internal static void OpenView(UIView view)
        {
            if (!root) throw new NullReferenceException("UIRoot is not initialized.");
            root.ShowView(view);
        }

        internal static void CloseView(UIView view)
        {
            if (!root) throw new NullReferenceException("UIRoot is not initialized.");
            root.HideView(view);
        }

        internal static void HideMask(UIView view)
        {
            if (!root) throw new NullReferenceException("UIRoot is not initialized.");
            root.HideMask(view);
        }

        public static UIView FindView(string viewName)
        {
            if (!root) throw new NullReferenceException("UIRoot is not initialized.");
            return root.FindView(viewName);
        }

        public static List<UIView> GetAllViews(UICanvasType canvasType)
        {
            if (!root) throw new NullReferenceException("UIRoot is not initialized.");
            return root.GetAllViews(canvasType);
        }
        
        public static void ShowAllViews()
        {
            if (!root) throw new NullReferenceException("UIRoot is not initialized.");
            root.ShowAllViews();
        }

        public static void HideAllViews()
        {
            if (!root) throw new NullReferenceException("UIRoot is not initialized.");
            root.HideAllViews();
        }

        public static void SetInteractable(bool isInteract)
        {
            if (!root) throw new NullReferenceException("UIRoot is not initialized.");
            root.SetInteractable(isInteract);
        }

        public static void DestroyAllViews(UICanvasType type = UICanvasType.Follow)
        {
            if (!root) throw new NullReferenceException("UIRoot is not initialized.");
            root.DestroyAllViews(type);
        }

        public static void HideAllLayer()
        {
            if (!root) throw new NullReferenceException("UIRoot is not initialized.");
            root.HideAllLayer();
        }
        public static void ShowAllLayer()
        {
            if (!root) throw new NullReferenceException("UIRoot is not initialized.");
            root.ShowAllLayer();
        }

        public static void HideCanvasLayer(UICanvasType canvasType)
        {
            if (!root) throw new NullReferenceException("UIRoot is not initialized.");
            root.HideCanvasLayer(canvasType);
        }

        public static void SetCanvasLayerHide(UICanvasType canvasType)
        {
            if (!root) throw new NullReferenceException("UIRoot is not initialized.");
            root.SetCanvasLayerHide(canvasType);
        }

        public static void SetCanvasLayerShow(UICanvasType canvasType)
        {
            if (!root) throw new NullReferenceException("UIRoot is not initialized.");
            root.SetCanvasLayerShow(canvasType);
        }
    }
}