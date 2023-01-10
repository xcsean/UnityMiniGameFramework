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

        /// <summary>
        /// 加载USS文件
        /// </summary>
        public UnityEngine.UIElements.StyleSheet LoadStyleSheet(string ussName)
        {
            var uss = Resources.Load("Uss/" + ussName) as UnityEngine.UIElements.StyleSheet;
            if (uss == null)
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"LoadStyleSheet {ussName} not exist");
                return null;
            }
            return uss;
        }

        public Texture2D LoadTexture(string iconName)
        {
            var t2d = Resources.Load(iconName) as Texture2D;
            if (t2d == null)
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"LoadTexture {iconName} not exist");
                return null;
            }
            return t2d;
        }

        public Texture2D LoadProductIcon(string iconName)
        {
            string path = $"icons/products/{iconName}";
            return LoadTexture(path);
        }
    }
}
