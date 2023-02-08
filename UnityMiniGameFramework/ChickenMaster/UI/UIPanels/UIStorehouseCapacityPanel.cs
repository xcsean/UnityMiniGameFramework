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
        protected Label _labLv;

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
            _labLv = this._uiObjects["labLv"].unityVisualElement as Label;

            _flyIconUxml = ((UnityResourceManager)UnityGameApp.Inst.Resource).LoadUXML($"UI/Controls/FlyIcon");
        }

        public void RefreshInfo(CMStoreHouse _cmStoreHouse)
        {
            _CMStoreHouse = _cmStoreHouse;
            if (_CMStoreHouse == null)
            {
                DoUpdateInputStore(0, 0);
                RefreshLv(1);

                return;
            }
            RefreshLv(_CMStoreHouse.storeHouseInfo.level);
            DoUpdateInputStore(_CMStoreHouse.storeHouseInfo.storeCount, 0);
        }

        public void RefreshLv(int lv)
        {
            _labLv.text = $"Lv.{lv}";
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

                if (changeCnt > 0)
                {
                    flyTotalCnt = 10;
                    InputStorehouseFlyAction();
                }
            }
        }

        public void OnUpdatePopup()
        {
            //if (Input.GetKeyDown(KeyCode.T))
            //{
            //    flyTotalCnt = 10;
            //    InputStorehouseFlyAction();
            //}

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

        private Vector2 GetSelfHeroPos()
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var pos = cmGame.Self.selfMapHero.unityGameObject.transform.position;
            var screenPos = UnityGameApp.Inst.ScreenToUIPos((UnityGameApp.Inst.MainScene.camera as UnityGameCamera).worldToScreenPos(pos));
            screenPos.y -= 110;
            return screenPos;
        }

        /// <summary>
        /// 鸡肉从主角头顶飞到仓库
        /// </summary>
        private void FlyAction()
        {
            var durationTv = new List<TimeValue>();

            TemplateContainer temp = null;
            if (flyIcons.Count == 0)
            {
                if (_flyIconUxml != null)
                {
                    temp = _flyIconUxml.CloneTree();
                    unityUIDocument.rootVisualElement.Add(temp);

                    string ussName = "unity-move-show";
                    var uss = ((UnityResourceManager)UnityGameApp.Inst.Resource).LoadStyleSheet(ussName);
                    temp.styleSheets.Add(uss);
                    temp.Q<VisualElement>("Icon").AddToClassList(ussName);
                }
            }
            else
            {
                temp = flyIcons[0];
                flyIcons.RemoveAt(0);
            }
            if (temp == null)
            {
                return;
            }

            durationTv.Clear();
            durationTv.Add(new TimeValue(0));
            temp.Q<VisualElement>("Icon").style.transitionDuration = new StyleList<TimeValue>(durationTv);


            temp.Q<VisualElement>("Icon").style.translate = new StyleTranslate(new Translate(new Length(
                GetSelfHeroPos().x - temp.parent.transform.position.x), new Length(GetSelfHeroPos().y - temp.parent.transform.position.y), 0f));

            temp.style.display = DisplayStyle.Flex;

            temp.schedule.Execute(() =>
            {
                durationTv.Clear();
                durationTv.Add(new TimeValue(0.5f));
                temp.Q<VisualElement>("Icon").style.transitionDuration = new StyleList<TimeValue>(durationTv);

                temp.Q<VisualElement>("Icon").style.translate = new StyleTranslate(new Translate(new Length(0f), new Length(0f), 0f));

                temp.schedule.Execute(() =>
                {
                    temp.style.display = DisplayStyle.None;
                    flyIcons.Add(temp);
                }).StartingIn(500 + 100);

            }).StartingIn(20);
        }

        /// <summary>
        /// 根据放入仓库数量创建飞鸡肉动画
        /// </summary>
        private int flyTotalCnt = 0;
        private bool creatingFlyIcon = false;
        private void InputStorehouseFlyAction()
        {
            if (flyTotalCnt > 0 && !creatingFlyIcon)
            {
                flyTotalCnt--;
                creatingFlyIcon = true;
                unityUIDocument.rootVisualElement.schedule.Execute(() =>
                {
                    creatingFlyIcon = false;
                    InputStorehouseFlyAction();

                }).StartingIn(50);

                FlyAction();
            }
        }

        public override void showUI()
        {
            base.showUI();

            addUpdate(this.OnUpdatePopup);
        }

    }
}
