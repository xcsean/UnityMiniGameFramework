using UnityEngine.EventSystems;
using UnityEngine;

namespace UnityMiniGameFramework
{
    public class TouchEventData: PointerEventData
    {
        public bool pinching { get; set; }
        
        public GameObject pointerPinch { get; set; }
        
        public TouchEventData(EventSystem eventSystem) : base(eventSystem)
        {
            pinching = false;
        }
    }
}