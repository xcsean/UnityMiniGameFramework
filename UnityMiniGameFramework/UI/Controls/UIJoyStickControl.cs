using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UIJoyStickControl : UIObject
    {
        protected float _panelWidth;
        protected UnityEngine.Vector2 _panelInitPos;
        protected UnityEngine.Vector2 _btnInitPos;

        protected VisualElement _btn;
        protected VisualElement _back;

        protected bool _click;
        protected bool _moving;
        protected UnityEngine.Vector3 _movVec;
        protected UnityEngine.Vector3 _dir;
        protected float _degree;

        public bool isMoving => _moving;
        public UnityEngine.Vector3 movVector3 => _movVec;

        public UnityEngine.Vector3 direction => _dir;
        public float degreee => _degree;

        override public string type => "UIJoyStickControl";
        new public static UIJoyStickControl create()
        {
            return new UIJoyStickControl();
        }

        override public void onInit(UIControlConf c, VisualElement o)
        {
            base.onInit(c, o);

            _btn = this._subControls["button"].unityVisualElement;
            _back = this._subControls["back"].unityVisualElement;

            UnityGameApp.Inst.regNextFramePostUpdateCall(_postInit);
        }

        void _postInit()
        {
            _btnInitPos = new UnityEngine.Vector2(_btn.transform.position.x, _btn.transform.position.y);
            _panelInitPos = new UnityEngine.Vector2(_back.transform.position.x, _back.transform.position.y);
            _panelWidth = _back.layout.width / 2;

            _unityVE.RegisterCallback<MouseDownEvent>(OnMouseDown);
            _unityVE.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            _unityVE.RegisterCallback<MouseUpEvent>(OnMouseUp);

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

        public void OnMouseDown(MouseDownEvent e)
        {
            _unityVE.transform.position = _transMousePosition(_unityVE, e.mousePosition);
            _btn.transform.position = _btnInitPos;

            this._click = true;
            this._moving = false;

            _unityVE.CaptureMouse();
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
        public void OnMouseUp(MouseUpEvent e)
        {
            _unityVE.transform.position = _panelInitPos;
            _btn.transform.position = _btnInitPos;

            this._click = false;
            this._moving = false;

            _unityVE.ReleaseMouse();
        }

    }
}
