using UnityEngine;

namespace UnityMiniGameFramework
{
    public interface IMapLogicObject
    {
        Vector2Int LogicPos { set; get; }
        void UpdateUnityGoPos(Vector3 pos);
    }
}