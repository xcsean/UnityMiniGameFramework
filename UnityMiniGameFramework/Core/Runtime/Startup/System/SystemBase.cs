using System;
using UnityEngine;

namespace UnityMiniGameFramework
{
    public abstract class SystemBase : ScriptableObject, IDisposable
    {
        internal protected GameManager manager { get; internal set; }
        
        public GameState state { get; internal set; }
        
        internal protected virtual void OnReset() { }

        internal protected virtual void OnStart() { }

        internal protected virtual void OnUpdate() { }

        internal protected virtual void OnLateUpdate() { }

        internal protected virtual void OnFixedUpdate() { }

        internal protected virtual void OnGUI() { }

        internal protected virtual void OnFocus(bool hasFocus) { }

        internal protected virtual void OnPause(bool pauseStatus) { }

        internal protected virtual void OnQuit() { }

        public void Dispose()
        {
            Destroy(this);
        }
    }
}