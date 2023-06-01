using UnityEngine.EventSystems;

namespace UnityMiniGameFramework
{
    public static class PinchExecuteEvents
    {
        private static readonly ExecuteEvents.EventFunction<IInitializePotentialPinchHandler>
            s_InitializePotentialPinchHandler = Execute;

        private static void Execute(IInitializePotentialPinchHandler handler, BaseEventData eventData)
        {
            handler.OnInitializePotentialPinch(ExecuteEvents.ValidateEventData<TouchEventData>(eventData));
        }

        private static readonly ExecuteEvents.EventFunction<IBeginPinchHandler> s_BeginPinchHandler = Execute;

        private static void Execute(IBeginPinchHandler handler, BaseEventData eventData)
        {
            handler.OnBeginPinch(ExecuteEvents.ValidateEventData<PinchEventData>(eventData));
        }

        private static readonly ExecuteEvents.EventFunction<IPinchHandler> s_PinchHandler = Execute;

        private static void Execute(IPinchHandler handler, BaseEventData eventData)
        {
            handler.OnPinch(ExecuteEvents.ValidateEventData<PinchEventData>(eventData));
        }

        private static readonly ExecuteEvents.EventFunction<IEndPinchHandler> s_EndPinchHandler = Execute;

        private static void Execute(IEndPinchHandler handler, BaseEventData eventData)
        {
            handler.OnEndPinch(ExecuteEvents.ValidateEventData<PinchEventData>(eventData));
        }

        public static ExecuteEvents.EventFunction<IInitializePotentialPinchHandler> initializePotentialPinchHandler =>
            s_InitializePotentialPinchHandler;

        public static ExecuteEvents.EventFunction<IBeginPinchHandler> beginPinchHandler => s_BeginPinchHandler;
        public static ExecuteEvents.EventFunction<IPinchHandler> pinchHandler => s_PinchHandler;
        public static ExecuteEvents.EventFunction<IEndPinchHandler> endPinchHandler => s_EndPinchHandler;
    }
}