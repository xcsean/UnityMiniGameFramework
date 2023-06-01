using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityMiniGameFramework
{
    public class PinchEventData : BaseEventData
    {
        public readonly List<TouchEventData> touches = new List<TouchEventData>();

        public GameObject pointerPinch { get; private set; }

        public TouchEventData pinchStart { get; private set; }

        public TouchEventData pinchEnd { get; private set; }

        public Vector2 lastPinchDelta { get; private set; }

        public Vector2 pinchDelta { get; private set; }

        public Vector2 pressPosition
        {
            get
            {
                if (pinchStart != null && pinchEnd != null)
                    return (pinchEnd.pressPosition - pinchStart.pressPosition) / 2 + pinchStart.pressPosition;
                if (pinchStart != null)
                    return pinchStart.pressPosition;
                if (pinchEnd != null)
                    return pinchEnd.pressPosition;
                return Vector2.zero;
            }
        }

        public Vector2 position
        {
            get
            {
                if (pinchStart != null && pinchEnd != null)
                    return (pinchEnd.position - pinchStart.position) / 2 + pinchStart.position;
                if (pinchStart != null)
                    return pinchStart.position;
                if (pinchEnd != null)
                    return pinchEnd.position;
                return Vector2.zero;
            }
        }

        public bool pinching
        {
            get => (pinchStart != null && pinchStart.pinching) && (pinchEnd != null && pinchEnd.pinching);
            set
            {
                if (pinchStart != null)
                    pinchStart.pinching = value;
                if (pinchEnd != null)
                    pinchEnd.pinching = value;
                if (value && pinchStart != null && pinchEnd != null)
                    pinchDelta = pinchEnd.position - pinchStart.position;
            }
        }

        public Vector2 delta { get; private set; }


        public PinchEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public bool IsPointerMoving()
        {
            foreach (var touch in touches)
            {
                if (touch.IsPointerMoving())
                    return true;
            }

            return false;
        }

        public bool Prepare()
        {
            if (pinching) return false;

            var dirtyStart = pinchStart != null;
            var dirtyEnd = pinchEnd != null;
            if (dirtyStart || dirtyEnd)
            {
                foreach (var touch in touches)
                {
                    if (dirtyStart && pinchStart == touch)
                        dirtyStart = false;
                    if (dirtyEnd && pinchEnd == touch)
                        dirtyEnd = false;
                    if (!dirtyStart && !dirtyEnd)
                        break;
                }

                if (dirtyStart)
                    pinchStart = null;
                if (dirtyEnd)
                    pinchEnd = null;
                if (pinchStart == null || pinchEnd == null)
                    pointerPinch = null;
            }

            if ((pinchStart == null || pinchEnd == null) && touches.Count > 1)
            {
                for (var i = 0; i < touches.Count - 1; i++)
                {
                    var matched = false;
                    var target = touches[i].pointerPinch;
                    for (var j = i + 1; j < touches.Count; j++)
                    {
                        var target2 = touches[j].pointerPinch;
                        if (target != target2) continue;
                        matched = true;
                        pinchStart = touches[i];
                        pinchEnd = touches[j];
                        pointerPinch = target;
                        break;
                    }

                    if (!matched) continue;
                    dirtyStart = dirtyEnd = true;
                    break;
                }
            }

            return dirtyStart || dirtyEnd;
        }

        public static bool ShouldStartDrag(Vector2 pressPos, Vector2 currentPos, float threshold,
            bool useDragThreshold)
        {
            if (!useDragThreshold)
                return true;
            return (pressPos - currentPos).sqrMagnitude >= threshold * threshold;
        }

        public bool ShouldStartPinch(float threshold)
        {
            return ShouldStartDrag(pinchStart.pressPosition, pinchStart.position, threshold,
                pinchStart.useDragThreshold) || ShouldStartDrag(pinchEnd.pressPosition, pinchEnd.position, threshold,
                pinchEnd.useDragThreshold);
        }

        public bool HasPinch(TouchEventData pinch)
        {
            return pinchStart == pinch || pinchEnd == pinch;
        }

        public void CalculateDelta()
        {
            lastPinchDelta = pinchDelta;
            pinchDelta = pinchEnd.position - pinchStart.position;
        }
    }
}