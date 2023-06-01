using UnityEngine.EventSystems;
namespace UnityMiniGameFramework
{
    public interface IEventIgnoreHandler
    {
        bool CanIgnore<T>() where T : IEventSystemHandler;
    }
}