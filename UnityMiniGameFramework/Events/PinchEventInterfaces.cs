using UnityEngine.EventSystems;

namespace UnityMiniGameFramework
{
    public interface IInitializePotentialPinchHandler : IEventSystemHandler
    {
        void OnInitializePotentialPinch(TouchEventData eventData);
    }

    public interface IBeginPinchHandler : IEventSystemHandler
    {
        void OnBeginPinch(PinchEventData eventData);
    }

    public interface IPinchHandler : IEventSystemHandler
    {
        void OnPinch(PinchEventData eventData);
    }

    public interface IEndPinchHandler : IEventSystemHandler
    {
        void OnEndPinch(PinchEventData eventData);
    }
}