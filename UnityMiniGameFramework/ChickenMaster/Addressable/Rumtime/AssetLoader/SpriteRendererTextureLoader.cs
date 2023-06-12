using System;
using UnityEngine;

namespace UnityMiniGameFramework.Addressable
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteRendererTextureLoader : AssetLoader<Texture2D>
    {
        private SpriteRenderer m_sprite;

        public SpriteRenderer Sprite
        {
            get
            {
                if (!m_sprite)
                    m_sprite = GetComponent<SpriteRenderer>();
                return m_sprite;
            }
        }

        [SerializeField] private Texture2D m_DefaultTex;

        [NonSerialized] private Sprite m_Sprite;

        protected override void OnComplete(Texture2D result)
        {
            m_Sprite = Convert2Sprite(result);
            Sprite.sprite = m_Sprite;
        }

        private Sprite Convert2Sprite(Texture2D texture2D)
        {
            DestroySprite();
            if (texture2D)
                return UnityEngine.Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height),
                    Vector2.one * 0.5f);
            else
                return null;
        }

        protected override void OnFailed() => OnComplete(m_DefaultTex);

        protected override void OnUnloaded()
        {
            DestroySprite();
            Sprite.sprite = null;
        }

        private void DestroySprite()
        {
            if (!m_sprite) return;
            if (Application.isPlaying)
                Destroy(m_Sprite);
            else
                DestroyImmediate(m_Sprite);
            m_sprite = null;
        }
    }
}