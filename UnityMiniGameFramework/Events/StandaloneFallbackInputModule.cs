using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityMiniGameFramework
{
    public class StandaloneFallbackInputModule : StandaloneInputModule
    {
        private static readonly List<Component> s_Components = new List<Component>();

        [SerializeField] private GameObject m_FallbackTarget;

        [NonSerialized] private Vector2 m_LastMousePosition;
        [NonSerialized] private Vector2 m_MousePosition;
        [NonSerialized] private PointerEventData m_InputPointerEvent;
        [NonSerialized] private readonly MouseState m_MouseState = new MouseState();
        [NonSerialized] private PinchEventData m_PinchEventData;

        public GameObject FallbackTarget
        {
            get => m_FallbackTarget;
            set => m_FallbackTarget = value;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_PinchEventData = new PinchEventData(eventSystem);
        }

        private bool ShouldIgnoreEventsOnNoFocus()
        {
#if UNITY_EDITOR
            return !UnityEditor.EditorApplication.isRemoteConnected;
#else
            return true;
#endif
        }

        public bool IsPointerOverGameObjectDraggable() =>
            IsPointerOverGameObjectDraggable(PointerInputModule.kMouseLeftId);

        public bool IsPointerOverGameObjectDraggable(int pointerId)
        {
            var lastPointer = GetLastPointerEventData(pointerId);
            if (lastPointer == null || !lastPointer.pointerEnter)
                return false;
            return ExecuteEvents.GetEventHandler<IDragHandler>(lastPointer.pointerEnter);
        }

        public GameObject GetLastRaycastResult() => GetLastRaycastResult(PointerInputModule.kMouseLeftId);

        public GameObject GetLastRaycastResult(int pointerId)
        {
            var lastPointer = GetLastPointerEventData(pointerId);
            return lastPointer?.pointerEnter;
        }

        public override bool ShouldActivateModule()
        {
            if (!enabled || !gameObject.activeInHierarchy)
                return false;

            return base.ShouldActivateModule() || (m_MousePosition - m_LastMousePosition).sqrMagnitude > 0;
        }

        public override void ActivateModule()
        {
            if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus())
                return;

            base.ActivateModule();
            m_MousePosition = input.mousePosition;
            m_LastMousePosition = input.mousePosition;
        }

        public override void UpdateModule()
        {
            if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus())
            {
                if (m_InputPointerEvent != null && m_InputPointerEvent.pointerDrag && m_InputPointerEvent.dragging)
                    ReleaseMouse(m_InputPointerEvent, m_InputPointerEvent.pointerCurrentRaycast.gameObject);

                m_InputPointerEvent = null;
                return;
            }

            m_LastMousePosition = m_MousePosition;
            m_MousePosition = input.mousePosition;
        }

        public override void Process()
        {
            if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus())
                return;

            var usedEvent = SendUpdateEventToSelectedObject();

            if (!ProcessTouchEvents() && input.mousePresent)
                ProcessMouseEvent();

            if (eventSystem.sendNavigationEvents)
            {
                if (!usedEvent)
                    usedEvent |= SendMoveEventToSelectedObject();

                if (!usedEvent)
                    SendSubmitEventToSelectedObject();
            }
        }

        private bool GetTouchData(int id, out TouchEventData data, bool create)
        {
            if (!m_PointerData.TryGetValue(id, out var eventData) && create)
            {
                eventData = new TouchEventData(eventSystem)
                {
                    pointerId = id,
                };
                m_PointerData.Add(id, eventData);
                data = (TouchEventData) eventData;
                return true;
            }

            data = eventData as TouchEventData;
            return false;
        }

        private bool ProcessTouchEvents()
        {
            m_PinchEventData.touches.Clear();
            for (var i = 0; i < input.touchCount; i++)
            {
                var touch = input.GetTouch(i);
                if (touch.type == TouchType.Indirect)
                    continue;
                var pointer = GetTouchPointerEventData(touch, out var pressed, out var released);

                ProcessTouchPress(pointer, pressed, released);

                if (!released)
                {
                    ProcessMove(pointer);
                    m_PinchEventData.touches.Add(pointer);
                }
                else
                    RemovePointerData(pointer);

                m_RaycastResultCache.Clear();
            }

            ProcessPinch(m_PinchEventData);
            foreach (var touch in m_PinchEventData.touches)
                ProcessDrag(touch);
            return input.touchCount > 0;
        }

        private void ProcessPinch(PinchEventData eventData)
        {
            if (eventData.Prepare())
            {
                if (eventData.pinchStart != null)
                    eventData.pinchStart.pressPosition = eventData.pinchStart.position;
                if (eventData.pinchEnd != null)
                    eventData.pinchEnd.pressPosition = eventData.pinchEnd.position;
            }

            if (!eventData.IsPointerMoving() || Cursor.lockState == CursorLockMode.Locked ||
                eventData.pointerPinch == null)
                return;

            // Before doing pinch we should cancel any drag state
            if (eventData.pinchStart.pointerDrag && eventData.pinchStart.dragging)
            {
                ExecuteEvents.Execute(eventData.pinchStart.pointerDrag, eventData.pinchStart,
                    ExecuteEvents.endDragHandler);
                eventData.pinchStart.dragging = false;
            }

            if (eventData.pinchEnd.pointerDrag && eventData.pinchEnd.dragging)
            {
                ExecuteEvents.Execute(eventData.pinchEnd.pointerDrag, eventData.pinchEnd,
                    ExecuteEvents.endDragHandler);
                eventData.pinchEnd.dragging = false;
            }

            if (!eventData.pinching && eventData.ShouldStartPinch(eventSystem.pixelDragThreshold))
            {
                ExecuteEvents.Execute(eventData.pointerPinch, eventData, PinchExecuteEvents.beginPinchHandler);
                eventData.pinching = true;
            }

            if (eventData.pinching)
            {
                if (eventData.pinchStart.pointerPress != eventData.pointerPinch)
                    ProcessPointerUp(eventData.pinchStart);
                if (eventData.pinchEnd.pointerPress != eventData.pointerPinch)
                    ProcessPointerUp(eventData.pinchEnd);
                eventData.CalculateDelta();
                ExecuteEvents.Execute(eventData.pointerPinch, eventData, PinchExecuteEvents.pinchHandler);
            }
        }

        private void ProcessPointerUp(PointerEventData pointerEvent)
        {
            ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
            pointerEvent.eligibleForClick = false;
            pointerEvent.pointerPress = null;
            pointerEvent.rawPointerPress = null;
        }

        private void ProcessDrag(TouchEventData pointerEvent)
        {
            if (!pointerEvent.IsPointerMoving() || Cursor.lockState == CursorLockMode.Locked ||
                pointerEvent.pointerDrag == null || pointerEvent.pinching ||
                pointerEvent.pointerDrag == m_PinchEventData.pointerPinch)
                return;

            if (!pointerEvent.dragging && PinchEventData.ShouldStartDrag(pointerEvent.pressPosition,
                pointerEvent.position, eventSystem.pixelDragThreshold, pointerEvent.useDragThreshold))
            {
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.beginDragHandler);
                pointerEvent.dragging = true;
            }

            if (pointerEvent.dragging)
            {
                if (pointerEvent.pointerPress != pointerEvent.pointerDrag)
                    ProcessPointerUp(pointerEvent);
            }

            ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.dragHandler);
        }

        private GameObject ExecuteHierarchyOrFallback<T>(PointerEventData eventData,
            ExecuteEvents.EventFunction<T> callbackFunction) where T : IEventSystemHandler
        {
            var target = GetEventHandlerOrFallback<T>(out var raycast);
            if (raycast.isValid) eventData.pointerCurrentRaycast = raycast;
            return ExecuteEvents.Execute(target, eventData, callbackFunction) ? target : null;
        }

        private static bool IsHandlerCanIgnore<T>(GameObject target) where T : IEventSystemHandler
        {
            target.GetComponents(s_Components);
            foreach (var component in s_Components)
            {
                if (!(component is T))
                    continue;
                if (!(component is IEventIgnoreHandler handler) || !handler.CanIgnore<T>())
                    return false;
            }

            return true;
        }

        private GameObject GetEventHandlerOrFallback<T>(out RaycastResult result) where T : IEventSystemHandler
        {
            result = default;
            foreach (var raycast in m_RaycastResultCache)
            {
                var target = ExecuteEvents.GetEventHandler<T>(raycast.gameObject);
                if (target) target.GetComponents(s_Components);
                while (target && IsHandlerCanIgnore<T>(target))
                {
                    var parent = target.transform.parent;
                    target = parent ? parent.gameObject : null;
                    target = ExecuteEvents.GetEventHandler<T>(target);
                }

                if (!target) continue;
                result = raycast;
                return target;
            }

            return ExecuteEvents.GetEventHandler<T>(m_FallbackTarget);
        }

        private GameObject GetEventHandlerOrFallback<T>() where T : IEventSystemHandler =>
            GetEventHandlerOrFallback<T>(out _);

        private void ProcessTouchPress(TouchEventData pointerEvent, bool pressed, bool released)
        {
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

            if (pressed)
            {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                DeselectIfSelectionChanged(pointerEvent);

                if (pointerEvent.pointerEnter != currentOverGo)
                {
                    HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                    pointerEvent.pointerEnter = currentOverGo;
                }

                var newPressed = ExecuteHierarchyOrFallback(pointerEvent, ExecuteEvents.pointerDownHandler);
                var newClick = GetEventHandlerOrFallback<IPointerClickHandler>();

                if (!newPressed)
                    newPressed = newClick;

                var time = Time.unscaledTime;

                if (newPressed == pointerEvent.lastPress)
                {
                    var diffTime = time - pointerEvent.clickTime;
                    if (diffTime < 0.3f)
                        ++pointerEvent.clickCount;
                    else
                        pointerEvent.clickCount = 1;

                    pointerEvent.clickTime = time;
                }
                else
                {
                    pointerEvent.clickCount = 1;
                }

                pointerEvent.pointerPress = newPressed;
                pointerEvent.rawPointerPress = currentOverGo;
                pointerEvent.pointerClick = newClick;

                pointerEvent.clickTime = time;

                pointerEvent.pointerDrag = GetEventHandlerOrFallback<IDragHandler>();
                if (pointerEvent.pointerDrag)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent,
                        ExecuteEvents.initializePotentialDrag);

                pointerEvent.pointerPinch = GetEventHandlerOrFallback<IPinchHandler>();
                if (pointerEvent.pointerPinch)
                    ExecuteEvents.Execute(pointerEvent.pointerPinch, pointerEvent,
                        PinchExecuteEvents.initializePotentialPinchHandler);
            }

            if (released)
            {
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                var pointerClickHandler = GetEventHandlerOrFallback<IPointerClickHandler>();
                if (pointerEvent.pointerClick == pointerClickHandler && pointerEvent.eligibleForClick)
                    ExecuteEvents.Execute(pointerEvent.pointerClick, pointerEvent, ExecuteEvents.pointerClickHandler);

                if (pointerEvent.pointerDrag && pointerEvent.dragging)
                    ExecuteHierarchyOrFallback(pointerEvent, ExecuteEvents.dropHandler);

                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;
                pointerEvent.pointerClick = null;

                if (pointerEvent.pointerDrag && pointerEvent.dragging)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;

                if (m_PinchEventData.HasPinch(pointerEvent) && m_PinchEventData.pointerPinch &&
                    m_PinchEventData.pinching)
                {
                    ExecuteEvents.Execute(m_PinchEventData.pointerPinch, m_PinchEventData,
                        PinchExecuteEvents.endPinchHandler);
                    m_PinchEventData.pinching = false;
                }

                ExecuteEvents.Execute(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
                pointerEvent.pointerEnter = null;
            }

            m_InputPointerEvent = pointerEvent;
        }

        private new TouchEventData GetTouchPointerEventData(Touch input, out bool pressed, out bool released)
        {
            var created = GetTouchData(input.fingerId, out var pointerData, true);
            pointerData.Reset();

            pressed = created || input.phase == TouchPhase.Began;
            released = input.phase == TouchPhase.Canceled || input.phase == TouchPhase.Ended;

            if (created)
                pointerData.position = input.position;

            if (pressed)
                pointerData.delta = Vector2.zero;
            else
                pointerData.delta = input.position - pointerData.position;

            pointerData.position = input.position;
            pointerData.button = PointerEventData.InputButton.Left;

            if (input.phase == TouchPhase.Canceled)
                pointerData.pointerCurrentRaycast = new RaycastResult();
            else
            {
                eventSystem.RaycastAll(pointerData, m_RaycastResultCache);
                var raycast = FindFirstRaycast(m_RaycastResultCache);
                pointerData.pointerCurrentRaycast = raycast;
            }

            return pointerData;
        }

        protected override MouseState GetMousePointerEventData(int id)
        {
            var created = GetPointerData(kMouseLeftId, out var leftData, true);
            leftData.Reset();

            if (created)
                leftData.position = input.mousePosition;

            var pos = input.mousePosition;
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                leftData.position = new Vector2(-1.0f, -1.0f);
                leftData.delta = Vector2.zero;
            }
            else
            {
                leftData.delta = pos - leftData.position;
                leftData.position = pos;
            }

            leftData.scrollDelta = input.mouseScrollDelta;
            leftData.button = PointerEventData.InputButton.Left;
            eventSystem.RaycastAll(leftData, m_RaycastResultCache);
            var raycast = FindFirstRaycast(m_RaycastResultCache);
            leftData.pointerCurrentRaycast = raycast;

            GetPointerData(kMouseRightId, out var rightData, true);
            rightData.Reset();

            CopyFromTo(leftData, rightData);
            rightData.button = PointerEventData.InputButton.Right;

            GetPointerData(kMouseMiddleId, out var middleData, true);
            middleData.Reset();

            CopyFromTo(leftData, middleData);
            middleData.button = PointerEventData.InputButton.Middle;

            m_MouseState.SetButtonState(PointerEventData.InputButton.Left, StateForMouseButton(0), leftData);
            m_MouseState.SetButtonState(PointerEventData.InputButton.Right, StateForMouseButton(1), rightData);
            m_MouseState.SetButtonState(PointerEventData.InputButton.Middle, StateForMouseButton(2), middleData);

            return m_MouseState;
        }

        private new void ProcessMouseEvent(int id = 0)
        {
            var mouseData = GetMousePointerEventData(id);
            var leftButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData;

            ProcessMousePress(leftButtonData);
            ProcessMove(leftButtonData.buttonData);
            ProcessDrag(leftButtonData.buttonData);

            ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData);
            ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
            ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
            ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);

            if (!Mathf.Approximately(leftButtonData.buttonData.scrollDelta.sqrMagnitude, 0.0f) &&
                leftButtonData.buttonData.position.x >= 0 && leftButtonData.buttonData.position.x <= Screen.width &&
                leftButtonData.buttonData.position.y >= 0 && leftButtonData.buttonData.position.y <= Screen.height)
                ExecuteHierarchyOrFallback(leftButtonData.buttonData, ExecuteEvents.scrollHandler);
            m_RaycastResultCache.Clear();
        }

        private new void ProcessMousePress(MouseButtonEventData data)
        {
            var pointerEvent = data.buttonData;
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

            if (data.PressedThisFrame())
            {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                DeselectIfSelectionChanged(pointerEvent);

                var newPressed = ExecuteHierarchyOrFallback(pointerEvent, ExecuteEvents.pointerDownHandler);
                var newClick = GetEventHandlerOrFallback<IPointerClickHandler>();

                if (!newPressed)
                    newPressed = newClick;

                var time = Time.unscaledTime;

                if (newPressed == pointerEvent.lastPress)
                {
                    var diffTime = time - pointerEvent.clickTime;
                    if (diffTime < 0.3f)
                        ++pointerEvent.clickCount;
                    else
                        pointerEvent.clickCount = 1;

                    pointerEvent.clickTime = time;
                }
                else
                {
                    pointerEvent.clickCount = 1;
                }

                pointerEvent.pointerPress = newPressed;
                pointerEvent.rawPointerPress = currentOverGo;
                pointerEvent.pointerClick = newClick;

                pointerEvent.clickTime = time;

                pointerEvent.pointerDrag = GetEventHandlerOrFallback<IDragHandler>();
                if (pointerEvent.pointerDrag)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent,
                        ExecuteEvents.initializePotentialDrag);

                m_InputPointerEvent = pointerEvent;
            }

            if (data.ReleasedThisFrame())
            {
                ReleaseMouse(pointerEvent, currentOverGo);
            }
        }

        private void ReleaseMouse(PointerEventData pointerEvent, GameObject currentOverGo)
        {
            ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

            var pointerClickHandler = GetEventHandlerOrFallback<IPointerClickHandler>();
            if (pointerEvent.pointerClick == pointerClickHandler && pointerEvent.eligibleForClick)
                ExecuteEvents.Execute(pointerEvent.pointerClick, pointerEvent, ExecuteEvents.pointerClickHandler);

            if (pointerEvent.pointerDrag && pointerEvent.dragging)
                ExecuteHierarchyOrFallback(pointerEvent, ExecuteEvents.dropHandler);

            pointerEvent.eligibleForClick = false;
            pointerEvent.pointerPress = null;
            pointerEvent.rawPointerPress = null;
            pointerEvent.pointerClick = null;

            if (pointerEvent.pointerDrag && pointerEvent.dragging)
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

            pointerEvent.dragging = false;
            pointerEvent.pointerDrag = null;

            if (currentOverGo != pointerEvent.pointerEnter)
            {
                HandlePointerExitAndEnter(pointerEvent, null);
                HandlePointerExitAndEnter(pointerEvent, currentOverGo);
            }

            m_InputPointerEvent = pointerEvent;
        }

        private void DeselectIfSelectionChanged(BaseEventData pointerEvent)
        {
            // Selection tracking
            var selectHandlerGO = GetEventHandlerOrFallback<ISelectHandler>();
            // if we have clicked something new, deselect the old thing
            // leave 'selection handling' up to the press event though.
            var target = selectHandlerGO;
            while (target)
            {
                if (target == eventSystem.currentSelectedGameObject)
                    return;
                var parent = target.transform.parent;
                target = parent ? parent.gameObject : null;
            }

            eventSystem.SetSelectedGameObject(null, pointerEvent);
        }
    }
}