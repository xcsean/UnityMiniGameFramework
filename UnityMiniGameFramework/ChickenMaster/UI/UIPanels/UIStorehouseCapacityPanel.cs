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
    public class CapacityPopupNumber
    {
        public string Text;
        public Color TextColor;
        public float LifeTime;
        public Vector3 UpPos;
    }

    /// <summary>
    /// 仓库容量
    /// </summary>
    public class UIStorehouseCapacityPanel : UIPanel
    {
        override public string type => "UIStorehouseCapacityPanel";
        public static UIStorehouseCapacityPanel create()
        {
            return new UIStorehouseCapacityPanel();
        }

        protected Label _labCapacity;
        protected Label _labPopup;
        protected CapacityPopupNumber popupNumber;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            FindUI();

            DoUpdateInputStore(0, 0);
        }

        protected void FindUI()
        {
            _labCapacity = this._uiObjects["labCapacity"].unityVisualElement as Label;
            _labPopup = this._uiObjects["labPopup"].unityVisualElement as Label;
        }

        public void DoUpdateInputStore(int totalCnt, int changeCnt)
        {
            // 库存
            _labCapacity.text = $"{totalCnt}";
            _labPopup.text = "";

            if (changeCnt != 0)
            {
                popupNumber = new CapacityPopupNumber()
                {
                    Text = changeCnt > 0 ? $"+{changeCnt}" : $"{changeCnt}",
                    TextColor = changeCnt > 0 ? Color.green : Color.red,
                    LifeTime = 1f,
                    UpPos = Vector3.zero,
                };
            }
        }

        public void OnUpdatePopup()
        {
            if (popupNumber != null && popupNumber.LifeTime > 0f)
            {
                popupNumber.LifeTime -= Time.deltaTime;
                // 反向的
                popupNumber.UpPos.y -= Time.deltaTime * 50;
                _labPopup.transform.position = popupNumber.UpPos;
                if (_labPopup.text == "")
                {
                    _labPopup.text = $"{popupNumber.Text}";
                    _labPopup.style.color = new StyleColor(popupNumber.TextColor);
                }
            }
            else
            {
                _labPopup.text = "";
                popupNumber = null;
            }
        }

        public override void showUI()
        {
            base.showUI();

            UnityGameApp.Inst.addUpdateCall(this.OnUpdatePopup);
        }

        public override void hideUI()
        {
            base.hideUI();

            UnityGameApp.Inst.removeUpdateCall(this.OnUpdatePopup);
        }
    }
}
