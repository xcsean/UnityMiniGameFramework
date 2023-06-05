using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;


namespace UnityMiniGameFramework.UISystem
{
    using Scene = UnityEngine.SceneManagement.Scene;
    public static class SceneManager
    {
        class SceneInfo
        {
            public object path { get; set; }
            public bool isAdditive { get; set; }
            public bool hideAll { get; set; }

            public AsyncOperationHandle<SceneInstance> handle { get; private set; }

            private readonly HashSet<SceneInfo> m_HiddenScenes = new HashSet<SceneInfo>();
            private readonly List<GameObject> m_HiddenGameObjects = new List<GameObject>();

            public void Load()
            {
                handle = Addressables.LoadSceneAsync(path, isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
                handle.Completed += s_OnSceneLoaded;
                m_HiddenScenes.Clear();
                m_HiddenGameObjects.Clear();
            }

            public void Unload()
            {
                if (handle.IsValid() && handle.IsDone)
                {
                    Addressables.UnloadSceneAsync(handle);
                    handle = default;
                }
            }

            public void OnSceneLoad(SceneInfo scene)
            {
                if (!scene.hideAll) return;
                m_HiddenScenes.Add(scene);
                if (m_HiddenScenes.Count > 1)
                    return;
                if (!handle.IsValid() || !handle.IsDone)
                    return;
                handle.Result.Scene.GetRootGameObjects(s_RootGameObjects);
                m_HiddenGameObjects.Clear();
                foreach (var go in s_RootGameObjects)
                {
                    if (!go || !go.activeSelf)
                        continue;
                    m_HiddenGameObjects.Add(go);
                    go.SetActive(false);
                }
            }

            public void OnSceneExit(SceneInfo scene)
            {
                if (!scene.hideAll) return;
                m_HiddenScenes.Remove(scene);
                if (m_HiddenScenes.Count > 0)
                    return;
                foreach (var go in m_HiddenGameObjects)
                {
                    if (!go) continue;
                    go.SetActive(true);
                }

                m_HiddenGameObjects.Clear();
            }

            public override string ToString()
            {
                return $"path: {path}, isAdditive: {isAdditive}, hideAll: {hideAll}";
            }
        }

        private static readonly ObjectPool<SceneInfo> s_SceneInfoPool =
            new ObjectPool<SceneInfo>(info => info.path = null, info => info.Unload());

        private static readonly Action<AsyncOperationHandle<SceneInstance>> s_OnSceneLoaded = OnSceneLoaded;

        private static readonly Dictionary<object, List<SceneInfo>> s_AllScenes =
            new Dictionary<object, List<SceneInfo>>();

        private static readonly List<SceneInfo> s_WaitForLoadingScenes = new List<SceneInfo>();
        private static SceneInfo s_LoadingScene;
        private static readonly List<SceneInfo> s_LoadedScenes = new List<SceneInfo>();

        private static readonly List<GameObject> s_RootGameObjects = new List<GameObject>();
        private static readonly List<Transform> s_CachedTransforms = new List<Transform>();

        public static event Action<object> SceneLoading;
        public static event Action<object> SceneUnloading;
        public static event Action<bool, object, Scene, bool> SceneLoaded;

        public static bool IsLoading => s_LoadingScene != null;

        public static float Progress => IsLoading ? s_LoadingScene.handle.PercentComplete : 1;

        public static Scene LastScene { get; private set; }

        public static Camera MainCamera { get; private set; }

        static SceneManager()
        {
            Application.quitting += OnQuit;
            GameManager.onApplicationReset += OnReset;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (s_LoadingScene != null)
            {
                if (!s_LoadingScene.isAdditive)
                {
                    for (var i = s_LoadedScenes.Count - 1; i >= 0; i--)
                    {
                        var info = s_LoadedScenes[i];
                        s_LoadedScenes.RemoveAt(i);
                        SceneUnloadingEvent(info.path);
                        s_SceneInfoPool.Release(info);
                    }
                }
                else
                {
                    foreach (var info in s_LoadedScenes)
                        info.OnSceneLoad(s_LoadingScene);
                }
            }

            if (s_LoadingScene != null && s_LoadingScene.isAdditive && !s_LoadingScene.hideAll)
            {
                var mainCamera = GetSceneMainCamera(scene);
                if (mainCamera)
                {
                    if (MainCamera)
                    {
                        MainCamera.enabled = false;
                        mainCamera.cullingMask = MainCamera.cullingMask;
                    }

                    MainCamera = mainCamera;
                }
            }
            else MainCamera = Camera.main;

            LastScene = scene;
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);
        }

        public static bool ContainsScene(object levelPath) =>
            s_AllScenes.TryGetValue(levelPath, out var list) && list.Count > 0;

        public static bool TryEnterScene(object levelPath, bool isAdditive = false, bool hideAll = false)
        {
            if (ContainsScene(levelPath))
                return false;
            EnterScene(levelPath, isAdditive, hideAll);
            return true;
        }

