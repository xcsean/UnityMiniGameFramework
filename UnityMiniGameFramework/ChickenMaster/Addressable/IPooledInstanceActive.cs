using System;
using UnityEngine;

namespace UnityMiniGameFramework.Addressable
{
    public interface IPooledInstanceActive : IPooledActive
    {
        void OnActiveChanged(bool active);
        
        Action<GameObject> OnDestroy { set; }
    }
}