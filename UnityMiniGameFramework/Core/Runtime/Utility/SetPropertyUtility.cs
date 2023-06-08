using UnityEngine;
using System.Collections.Generic;

namespace UnityMiniGameFramework
{
    public static class SetPropertyUtility
    {
        public static bool SetColor(ref Color curValue, Color newValue)
        {
            if (curValue.r == newValue.r && curValue.g == newValue.g && curValue.b == newValue.b &&
                curValue.a == newValue.a)
                return false;
            curValue = newValue;
            return true;
        }

        public static bool SetVector2(ref Vector2 curValue, Vector2 newValue)
        {
            if (curValue.x == newValue.x && curValue.y == newValue.y)
                return false;
            curValue = newValue;
            return true;
        }

        public static bool SetVector3(ref Vector3 curValue, Vector3 newValue)
        {
            if (curValue.x == newValue.x && curValue.y == newValue.y && curValue.z == newValue.z)
                return false;
            curValue = newValue;
            return true;
        }

        public static bool SetStruct<T>(ref T curValue, T newValue) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(curValue, newValue))
                return false;
            curValue = newValue;
            return true;
        }

        public static bool SetClass<T>(ref T curValue, T newValue) where T : class
        {
            if ((curValue == null && newValue == null) || (curValue != null && curValue.Equals(newValue)))
                return false;
            curValue = newValue;
            return true;
        }
    }
}