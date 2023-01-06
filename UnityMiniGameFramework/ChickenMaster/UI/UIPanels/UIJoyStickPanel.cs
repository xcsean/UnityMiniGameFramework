using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.UIElements;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class UIJoyStickPanel : UIPanel
    {
        override public string type => "UIJoyStickPanel";
        public static UIJoyStickPanel create()
        {
            return new UIJoyStickPanel();
        }

        protected float _panelWidth;
        protected UnityEngine.Vector2 _panelInitPos;
        protected UnityEngine.Vector2 _btnInitPos;

        protected VisualElement _btn;
        protected VisualElement _back;
        protected VisualElement _content;
        protected VisualElement _clickArea;

        protected bool _click;
        protected bool _moving;
        protected UnityEngine.Vector3 _movVec;
        protected UnityEngine.Vector3 _dir;
        protected float _degree;

        public bool isMoving => _moving;
        public UnityEngine.Vector3 movVector3 => _movVec;

        public UnityEngine.Vector3 direction => _dir;
        public float degreee => _degree;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _btn = this._uiObjects["button"].unityVisualElement;
            _back = this._uiObjects["back"].unityVisualElement;
            _content = this._uiObjects["Content"].unityVisualElement;
            _clickArea = this._uiObjects["ClickArea"].unityVisualElement;

            _btn.style.visibility = Visibility.Hidden;
            _back.style.visibility = Visibility.Hidden;

            UnityGameApp.Inst.regNextFramePostUpdateCall(_postInit);
        }

        void _postInit()
        {
            _btnInitPos = new UnityEngine.Vector2(_btn.transform.position.x, _btn.transform.position.y);
            _panelInitPos = new UnityEngine.Vector2(_back.transform.position.x, _back.transform.position.y);
            _panelWidth = _back.layout.width / 2;

            _clickArea.RegisterCallback<MouseDownEvent>(OnMouseDown);
            _clickArea.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            _clickArea.RegisterCallback<MouseUpEvent>(OnMouseUp);
            _clickArea.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
        }

        protected UnityEngine.Vector2 _transMousePosition(VisualElement ve, UnityEngine.Vector2 mousePos)
        {
            //Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(
            //m_Bar.panel, TransformToFollow.position, UnityGameApp.Inst.MainScene.unityCamera);

            //newPosition.x = (newPosition.x - m_Bar.layout.width / 2);
            //m_Bar.transform.position = newPosition;

            //UnityEngine.Vector2 mousePositionCorrected = new UnityEngine.Vector2(mousePos.x, Screen.height - mousePos.y);
            //UnityEngine.Vector2 newPosition = RuntimePanelUtils.ScreenToPanel(ve.panel, mousePos);
            UnityEngine.Vector2 newPosition = ve.WorldToLocal(mousePos);
            newPosition.x = newPosition.x - ve.layout.width / 2;
            newPosition.y = newPosition.y - ve.layout.height / 2;
            return newPosition;
        }

        private void SetClickArea(bool isFill)
        {
            if (isFill)
            {
                _clickArea.style.width = new StyleLength(Length.Percent(100));
                _clickArea.style.height = new StyleLength(Length.Percent(100));
                _clickArea.style.left = new StyleLength(Length.Percent(0));
                _clickArea.style.bottom = new StyleLength(0f);
            }
            else
            {
                // 参数参考Content节点
                _clickArea.style.width = new StyleLength(Length.Percent(76));
                _clickArea.style.height = new StyleLength(Length.Percent(50));
                _clickArea.style.left = new StyleLength(Length.Percent(12));
                _clickArea.style.bottom = new StyleLength(80f);
            }
        }

        public void OnMouseDown(MouseDownEvent e)
        {
            _btn.style.visibility = Visibility.Visible;
            _back.style.visibility = Visibility.Visible;

            _content.transform.position = _transMousePosition(_content, e.mousePosition);
            _btn.transform.position = _btnInitPos;

            SetClickArea(true);

            this._click = true;
            this._moving = false;

            _btn.style.unityBackgroundImageTintColor = new StyleColor(new UnityEngine.Color(1, 1, 1, 1));

            _clickArea.CaptureMouse();
        }
        public void OnMouseMove(MouseMoveEvent e)
        {
            if (!this._click)
            {
                return;
            }
            this._moving = true;

            UnityEngine.Vector2 posDelta = e.mouseDelta;

            UnityEngine.Vector2 btn_pos = _btn.transform.position;
            btn_pos.x += posDelta.x;
            btn_pos.y += posDelta.y;

            _btn.transform.position = btn_pos;

            //将小圆限制大圆范围内
            //Debug.Log("this.btn.transform.position  = " + this.btn.transform.position);
            float ratio = btn_pos.magnitude / this._panelWidth;
            if (ratio > 1)
            {
                _btn.transform.position = _btn.transform.position / ratio;
            }

            ////获取向量归一化
            _dir = _btn.transform.position.normalized;
            ////获取弧度
            _degree = UnityEngine.Mathf.Atan2(_btn.transform.position.y, _btn.transform.position.x);

            _movVec = new UnityEngine.Vector3(-_btn.transform.position.x, 0, _btn.transform.position.y);
        }

        public void OnMouseLeave(MouseLeaveEvent e)
        {
            //OnMouseUp(null);
        }

        public void OnMouseUp(MouseUpEvent e)
        {
            _content.transform.position = _panelInitPos;
            _btn.transform.position = _btnInitPos;

            SetClickArea(false);

            this._click = false;
            this._moving = false;

            _btn.style.unityBackgroundImageTintColor = new StyleColor(new UnityEngine.Color(1, 1, 1, 0.4f));

            _btn.style.visibility = Visibility.Hidden;
            _back.style.visibility = Visibility.Hidden;

            _clickArea.ReleaseMouse();
        }

    }
}
