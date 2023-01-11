using System;
using System.Collections;
using UnityEngine;

namespace UnityMiniGameFramework
{
    public class UIImageNumText : UIImageText
    {
        private GameObject m_TargetObject;
        private RectTransform m_ParentTf;

        protected override void Awake()
        {
            base.Awake();
            m_ParentTf = gameObject.transform.parent.GetComponent<RectTransform>();
        }


        public override string text
        {
            get => base.text;
            set => base.text = StringUtil.StringNumFormat(value);
        }

        public void SetUIPos(GameObject targetObj = null)
        {
            m_TargetObject = targetObj != null ? targetObj : m_TargetObject;
            if (m_TargetObject == null)
                return;
            if (!(Camera.main is null))
            {
                Vector3 screenPoint = Camera.main.WorldToScreenPoint(m_TargetObject.transform.position);
                Vector3 uiPos = Vector2.zero;
                RectTransformUtility.ScreenPointToWorldPointInRectangle(m_ParentTf.parent.GetComponent<RectTransform>(),
                    screenPoint,null, out uiPos);
                m_ParentTf.position = new Vector3(uiPos.x, uiPos.y + 50, 0);
            }
        }

        private void FixedUpdate()
        {
            SetUIPos();
        }


        public void OnPlayEnd()
        {
            Destroy(gameObject.transform.parent.gameObject, 0.1f);
        }
    }
}