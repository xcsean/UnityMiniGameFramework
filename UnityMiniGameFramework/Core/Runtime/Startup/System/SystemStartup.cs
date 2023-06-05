using UnityEngine;

namespace UnityMiniGameFramework
{
    public abstract class SystemStartup : StartupBase
    {
        [SerializeField]
        private GameState m_WakeupState;

        public GameState wakeupState
        {
            get { return m_WakeupState; }
            set { m_WakeupState = value; }
        }

        internal bool Wakeup(GameManager manager, out SystemBase system)
        {
            if (m_WakeupState == manager.state)
            {
                system = CreateSystem(manager);
                if (system)
                {
                    system.hideFlags = HideFlags.HideAndDontSave;
                    system.manager = manager;
                    system.state = m_WakeupState;
                    system.OnStart();
                }
                return true;
            }
            system = null;
            return false;
        }

        protected abstract SystemBase CreateSystem(GameManager manager);
    }
}