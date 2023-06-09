using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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

        private static readonly Action<AsyncOperationHandle<GameObject>> s_InstantiatedAction = OnInstantiated;

        private static readonly Dictionary<AsyncOperationHandle<GameObject>, string>
            s_CachedHandles = new Dictionary<AsyncOperationHandle<GameObject>, string>();

        private static readonly Dictionary<int, int> s_InstanceLayer = new Dictionary<int, int>();
        private static readonly List<Transform> s_Transforms = new List<Transform>();

        private static bool s_HiddenInitialized;
        private static int s_HiddenLayer;

        public static void Create(string address, Transform parent = null)
        {
            try
            {
                var handle = Addressables.InstantiateAsync(address, parent);
                if (!handle.IsValid())
                    throw new ArgumentNullException("The target res address is not valid.");
                s_CachedHandles.Add(handle, address);
                handle.Completed += s_InstantiatedAction;
                if (handle.IsDone && handle.Status == AsyncOperationStatus.Succeeded && handle.Result)
                    SwitchVisible(handle.Result, false);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        private static void OnInstantiated(AsyncOperationHandle<GameObject> handle)
        {
            if (!s_CachedHandles.TryGetValue(handle, out var cache))
                return;
            s_CachedHandles.Remove(handle);
            try
            {
                if (handle.Status != AsyncOperationStatus.Succeeded)
                    throw handle.OperationException;
                var gameObject = handle.Result;
                if (!gameObject)
                    throw new NullReferenceException($"Instantiated gameObject is null.");
                SwitchVisible(handle.Result, true);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        private static int GetHiddenLayer()
        {
            if (s_HiddenInitialized)
                return s_HiddenLayer;
            s_HiddenInitialized = true;
            s_HiddenLayer = LayerMask.NameToLayer("Hidden");
            return s_HiddenLayer;
        }

        private static void SwitchVisible(GameObject instance, bool visible)
        {
            if (!instance) return;
            if (instance.TryGetComponent(out CanvasGroup group))
            {
                group.alpha = visible ? 1 : 0;
                group.blocksRaycasts = visible;
            }
            else
            {
                if (!visible)
                {
                    s_InstanceLayer[instance.GetInstanceID()] = instance.layer;
                    instance.GetComponentsInChildren(true, s_Transforms);
                    foreach (var trans in s_Transforms)
                        trans.gameObject.layer = GetHiddenLayer();
                }
                else if (s_InstanceLayer.TryGetValue(instance.GetInstanceID(), out var layer))
                {
                    s_InstanceLayer.Remove(instance.GetInstanceID());
                    instance.GetComponentsInChildren(true, s_Transforms);
                    foreach (var trans in s_Transforms)
                        trans.gameObject.layer = layer;
                }
            }
        }
    }
}