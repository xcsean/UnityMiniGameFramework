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

        private CMStoreHouse _CMStoreHouse;

        protected VisualTreeAsset _flyIconUxml;
        protected Label _labCapacity;
        protected Label _labPopup;

        protected CapacityPopupNumber popupNumber;

        protected Color _red = new Color(237f / 255f, 77f / 255f, 10f / 255f);
        protected Color _green = new Color(146f / 255f, 234f / 255f, 75f / 255f);

        private List<TemplateContainer> flyIcons = new List<TemplateContainer>();

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            FindUI();

            RefreshInfo(null);
        }

        protected void FindUI()
        {
            _labCapacity = this._uiObjects["labCapacity"].unityVisualElement as Label;
            _labPopup = this._uiObjects["labPopup"].unityVisualElement as Label;

            _flyIconUxml = ((UnityResourceManager)UnityGameApp.Inst.Resource).LoadUXML($"UI/Controls/FlyIcon");
        }

        public void RefreshInfo(CMStoreHouse _cmStoreHouse)
        {
            _CMStoreHouse = _cmStoreHouse;
            if (_CMStoreHouse == null)
            {
                DoUpdateInputStore(0, 0);

                return;
            }
            DoUpdateInputStore(_CMStoreHouse.storeHouseInfo.storeCount, 0);
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
                    TextColor = changeCnt > 0 ? _green : _red,
                    LifeTime = 1f,
                    UpPos = Vector3.zero,
                };
            }
        }

        public void OnUpdatePopup()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                FlyAction();
            }

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

        private void FlyAction()
        {
            var tvList = new List<TimeValue>();

            for (int i = 0; i < 10; i++)
            {
                TemplateContainer temp = null;
                if (flyIcons.Count == 0)
                {
                    if (_flyIconUxml != null)
                    {
                        temp = _flyIconUxml.CloneTree();
                    }
                }
                else
                {
                    temp = flyIcons[0];
                    flyIcons.RemoveAt(0);
                }
                if (temp == null)
                {
                    continue;
                }
                //tvList.Clear();
                //tvList.Add(new TimeValue(i * 0.02f));
                temp.transform.position = new Vector3(0, 0);
                temp.style.display = DisplayStyle.Flex;
                //temp.Q<VisualElement>("Icon").style.transitionDelay = new StyleList<TimeValue>(tvList);
                //temp.Q<VisualElement>("Icon").style.translate = new StyleTranslate(new Translate(new Length(400f), new Length(-400f), 0f));
                //temp.schedule.Execute(() =>
                //{
                //    //temp.Q<VisualElement>("Icon").style.translate = new StyleTranslate(new Translate(new Length(0f), new Length(0f), 0f));
                //    temp.style.display = DisplayStyle.None;
                //    flyIcons.Add(temp);
                //}).StartingIn(2000);
            }
        }

        public override void showUI()
        {
            base.showUI();

            addUpdate(this.OnUpdatePopup);
        }

    }
}
