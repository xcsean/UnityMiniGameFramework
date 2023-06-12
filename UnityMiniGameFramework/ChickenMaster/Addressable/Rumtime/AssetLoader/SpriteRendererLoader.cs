using UnityEngine;

namespace UnityMiniGameFramework.Addressable
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteRendererLoader : SpriteLoader
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

        protected override void OnComplete(Sprite result)
        {
            Sprite.sprite = result;
        }
    }
}