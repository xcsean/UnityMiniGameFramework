using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityMiniGameFramework.UISystem
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasScaler))]
    public sealed class UICanvasMatchScaler : UIBehaviour
    {
        [NonSerialized] private CanvasScaler m_CanvasScaler;

        public CanvasScaler CanvasScaler
        {
            get
            {
                if (!m_CanvasScaler)
                    m_CanvasScaler = GetComponent<CanvasScaler>();
                return m_CanvasScaler;
            }
        }

        protected override void OnEnable()
        {
            UpdateScalerMatch();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            if (isActiveAndEnabled)
                UpdateScalerMatch();
        }

        private void UpdateScalerMatch()
        {
            var width = CanvasScaler.referenceResolution.x;
            var height = CanvasScaler.referenceResolution.y;
            var aspect = width / height;
            var ratio = Screen.width / (float) Screen.height;
            var scaler = 0;
            if (ratio > aspect)
            {
                width = ratio * height;
                scaler = 1;
            }
            else
                height = width / ratio;

            CanvasScaler.matchWidthOrHeight = scaler;
            if (Application.isPlaying)
                StartCoroutine(ResetResolution((int) width, (int) height));
        }

        private IEnumerator ResetResolution(int width, int height)
        {
            yield return new WaitForEndOfFrame();
            Screen.SetResolution(width, height, true);
            print(Screen.currentResolution);
        }
    }
}