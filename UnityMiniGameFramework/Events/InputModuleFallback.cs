using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace UnityMiniGameFramework
{
    public class InputModuleFallback : UIBehaviour
    {
        private static readonly List<InputModuleFallback> s_Modules = new List<InputModuleFallback>();

        protected override void OnEnable()
        {
            s_Modules.Add(this);
        }

        protected override void OnDisable()
        {
            s_Modules.Remove(this);
            if (s_Modules.Count > 0)
                SetFallbackTarget(s_Modules[s_Modules.Count - 1].gameObject);
        }

        private void SetFallbackTarget(GameObject go)
        {
            var eventSystem = EventSystem.current;
            if (!eventSystem) return;
            var inputModule = eventSystem.currentInputModule;
            if (inputModule is StandaloneFallbackInputModule module)
                module.FallbackTarget = go;
        }
    }
}