using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UnityMiniGameFramework.Addressable
{
    public abstract class SpriteLoader : AssetLoader<Sprite>
    {
        [SerializeField] private Sprite m_DefaultSprite;

        public Sprite DefaultSprite => m_DefaultSprite;

        [SerializeField] private AssetReferenceSprite m_ReferenceSprite = new AssetReferenceSprite(null);

        public override AssetReferenceT<Sprite> Reference
        {
            get => m_ReferenceSprite;
            set => m_ReferenceSprite = new AssetReferenceSprite(value.AssetGUID);
        }

        protected override void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && !string.IsNullOrEmpty(Reference.AssetGUID) && !string.IsNullOrEmpty(Reference.SubObjectName))
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(Reference.AssetGUID);
                var assets = UnityEditor.AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
                OnComplete(assets.FirstOrDefault(asset => asset.name == Reference.SubObjectName) as Sprite);
            }
            else
#endif
            base.OnEnable();
        }

        protected override void OnUnloaded() => OnComplete(null);

        protected override void OnFailed() => OnComplete(m_DefaultSprite);
    }
}