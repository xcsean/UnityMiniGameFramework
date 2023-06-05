using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace UnityMiniGameFramework
{
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        
        [SerializeField]
        private string m_SettingsPath;

        private readonly List<SystemStartup> m_SystemStartups = new List<SystemStartup>();
        [SerializeField, HideInInspector]
        private List<SystemBase> m_Systems = new List<SystemBase>();
        [SerializeField, HideInInspector]
        private List<ProcedureBase> m_Procedures = new List<ProcedureBase>();

        private static readonly List<Func<IEnumerator>> s_ResetCallbacks = new List<Func<IEnumerator>>();

        public static event UnityAction<bool> onApplicationFocus;
        public static event UnityAction<bool> onApplicationPause;
        public static event UnityAction onApplicationQuit;
        public static event Func<IEnumerator> onApplicationReset
        {
            add => s_ResetCallbacks.Add(value);
            remove => s_ResetCallbacks.Remove(value);
        }

        public GameState state { get; private set; }

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitSettings();
            InitStartups();
            ChangeState(GameState.Awake);
        }
        

        private void InitSettings()
        {
            SystemSettings settings = Resources.Load<SystemSettings>(m_SettingsPath);
            settings?.Apply();
            Resources.UnloadAsset(settings);
        }

        private void InitStartups()
        {
            StartupBase[] startups = Resources.LoadAll<StartupBase>(m_SettingsPath);
            Array.Sort(startups, StartupBase.startupComparer);
            for (int i = 0; i < startups.Length; i++)
            {
                StartupBase startup = startups[i];
                if (startup is SystemStartup)
                {
                    var systemStartup = startup as SystemStartup;
                    if (systemStartup.wakeupState != GameState.None)
                        m_SystemStartups.Add(systemStartup);
                }
                else if (startup is ProcedureBase)
                    m_Procedures.Add(startup as ProcedureBase);
            }
        }

        private void ChangeState(GameState state)
        {
            this.state = state;
            for (int i = 0; i < m_SystemStartups.Count;)
            {
                SystemStartup startup = m_SystemStartups[i];
                try
                {
                    if (startup.Wakeup(this, out SystemBase system))
                    {
                        if (system) m_Systems.Add(system);
                        m_SystemStartups.RemoveAt(i);
                        Resources.UnloadAsset(startup);
                    }
                    else ++i;
                }
                catch (Exception ex)
                {
                    m_SystemStartups.RemoveAt(i);
                    Debug.LogErrorFormat("{0} wakeup error: {1}", startup.name, ex);
                }
            }
        }

        private IEnumerator Start()
        {
            for (int i = 0; i < m_Procedures.Count;)
            {
                ProcedureBase procedure = m_Procedures[i];
                yield return procedure.Execute(this);
                m_Procedures.RemoveAt(i);
                Resources.UnloadAsset(procedure);
            }
            ChangeState(GameState.Game);
        }

        private void Update()
        {
            for (int i = 0; i < m_Systems.Count; i++)
            {
                try
                {
                    m_Systems[i].OnUpdate();
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < m_Systems.Count; i++)
            {
                try
                {
                    m_Systems[i].OnLateUpdate();
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < m_Systems.Count; i++)
            {
                try
                {
                    m_Systems[i].OnFixedUpdate();
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }

        private void OnGUI()
        {
            for (int i = 0; i < m_Systems.Count; i++)
            {
                try
                {
                    m_Systems[i].OnGUI();
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            for (int i = 0; i < m_Systems.Count; i++)
            {
                try
                {
                    m_Systems[i].OnFocus(hasFocus);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            onApplicationFocus?.Invoke(hasFocus);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            for (int i = 0; i < m_Systems.Count; i++)
            {
                try
                {
                    m_Systems[i].OnPause(pauseStatus);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            onApplicationPause?.Invoke(pauseStatus);
        }

        private void OnApplicationQuit()
        {
            for (int i = 0; i < m_Systems.Count; i++)
            {
                try
                {
                    m_Systems[i].OnQuit();
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            onApplicationQuit?.Invoke();
        }

        private void OnDestroy()
        {
            for (int i = 0; i < m_Systems.Count; i++)
            {
                try
                {
                    m_Systems[i].Dispose();
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            m_Systems.Clear();
            Instance = null;
        }

        public static void ResetGame() => Instance.StartCoroutine(Instance.DoReset());

        private IEnumerator DoReset()
        {
            foreach (var system in m_Systems)
            {
                if (system.state != GameState.Awake)
                    continue;
                try
                {
                    system.OnReset();
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }

            foreach (var callback in s_ResetCallbacks)
            {
                IEnumerator coroutine = null;
                try
                {
                    coroutine = callback?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
                if (coroutine == null)
                    continue;
                yield return coroutine;
            }

            foreach (var system in m_Systems)
            {
                if (system.state != GameState.Game)
                    continue;
                try
                {
                    system.OnReset();
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }

        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}