        public static void EnterScene(object levelPath, bool isAdditive = false, bool hideAll = false)
        {
            if (!isAdditive)
            {
                foreach (var s in s_WaitForLoadingScenes)
                    s_SceneInfoPool.Release(s);
                s_WaitForLoadingScenes.Clear();
                foreach (var infos in s_AllScenes.Values)
                    infos.Clear();
            }

            var info = s_SceneInfoPool.Get();
            info.path = levelPath;
            info.isAdditive = isAdditive;
            info.hideAll = hideAll;

            s_WaitForLoadingScenes.Add(info);

            if (!s_AllScenes.TryGetValue(info.path, out var list))
            {
                list = new List<SceneInfo>();
                s_AllScenes.Add(info.path, list);
            }

            list.Add(info);

            if (!TryLoadScene()) return;
            Application.backgroundLoadingPriority = ThreadPriority.High;
            QualitySettings.asyncUploadTimeSlice = 8;
        }

        public static void LeaveScene(object levelPath)
        {
            if (!s_AllScenes.TryGetValue(levelPath, out var infos) || infos.Count == 0)
                return;
            var index = infos.Count - 1;
            var info = infos[index];
            if (!info.isAdditive)
            {
                Debug.LogWarning("Can not leave scene which is not additive.");
                return;
            }

            infos.RemoveAt(index);

            var releasable = false;
            if (!info.handle.IsValid())
            {
                if (s_WaitForLoadingScenes.Remove(info))
                    releasable = true;
            }
            else if (s_LoadedScenes.Remove(info))
                releasable = true;

            if (!releasable) return;
            s_SceneInfoPool.Release(info);
            foreach (var sceneInfo in s_LoadedScenes)
                sceneInfo.OnSceneExit(info);

            for (var i = s_LoadedScenes.Count - 1; i >= 0; i--)
            {
                var mainCamera = GetSceneMainCamera(s_LoadedScenes[i].handle.Result.Scene);
                if (mainCamera)
                {
                    if (MainCamera)
                    {
                        MainCamera.enabled = false;
                        mainCamera.cullingMask = MainCamera.cullingMask;
                    }

                    MainCamera = mainCamera;
                    MainCamera.enabled = true;
                    break;
                }
            }

            if (!MainCamera) MainCamera = Camera.main;
            SceneUnloadingEvent(info.path);
        }

        public static Scene GetLoadedScene(string levelPath)
        {
            return UnityEngine.SceneManagement.SceneManager.GetSceneByName(levelPath);
        }

        private static bool TryLoadScene()
        {
            if (s_WaitForLoadingScenes.Count > 0 && s_LoadingScene == null)
            {
                s_LoadingScene = s_WaitForLoadingScenes[0];
                s_WaitForLoadingScenes.RemoveAt(0);

                s_LoadingScene.Load();
                try
                {
                    SceneLoading?.Invoke(s_LoadingScene.path);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

                return true;
            }

            return false;
        }

        private static void OnSceneLoaded(AsyncOperationHandle<SceneInstance> handle)
        {
            if (handle.GetHashCode() != s_LoadingScene.handle.GetHashCode())
                return;
            if (s_AllScenes.TryGetValue(s_LoadingScene.path, out var list) && list.Contains(s_LoadingScene))
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                    s_LoadedScenes.Add(s_LoadingScene);
                try
                {
                    SceneLoaded?.Invoke(handle.Status == AsyncOperationStatus.Succeeded, s_LoadingScene.path,
                        handle.Result.Scene, s_LoadingScene.isAdditive);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            else if (handle.Status == AsyncOperationStatus.Succeeded)
                s_SceneInfoPool.Release(s_LoadingScene);

            s_LoadingScene = null;
            if (TryLoadScene())
                return;
            Application.backgroundLoadingPriority = ThreadPriority.Low;
            QualitySettings.asyncUploadTimeSlice = 2;
        }

        private static IEnumerator OnReset()
        {
            if (s_AllScenes.Count <= 0)
                yield break;
            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Empty");
        }

        private static void OnQuit()
        {
            SceneLoading = null;
            SceneUnloading = null;
            SceneLoaded = null;
            //NetHandler.Instance.Clear();
        }

        private static void SceneUnloadingEvent(object levelPath)
        {
            try
            {
                SceneUnloading?.Invoke(levelPath);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private static Camera GetSceneMainCamera(Scene scene)
        {
            scene.GetRootGameObjects(s_RootGameObjects);
            foreach (var go in s_RootGameObjects)
            {
                if (!go || !go.activeSelf)
                    continue;
                go.GetComponentsInChildren(s_CachedTransforms);
                foreach (var trans in s_CachedTransforms)
                {
                    if (trans.CompareTag("MainCamera"))
                        return trans.GetComponent<Camera>();
                }
            }

            return null;
        }
    }
}