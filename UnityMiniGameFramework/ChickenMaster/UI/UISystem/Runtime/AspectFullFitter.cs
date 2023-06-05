using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityMiniGameFramework.UISystem
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class AspectFullFitter : UIBehaviour,ILayoutController
    {
        private static readonly List<AspectFullFitter> s_AllFitters = new List<AspectFullFitter>();

        public static bool IsFull { get; private set; }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            
        }

        public void SetLayoutHorizontal()
        {
            throw new System.NotImplementedException();
        }

        public void SetLayoutVertical()
        {
            throw new System.NotImplementedException();
        }
    }
}