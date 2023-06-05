using UnityEngine;

namespace UnityMiniGameFramework
{
    public sealed class SystemSettings : ScriptableObject
    {
        [SerializeField]
        private bool m_RunInBackground = true;
        [SerializeField]
        private int m_ScreenSleepTime = SleepTimeout.NeverSleep;
        [SerializeField]
        private int m_TargetFrameRate = 60;
        [SerializeField]
        private bool m_AutoPhysics = true;

        public void Apply()
        {
            Application.runInBackground = m_RunInBackground;
            Screen.sleepTimeout = m_ScreenSleepTime;
            Application.targetFrameRate = m_TargetFrameRate;
            Physics.autoSimulation = m_AutoPhysics;
            Physics.autoSyncTransforms = !m_AutoPhysics;
        }
    }
}