using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityMiniGameFramework
{
    public class EventTrigger : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler,
        IInitializePotentialDragHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IDropHandler,
        IScrollHandler,
        IUpdateSelectedHandler,
        ISelectHandler,
        IDeselectHandler,
        IMoveHandler,
        ISubmitHandler,
        ICancelHandler,
        IInitializePotentialPinchHandler,
        IBeginPinchHandler,
        IPinchHandler,
        IEndPinchHandler,
        IEventIgnoreHandler
    {
        [Serializable]
        public class Entry
        {
            public EventTriggerType eventID = EventTriggerType.PointerClick;
            public UnityEngine.EventSystems.EventTrigger.TriggerEvent callback = new UnityEngine.EventSystems.EventTrigger.TriggerEvent();
        }

        private static readonly Dictionary<Type, EventTriggerType> s_TriggerTypeMappings =
            new Dictionary<Type, EventTriggerType>
            {
                {typeof(IPointerEnterHandler), EventTriggerType.PointerEnter},
                {typeof(IPointerExitHandler), EventTriggerType.PointerExit},
                {typeof(IPointerDownHandler), EventTriggerType.PointerDown},
                {typeof(IPointerUpHandler), EventTriggerType.PointerUp},
                {typeof(IPointerClickHandler), EventTriggerType.PointerClick},
                {typeof(IDragHandler), EventTriggerType.Drag},
                {typeof(IDropHandler), EventTriggerType.Drop},
                {typeof(IScrollHandler), EventTriggerType.Scroll},
                {typeof(IUpdateSelectedHandler), EventTriggerType.UpdateSelected},
                {typeof(ISelectHandler), EventTriggerType.Select},
                {typeof(IDeselectHandler), EventTriggerType.Deselect},
                {typeof(IMoveHandler), EventTriggerType.Move},
                {typeof(IInitializePotentialDragHandler), EventTriggerType.InitializePotentialDrag},
                {typeof(IBeginDragHandler), EventTriggerType.BeginDrag},
                {typeof(IEndDragHandler), EventTriggerType.EndDrag},
                {typeof(ISubmitHandler), EventTriggerType.Submit},
                {typeof(ICancelHandler), EventTriggerType.Cancel},
                {typeof(IInitializePotentialPinchHandler), EventTriggerType.InitializePotentialPinch},
                {typeof(IBeginPinchHandler), EventTriggerType.BeginPinch},
                {typeof(IPinchHandler), EventTriggerType.Pinch},
                {typeof(IEndPinchHandler), EventTriggerType.EndPinch},
            };

        [SerializeField] private List<Entry> m_Delegates;

        public List<Entry> Triggers
        {
            get
            {
                if (m_Delegates == null)
                    m_Delegates = new List<Entry>();
                return m_Delegates;
            }
            set => m_Delegates = value;
        }

        private void Execute(EventTriggerType id, BaseEventData eventData)
        {
            foreach (var entry in Triggers)
            {
                if (entry.eventID != id) continue;
                entry.callback?.Invoke(eventData);
                return;
            }
        }

        private bool HasTrigger(EventTriggerType id)
        {
            foreach (var entry in Triggers)
            {
                if (entry.eventID == id)
                    return true;
            }

            return false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Execute(EventTriggerType.PointerEnter, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Execute(EventTriggerType.PointerExit, eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Execute(EventTriggerType.PointerDown, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Execute(EventTriggerType.PointerUp, eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Execute(EventTriggerType.PointerClick, eventData);
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            Execute(EventTriggerType.InitializePotentialDrag, eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Execute(EventTriggerType.BeginDrag, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Execute(EventTriggerType.Drag, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Execute(EventTriggerType.EndDrag, eventData);
        }

        public void OnDrop(PointerEventData eventData)
        {
            Execute(EventTriggerType.Drop, eventData);
        }

        public void OnScroll(PointerEventData eventData)
        {
            Execute(EventTriggerType.Scroll, eventData);
        }

        public void OnUpdateSelected(BaseEventData eventData)
        {
            Execute(EventTriggerType.UpdateSelected, eventData);
        }

        public void OnSelect(BaseEventData eventData)
        {
            Execute(EventTriggerType.Select, eventData);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            Execute(EventTriggerType.Deselect, eventData);
        }

        public void OnMove(AxisEventData eventData)
        {
            Execute(EventTriggerType.Move, eventData);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            Execute(EventTriggerType.Submit, eventData);
        }

        public void OnCancel(BaseEventData eventData)
        {
            Execute(EventTriggerType.Cancel, eventData);
        }

        public void OnInitializePotentialPinch(TouchEventData eventData)
        {
            Execute(EventTriggerType.InitializePotentialPinch, eventData);
        }

        public void OnBeginPinch(PinchEventData eventData)
        {
            Execute(EventTriggerType.BeginPinch, eventData);
        }

        public void OnPinch(PinchEventData eventData)
        {
            Execute(EventTriggerType.Pinch, eventData);
        }

        public void OnEndPinch(PinchEventData eventData)
        {
            Execute(EventTriggerType.EndPinch, eventData);
        }

        public bool CanIgnore<T>() where T : IEventSystemHandler
        {
            if (s_TriggerTypeMappings.TryGetValue(typeof(T), out var type))
                return !HasTrigger(type);
            return true;
        }
    }
}