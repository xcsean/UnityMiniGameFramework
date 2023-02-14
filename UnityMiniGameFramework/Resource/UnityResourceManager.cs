using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MiniGameFramework;
using UnityEngine.U2D;

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
            if (obj == null)
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error,
                    $"LoadUnityPrefabObject {prefabName} not exist");
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

        /// <summary>
        /// 加载UXML文件
        /// </summary>
        public UnityEngine.UIElements.VisualTreeAsset LoadUXML(string uxmlName)
        {
            var uxml = Resources.Load(uxmlName) as UnityEngine.UIElements.VisualTreeAsset;
            if (uxml == null)
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"LoadUXML {uxmlName} not exist");
                return null;
            }

            return uxml;
        }


        public Sprite LoadSprite(string spritePath)
        {
            var sp = Resources.Load<Sprite>(spritePath);
            if (sp == null)
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"LoadSprite {spritePath} not exist");
                return null;
            }

            return sp;
        }

        public Sprite LoadSpriteByAtlas(string spriteName, string atlasPath)
        {
            SpriteAtlas atlas = Resources.Load<SpriteAtlas>(atlasPath);
            if (atlas == null)
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"LoadSpriteAtlas {atlasPath} not exist");
                return null;
            }

            Sprite sprite = atlas.GetSprite(spriteName);
            if (sprite == null)
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"LoadSprite {spriteName} not exist");
                return null;
            }

            return sprite;
        }

        public Font LoadFont(string path)
        {
            Font font = Resources.Load<Font>(path);
            if (font == null)
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"LoadFont {path} not exist");
                return null;
            }

            return font;
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

        public AudioClip LoadAudioClip(string path)
        {
            var clip = Resources.Load<AudioClip>(path);
            if (clip == null)
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"LoadAudioClip {path} not exist");
                return null;
            }

            return clip;
        }
    }
}