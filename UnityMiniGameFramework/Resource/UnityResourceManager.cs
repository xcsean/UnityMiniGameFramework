using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class UnityResourceManager : ResourceManager
    {

        override public void Init()
        {

        }


        public UnityEngine.GameObject LoadUnityPrefabObject(string prefabName)
        {
            var obj = Resources.Load(prefabName) as UnityEngine.GameObject;
            if(obj == null)
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"LoadUnityPrefabObject {prefabName} not exist");
                return null;
            }
            return obj;
        }

        public void ReleaseUnityPrefabObject(UnityEngine.GameObject o)
        {
            Resources.UnloadAsset(o);
        }
    }
}
