using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace UnityMiniGameFramework.UISystem
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Image))]
    public class ImageFilledWithSliced : UIBehaviour, UnityEngine.UI.IMeshModifier
    {
        static readonly Vector2[] s_VertScratch = new Vector2[4];
        static readonly Vector2[] s_UVScratch = new Vector2[4];
        private Image m_Image;

        public Image image
        {
            get
            {
                if (!m_Image)
                    m_Image = GetComponent<Image>();
                return m_Image;
            }
        }

        protected override void OnEnable()
        {
            image.SetVerticesDirty();
        }

        protected override void OnDisable()
        {
            image.SetVerticesDirty();
        }

        public void ModifyMesh(VertexHelper verts)
        {
            if (!IsActive()) return;
            if (image.type != Image.Type.Filled || (image.fillMethod != Image.FillMethod.Horizontal &&
                                                    image.fillMethod != Image.FillMethod.Vertical))
                return;
            if (!image.hasBorder) return;
            Vector4 outer = Vector4.zero, inner = Vector4.zero, padding = Vector4.zero, border = Vector4.zero;
            if (image.overrideSprite)
            {
                outer = DataUtility.GetOuterUV(image.overrideSprite);
                inner = DataUtility.GetInnerUV(image.overrideSprite);
                padding = DataUtility.GetPadding(image.overrideSprite);
                border = image.overrideSprite.border;
            }

            Rect rect = image.GetPixelAdjustedRect();
            border /= image.pixelsPerUnit;
            padding /= image.pixelsPerUnit;

            s_VertScratch[0] = new Vector2(padding.x, padding.y);
            s_VertScratch[3] = new Vector2(rect.width - padding.z, rect.height - padding.w);

            s_VertScratch[1].x = border.x;
            s_VertScratch[1].y = border.y;
            s_VertScratch[2].x = rect.width - border.z;
            s_VertScratch[2].y = rect.height - border.w;

            for (int i = 0; i < 4; i++)
            {
                s_VertScratch[i].x += rect.x;
                s_VertScratch[i].y += rect.y;
            }

            s_UVScratch[0] = new Vector2(outer.x, outer.y);
            s_UVScratch[1] = new Vector2(inner.x, inner.y);
            s_UVScratch[2] = new Vector2(inner.z, inner.w);
            s_UVScratch[3] = new Vector2(outer.z, outer.w);

            if (image.fillMethod == Image.FillMethod.Horizontal)
            {
                float fillMin = border.x / rect.width;
                float fillMax = border.z / rect.width;
                float fillCenter = inner.z - inner.x;
                if (image.fillOrigin == 1)
                {
                    float amountScratch = s_VertScratch[3].x - rect.width * image.fillAmount;
                    float fill = image.fillAmount;
                    if (fill > fillMax && fill < (1 - fillMin)) fill = inner.z + (fill - fillMax) * fillCenter;
                    else if (fill < fillMax) fill = fill / fillMax * (outer.z - inner.z);
                    else fill = (outer.z - inner.x) + (fill - 1 + fillMin) / fillMin * (inner.x - outer.x);
                    float amountUVScratch = outer.z - fill;
                    for (int i = 0; i < 3; i++)
                    {
                        if (s_VertScratch[i].x < amountScratch) s_VertScratch[i].x = amountScratch;
                        if (s_UVScratch[i].x < amountUVScratch) s_UVScratch[i].x = amountUVScratch;
                    }
                }
                else
                {
                    float amountScratch = s_VertScratch[0].x + rect.width * image.fillAmount;
                    float fill = image.fillAmount;
                    if (fill > fillMin && fill < (1 - fillMax)) fill = inner.x + (fill - fillMin) * fillCenter;
                    else if (fill < fillMin) fill = fill / fillMin * (inner.x - outer.x);
                    else fill = inner.z + (fill - 1 + fillMax) / fillMax * (outer.z - inner.z);
                    float amountUVScratch = outer.x + fill;
                    for (int i = 1; i < 4; i++)
                    {
                        if (s_VertScratch[i].x > amountScratch) s_VertScratch[i].x = amountScratch;
                        if (s_UVScratch[i].x > amountUVScratch) s_UVScratch[i].x = amountUVScratch;
                    }
                }
            }
            else
            {
                float fillMin = border.y / rect.height;
                float fillMax = border.w / rect.height;
                float fillCenter = inner.w - inner.y;
                if (image.fillOrigin == 1)
                {
                    float amountScratch = s_VertScratch[3].y - rect.height * image.fillAmount;
                    float fill = image.fillAmount;
                    if (fill > fillMax && fill < (1 - fillMin)) fill = inner.w + (fill - fillMax) * fillCenter;
                    else if (fill < fillMax) fill = fill / fillMax * (outer.w - inner.w);
                    else fill = (outer.w - outer.y) + (fill - 1 + fillMin) / fillMin * (inner.y - outer.y);
                    float amountUVScratch = outer.w - fill;
                    for (int i = 0; i < 3; i++)
                    {
                        if (s_VertScratch[i].y < amountScratch) s_VertScratch[i].y = amountScratch;
                        if (s_UVScratch[i].y < amountUVScratch) s_UVScratch[i].y = amountUVScratch;
                    }
                }
                else
                {
                    float amountScratch = s_VertScratch[0].y + rect.height * image.fillAmount;
                    float fill = image.fillAmount;
                    if (fill > fillMin && fill < (1 - fillMax)) fill = inner.y + (fill - fillMin) * fillCenter;
                    else if (fill < fillMin) fill = fill / fillMin * (inner.y - outer.y);
                    else fill = inner.w + (fill - 1 + fillMax) / fillMax * (outer.w - inner.w);
                    float amountUVScratch = outer.y + fill;
                    for (int i = 1; i < 4; i++)
                    {
                        if (s_VertScratch[i].y > amountScratch) s_VertScratch[i].y = amountScratch;
                        if (s_UVScratch[i].y > amountUVScratch) s_UVScratch[i].y = amountUVScratch;
                    }
                }
            }

            verts.Clear();
            for (int x = 0; x < 3; x++)
            {
                int x2 = x + 1;
                for (int y = 0; y < 3; y++)
                {
                    int y2 = y + 1;
                    AddQuad(verts,
                        new Vector2(s_VertScratch[x].x, s_VertScratch[y].y),
                        new Vector2(s_VertScratch[x2].x, s_VertScratch[y2].y),
                        image.color,
                        new Vector2(s_UVScratch[x].x, s_UVScratch[y].y),
                        new Vector2(s_UVScratch[x2].x, s_UVScratch[y2].y));
                }
            }
        }

        static void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uvMin,
            Vector2 uvMax)
        {
            int startIndex = vertexHelper.currentVertCount;

            vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0), color, new Vector2(uvMin.x, uvMin.y));
            vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0), color, new Vector2(uvMin.x, uvMax.y));
            vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0), color, new Vector2(uvMax.x, uvMax.y));
            vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0), color, new Vector2(uvMax.x, uvMin.y));

            vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
        }

        public void ModifyMesh(Mesh mesh)
        {
        }
    }
}