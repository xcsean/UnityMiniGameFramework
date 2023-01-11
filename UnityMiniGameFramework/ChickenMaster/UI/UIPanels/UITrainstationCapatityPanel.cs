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

    public class CapacityPopup
    {
        public string Text;
        public Color TextColor;
        public float LifeTime;
        public Vector3 UpPos;
    }

    /// <summary>
    /// 仓库容量
    /// </summary>
    public class UITrainstationCapatityPanel : UIPanel
    {
        override public string type => "UITrainstationCapatityPanel";
        public static UITrainstationCapatityPanel create()
        {
            return new UITrainstationCapatityPanel();
        }

        private CMTrainStation _CMTrainStation;

        private VisualElement layout;
        private Label _labCapacity;
        private Label _labPopup;
        public CapacityPopup popupNumber;

        protected Color _red = new Color(237f / 255f, 77f / 255f, 10f / 255f);
        protected Color _green = new Color(146f / 255f, 234f / 255f, 75f / 255f);

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            FindUI();

            RefreshInfo(null);
        }

        protected void FindUI()
        {
            layout = _uiObjects["GridLayout"].unityVisualElement as VisualElement;
            var grid = layout.Q<VisualElement>("grid");
            _labCapacity = grid.Q<Label>("labCapacity");
            _labPopup = grid.Q<Label>("labPopup");
            popupNumber = null;
        }

        public void RefreshInfo(CMTrainStation _cmTrainStation)
        {
            _CMTrainStation = _cmTrainStation;
            if (_CMTrainStation == null)
            {
                DoUpdateInputStore(0, 0);

                return;
            }
            DoUpdateInputStore(_CMTrainStation.trainStationInfo.storeProducts.Count, 0);
        }

        public void DoUpdateInputStore(int totalCnt, int changeCnt)
        {
            // 库存
            _labCapacity.text = $"x{totalCnt}";
            _labPopup.text = "";

            if (changeCnt != 0)
            {
                popupNumber = new CapacityPopup()
                {
                    Text = changeCnt > 0 ? $"+{changeCnt}" : $"{changeCnt}",
                    TextColor = changeCnt > 0 ? _green : _red,
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

