using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityMiniGameFramework.Addressable
{
    [RequireComponent(typeof(RawImage))]
    public class RawImageLoader : AssetLoader<Texture2D>, UnityEngine.UI.IMeshModifier
    {
        [SerializeField] private Texture2D m_DefaultTex;
        [SerializeField] private bool m_Resizable;

        public Texture2D DefaultTex
        {
            get => m_DefaultTex;
            set => m_DefaultTex = value;
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

        [NonSerialized] private RawImage m_RawImage;

        public RawImage RawImage
        {
            get
            {
                if (!m_RawImage)
                    m_RawImage = GetComponent<RawImage>();
                return m_RawImage;
            }
        }

        protected override void OnComplete(Texture2D result)
        {
            RawImage.texture = result;
            SetNativeSize();
        }

        protected override void OnFailed() => OnComplete(m_DefaultTex);

        protected override void OnLoadBegin()
        {
            if (isNeedReset)
                RawImage.texture = null;
        }

        protected override void OnUnloaded() => OnComplete(null);

        private void SetNativeSize()
        {
            if (!m_Resizable)
                return;
            RawImage.SetNativeSize();
        }

        public void ModifyMesh(Mesh mesh)
        {
            if (!RawImage.texture)
                mesh.Clear();
        }

        public void ModifyMesh(VertexHelper verts)
        {
            if (!RawImage.texture)
                verts.Clear();
        }
    }
}