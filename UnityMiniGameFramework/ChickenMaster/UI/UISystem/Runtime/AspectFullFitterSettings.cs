using UnityEngine;

namespace UnityMiniGameFramework.UISystem
{
    
    [CreateAssetMenu]
    public class AspectFullFitterSettings : ScriptableObject
    {
        [SerializeField]
        private float m_AspectRatio = 1;

        public float AspectRatio
        {
            get => m_AspectRatio;
            set
            {
                if (Mathf.Approximately(m_AspectRatio, value))
                    return;
                m_AspectRatio = value;
                ScreenAdapter.ForceUpdate();
            }
        }

        protected AspectFullFitterSettings()
        {
            s_Instance = this;
        }

        private static AspectFullFitterSettings s_Instance;

        public static AspectFullFitterSettings Instance
        {
            get
            {
                if (!s_Instance)
                {
                    Resources.Load<AspectFullFitterSettings>(nameof(AspectFullFitterSettings));
                    if (!s_Instance)
                        CreateInstance<AspectFullFitterSettings>().hideFlags =
                            HideFlags.HideAndDontSave;
                }
                return s_Instance;
            }
        }

#if UNITY_EDITOR
        private void OnValidate() => ScreenAdapter.ForceUpdate();
#endif
    }
}