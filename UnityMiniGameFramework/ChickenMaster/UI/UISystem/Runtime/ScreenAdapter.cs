using System;
using UnityEngine;

namespace UnityMiniGameFramework.UISystem
{
    public class ScreenAdapter : MonoBehaviour
    {
        private static float s_ScreenRatio;
        private static event Action<float> s_OnScreenChanged;
        private static ScreenAdapter s_Instance;

        public static float ScreenRatio
        {
            get => s_ScreenRatio;
            private set
            {
                s_ScreenRatio = value;
                s_OnScreenChanged?.Invoke(s_ScreenRatio);
            }
        }

        public static event Action<float> OnScreenChanged
        {
            add
            {
                Init();
                s_OnScreenChanged += value;
                value?.Invoke(ScreenRatio);
            }
            remove => s_OnScreenChanged -= value;
        }

        private static void Init()
        {
            if (s_Instance || !Application.isPlaying) return;
            new GameObject(nameof(ScreenAdapter)).AddComponent<ScreenAdapter>();
        }

        private int m_Width;
        private int m_Height;

        private void Awake()
        {
            hideFlags = HideFlags.HideAndDontSave;
            if (!s_Instance)
            {
                s_Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        private void OnEnable()
        {
            SetDirty();
        }

        private void Update()
        {
            if (Screen.width == m_Width && Screen.height == m_Height)
                return;
            SetDirty();
        }

        private void SetDirty()
        {
            m_Width = Screen.width;
            m_Height = Screen.height;
            ScreenRatio = m_Width / (float) m_Height;
        }

        public static void ForceUpdate()
        {
            if (!s_Instance) return;
            s_Instance.SetDirty();
        }
    }
}