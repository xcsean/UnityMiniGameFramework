using UnityEngine;
using UnityEngine.UI;

namespace UnityMiniGameFramework.Addressable
{
    [RequireComponent(typeof(Image))]
    public class ImageLoader : SpriteLoader, UnityEngine.UI.IMeshModifier
    {
        private Image m_Image;

        [SerializeField] private bool m_Resizable;

        public Image Image
        {
            get
            {
                if (!m_Image)
                    m_Image = GetComponent<Image>();
                return m_Image;
            }
        }

        public bool Resizable
        {
            get => m_Resizable;
            set
            {
                if (m_Resizable == value)
                    return;
                m_Resizable = value;
                SetNativeSize();
            }
        }

        protected override void OnComplete(Sprite result)
        {
            Image.sprite = result;
            SetNativeSize();
        }

        private void SetNativeSize()
        {
            if (!m_Resizable)
                return;
            Image.SetNativeSize();
        }

        public void ModifyMesh(Mesh mesh)
        {
            if (!Image.sprite)
                mesh.Clear();
        }

        public void ModifyMesh(VertexHelper verts)
        {
            if (!Image.sprite)
                verts.Clear();
        }
    }
}