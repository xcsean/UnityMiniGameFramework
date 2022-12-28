using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UIElements;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class UIProduceNumPopupPanel : UIPanel
    {
        override public string type => "UIProduceNumPopupPanel";
        public static UIProduceNumPopupPanel create()
        {
            return new UIProduceNumPopupPanel();
        }

        protected Label _labNum;
        protected float labelUpValue = 0f;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            FindUI();
        }

        protected void FindUI()
        {
            _labNum = this._uiObjects["labNum"].unityVisualElement as Label;
        }

        public void PopupAction(int num, Action endCb = null)
        {
            if (num == 0)
            {
                if (endCb != null)
                {
                    endCb();
                }
                return;
            }
            _labNum.text = num > 0 ? $"+{num}" : $"{num}";
            _labNum.style.color = new StyleColor(num > 0 ? Color.green : Color.red);
            _labNum.transform.position = Vector3.zero;
            labelUpValue = 0f;
        }

        public void OnUpdate()
        {
            if (labelUpValue > 100)
            {
                _labNum.text = "";
                return;
            }
            labelUpValue += Time.deltaTime;
            _labNum.transform.position = new Vector3(0, labelUpValue, 0);
        }

        public override void showUI()
        {
            base.showUI();

            UnityGameApp.Inst.addUpdateCall(this.OnUpdate);
        }

        public override void hideUI()
        {
            base.hideUI();
        }
    }
}
