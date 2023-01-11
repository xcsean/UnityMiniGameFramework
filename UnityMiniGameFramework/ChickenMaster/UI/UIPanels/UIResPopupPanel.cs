using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.UIElements;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    /// <summary>
    /// 获得金币 飘字动效
    /// </summary>
    public class UIResPopupPanel : UIPanel
    {
        override public string type => "UIResPopupPanel";
        public static UIResPopupPanel create()
        {
            return new UIResPopupPanel();
        }

        protected VisualElement _popup;
        protected VisualElement _sprIcon;
        protected Label _labCount;

        private bool canMove = false;
        private float popupY = 0f;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            FindUI();
        }

        protected void FindUI()
        {
            _popup = this._uiObjects["Popup"].unityVisualElement;
            _sprIcon = this._uiObjects["sprIcon"].unityVisualElement;
            _labCount = this._uiObjects["labCount"].unityVisualElement as Label;
        }

        protected void onUpdate()
        {
            if (!canMove)
            {
                return;
            }
            popupY -= UnityEngine.Time.deltaTime * 100;
            _popup.transform.position = new UnityEngine.Vector3(0, popupY, 0);

            if (_trans != null)
            {
                var screenPos = UnityGameApp.Inst.ScreenToUIPos((UnityGameApp.Inst.MainScene.camera as UnityGameCamera)
                    .worldToScreenPos(_trans.position));

                setPoisition((int)screenPos.x, (int)screenPos.y);
            }

            if (popupY < -150f)
            {
                canMove = false;
                hideUI();
                return;
            }
        }

        private UnityEngine.Transform _trans;
        public void SetResInfo(int resCount, int resType = 0, UnityEngine.Transform trans = null)
        {
            _trans = trans;
            popupY = 0;
            canMove = true;
            _popup.transform.position = new UnityEngine.Vector3(0, 0, 0);
            string countStr = StringUtil.StringNumFormat($"{resCount}");
            _labCount.text = resCount > 0 ? $"+{countStr}" : $"{countStr}";

            showUI();
        }

        public override void showUI()
        {
            base.showUI();

            addUpdate(onUpdate);
        }
    }
}
