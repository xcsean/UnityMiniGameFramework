using UnityEngine;

namespace UnityMiniGameFramework
{
    public class BuildMeshRendererAcquirer : MonoBehaviour
    {
        [SerializeField] private Renderer buildMeshRender;

        public Renderer BuildMeshRender => buildMeshRender;
    }
}