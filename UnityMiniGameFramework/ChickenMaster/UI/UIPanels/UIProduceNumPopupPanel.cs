using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            FindUI();
        }

        protected void FindUI()
        {
            _labNum = this._uiObjects["labNum"].unityVisualElement as Label;
        }

        public void Show(int num, Action endCb = null)
        {
            if (num == 0)
            {
                if (endCb != null) endCb();
                return;
            }

            _labNum.style.color = new StyleColor(UnityEngine.Color.green);
        }

        public void OnUpdate()
        { 
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
