using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using MiniGameFramework;
using Debug = UnityEngine.Debug;
using GameObject = UnityEngine.GameObject;

namespace UnityMiniGameFramework
{
    /// <summary>
    /// 摇杆界面  全屏，UI层级最低
    /// </summary>
    public class UIJoyStickPanel : UIPanel
    {
        override public string type => "UIJoyStickPanel";

        public static UIJoyStickPanel create()
        {
            return new UIJoyStickPanel();
        }

        // 摇杆相关
        protected float _panelWidth;
        protected Vector2 _panelInitPos;
        protected Vector2 _btnInitPos;
        protected VisualElement _btn;
        protected VisualElement _back;
        protected VisualElement _content;
        protected VisualElement _clickArea;

        protected bool _click;
        protected bool _moving;
        protected Vector3 _movVec;
        protected Vector3 _dir;
        protected float _degree;

        public bool isMoving => _moving;
        public Vector3 movVector3 => _movVec;
        public Vector3 direction => _dir;
        public float degreee => _degree;

        // 英雄拖动相关
        protected Vector3 _currentPickHeroInitPos;
        protected CMNPCHeros _currentPickNPCHero;
        protected VFXRangeCricle _atkRangeCircleVFX;
        protected string _currentPickAreaName;
        protected Vector3 _currentPickPosition;
        protected Dictionary<string, CMNPCHeros> _defAreaHeros;
        private Vector2 _mapSize;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _btn = this._uiObjects["button"].unityVisualElement;
            _back = this._uiObjects["back"].unityVisualElement;
            // _content = this._uiObjects["Content"].unityVisualElement;
            _clickArea = this._uiObjects["ClickArea"].unityVisualElement;
            //
            _btn.style.visibility = Visibility.Hidden;
            _back.style.visibility = Visibility.Hidden;
            GameObject mapPanel = GameObject.Find("MapPanel");
            if (mapPanel)
            {
                var size = mapPanel.GetComponent<BoxCollider>().size;
                _mapSize = new Vector2(size.x, size.z);
            }


            //SetClickArea(true);

            UnityGameApp.Inst.regNextFramePostUpdateCall(_postInit);

            _atkRangeCircleVFX = UnityGameApp.Inst.VFXManager.createVFXObject("AttackRangeCircle") as VFXRangeCricle;
            _atkRangeCircleVFX.unityGameObject.SetActive(false);
        }

        void _postInit()
        {
            // _btnInitPos = new Vector2(_btn.transform.position.x, _btn.transform.position.y);
            // _panelInitPos = new Vector2(_back.transform.position.x, _back.transform.position.y);
            // _panelWidth = _back.layout.width / 2;

            _clickArea.RegisterCallback<PointerDownEvent>(OnMouseDown);
            _clickArea.RegisterCallback<PointerMoveEvent>(OnMouseMove);
            _clickArea.RegisterCallback<PointerUpEvent>(OnMouseUp);
            _clickArea.RegisterCallback<WheelEvent>(OnMouseRoll);
            //_clickArea.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            //_clickArea.RegisterCallback<MouseUpEvent>(OnMouseUp);
            //_clickArea.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
        }

        protected Vector2 _transMousePosition(VisualElement ve, Vector2 mousePos)
        {
            //Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(
            //m_Bar.panel, TransformToFollow.position, UnityGameApp.Inst.MainScene.unityCamera);

            //newPosition.x = (newPosition.x - m_Bar.layout.width / 2);
            //m_Bar.transform.position = newPosition;

            //UnityEngine.Vector2 mousePositionCorrected = new UnityEngine.Vector2(mousePos.x, Screen.height - mousePos.y);
            //UnityEngine.Vector2 newPosition = RuntimePanelUtils.ScreenToPanel(ve.panel, mousePos);
            Vector2 newPosition = ve.WorldToLocal(mousePos);
            newPosition.x = newPosition.x - ve.layout.width / 2;
            newPosition.y = newPosition.y - ve.layout.height / 2;
            return newPosition;
        }

        // 摇杆点击触发区域
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
                // 下半屏
                // 参数参考Content节点
                _clickArea.style.width = new StyleLength(Length.Percent(76));
                _clickArea.style.height = new StyleLength(Length.Percent(50));
                _clickArea.style.left = new StyleLength(Length.Percent(12));
                _clickArea.style.bottom = new StyleLength(80f);
            }
        }

        public void OnMouseDown(PointerDownEvent e)
        {
            OnPutEnd();

            _defAreaHeros = (UnityGameApp.Inst.Game as ChickenMasterGame).GetDefAreaHeros();
            if (OnCheckPut() != null)
            {
                // 点击英雄了不处理摇杆
                return;
            }

            OnMouseUp(null);

            // _btn.style.visibility = Visibility.Visible;
            // _back.style.visibility = Visibility.Visible;

            // _content.transform.position = _transMousePosition(_content, e.position);
            // _btn.transform.position = _btnInitPos;

            //SetClickArea(true);

            this._click = true;
            this._moving = false;

            //_btn.style.unityBackgroundImageTintColor = new StyleColor(new Color(1, 1, 1, 1));

            //_clickArea.CaptureMouse();
        }

        protected Vector2 posDelta = Vector2.zero;

        public void OnMouseMove(PointerMoveEvent e)
        {
            if (!this._click)
            {
                return;
            }

            var camera = UnityGameApp.Inst.MainScene.camera as UnityGameCamera;

            camera?.MoveCamera(e.deltaPosition);


            // this._moving = true;
            //
            // // posDelta = e.mouseDelta;
            // posDelta.x = e.deltaPosition.x;
            // posDelta.y = e.deltaPosition.y;
            //
            // Vector2 btn_pos = _btn.transform.position;
            // btn_pos.x += posDelta.x;
            // btn_pos.y += posDelta.y;
            //
            // _btn.transform.position = btn_pos;
            //
            // //将小圆限制大圆范围内
            // //Debug.Log("this.btn.transform.position  = " + this.btn.transform.position);
            // float ratio = btn_pos.magnitude / this._panelWidth;
            // if (ratio > 1)
            // {
            //     _btn.transform.position = _btn.transform.position / ratio;
            // }
            //
            // ////获取向量归一化
            // _dir = _btn.transform.position.normalized;
            // ////获取弧度
            // _degree = Mathf.Atan2(_btn.transform.position.y, _btn.transform.position.x);
            //
            // _movVec = new Vector3(-_btn.transform.position.x, 0, _btn.transform.position.y);
        }

        public void OnMouseLeave(MouseLeaveEvent e)
        {
            // 真机模拟环境，滑动中触发打开一个界面，在界面上松手会丢失MouseUpEvent
            //OnMouseUp(null);
        }

        public void OnMouseRoll(WheelEvent e)
        {
            var camera = UnityGameApp.Inst.MainScene.camera as UnityGameCamera;

            camera?.ScaleCamera(e.delta.y);
        }

        public void OnMouseUp(PointerUpEvent e)
        {
            if (!_click)
            {
                return;
            }

            //_content.transform.position = _panelInitPos;
            //_btn.transform.position = _btnInitPos;

            //SetClickArea(false);

            this._click = false;
            this._moving = false;

            //_btn.style.unityBackgroundImageTintColor = new StyleColor(new UnityEngine.Color(1, 1, 1, 0.4f));

            // _btn.style.visibility = Visibility.Hidden;
            // _back.style.visibility = Visibility.Hidden;

            _clickArea.ReleaseMouse();
        }

        /// <summary>
        /// 处理真机环境 滑动到按钮松手收不到MouseUpEvent事件
        /// </summary>
        protected void OnCheckTouchUp()
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount == 0)
                {
                    // mouse up
                    OnMouseUp(null);
                }
            }
            else
            {
                if (!Input.GetMouseButton(0))
                {
                    // mouse up
                    OnMouseUp(null);
                }
            }
        }

        /// <summary>
        /// 检测点击的英雄能否拖动
        /// </summary>
        /// <returns></returns>
        public CMNPCHeros OnCheckPut()
        {
            _currentPickNPCHero = null;

            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;

            UnityEngine.RaycastHit[] hitInfos = UnityEngine.Physics.RaycastAll(
                UnityGameApp.Inst.MainScene.unityCamera.ScreenPointToRay(Input.mousePosition),
                1000.0f,
                UnityEngine.LayerMask.GetMask("Self", "Factory"));

            for (int i = 0; i < hitInfos.Length; ++i)
            {
                var obj = hitInfos[i].collider.gameObject;

                var mapBuild = obj.GetComponent<UnityBuildingColliderTrigger>();
                if (mapBuild != null)
                {
                    mapBuild.ShowBuildingPanel();
                    break;
                }
                
                var ugComp = obj.GetComponent<UnityGameObjectBehaviour>();
                if (ugComp == null)
                {
                    // no  UnityGameObjectBehaviour component
                    continue;
                }

                var mapHeroObj = ugComp.mgGameObject as MapHeroObject;
                if (mapHeroObj == null)
                {
                    // not map hero object
                    continue;
                }

                foreach (var hPair in cmGame.cmNPCHeros)
                {
                    if (hPair.Value.mapHero == mapHeroObj)
                    {
                        _currentPickNPCHero = hPair.Value;
                        break;
                    }
                }

                if (_currentPickNPCHero != null)
                {
                    if (UnityGameApp.Inst.MainScene.map.currentLevel != null &&
                        UnityGameApp.Inst.MainScene.map.currentLevel.isStarted)
                    {
                        _currentPickNPCHero = null;
                        cmGame.ShowTips(CMGNotifyType.CMG_ERROR, "Unable to Drag!");
                        return null;
                    }

                    _currentPickNPCHero.isPicked = true;
                    _currentPickNPCHero.mapHero.actionComponent.addState(ActStates.STATE_KEY_NO_ATK);

                    var position = _currentPickNPCHero.mapHero.unityGameObject.transform.position;
                    _currentPickHeroInitPos = position;
                    position = new Vector3(
                        position.x,
                        position.y + 0.5f,
                        position.z
                    );
                    _currentPickNPCHero.mapHero.unityGameObject.transform.position = position;

                    _atkRangeCircleVFX.unityGameObject.SetActive(true);
                    _atkRangeCircleVFX.unityGameObject.transform.SetParent(_currentPickNPCHero.mapHero.unityGameObject
                        .transform);
                    _atkRangeCircleVFX.unityGameObject.transform.localPosition = new Vector3(0, 0.5f, 0);

                    _atkRangeCircleVFX.SetCircleRange(_currentPickNPCHero.gun.attackRange);

                    break;
                }
            }

            return _currentPickNPCHero;
        }

        /// <summary>
        /// 拖动摆放英雄结束
        /// </summary>
        public void OnPutEnd()
        {
            ShowDefArea(false);

            if (_currentPickNPCHero == null)
            {
                return;
            }


            _atkRangeCircleVFX.unityGameObject.SetActive(false);
            _atkRangeCircleVFX.unityGameObject.transform.SetParent(null);

            _currentPickNPCHero.isPicked = false;
            _currentPickNPCHero.mapHero.actionComponent.subState(ActStates.STATE_KEY_NO_ATK);
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            if (UnityGameApp.Inst.MainScene.implMap.isInActiveRect(_currentPickNPCHero.mapHero.unityGameObject
                .transform
                .position))
            {
                // pick ground position
                UnityEngine.RaycastHit[] hitInfos = UnityEngine.Physics.RaycastAll(
                    UnityGameApp.Inst.MainScene.unityCamera.ScreenPointToRay(Input.mousePosition),
                    1000.0f,
                    UnityEngine.LayerMask.GetMask("Map"));

                if (hitInfos.Length > 0)
                {
                    Vector3 updatePos = new Vector3(
                        hitInfos[0].point.x,
                        _currentPickHeroInitPos.y,
                        hitInfos[0].point.z
                    );

                    if (cmGame.MapLogicObjects.ContainsKey(AstarUtility.GetLogicPos(updatePos)))
                    {
                        cmGame.ShowTips(CMGNotifyType.CMG_ERROR, "Location occupied!");
                        _currentPickNPCHero.UpdateUnityGoPos(_currentPickHeroInitPos);
                        _currentPickNPCHero = null;
                        return;
                    }

                    if (_currentPickNPCHero.TryUpdateUnityGoPos(updatePos))
                    {
                        UnityGameApp.Inst.MainScene.implMap.ClearPath();
                        _currentPickNPCHero.UpdateUnityGoPos(updatePos);
                    }
                    else
                    {
                        cmGame.ShowTips(CMGNotifyType.CMG_ERROR, "Need to reserve access!");
                        // 无法放下
                        //_currentPickNPCHero.mapHero.unityGameObject.transform.position = _currentPickHeroInitPos;
                        _currentPickNPCHero.UpdateUnityGoPos(_currentPickHeroInitPos);
                        _currentPickNPCHero = null;

                        return;
                    }
                }

                var pos = _currentPickNPCHero.mapHero.unityGameObject.transform.position;
                _currentPickNPCHero.heroInfo.position.Set(pos.x, pos.y, pos.z);
            }
            else
            {
                // 未解锁英雄区域不能摆放
                //_currentPickNPCHero.mapHero.unityGameObject.transform.position = _currentPickHeroInitPos;
                _currentPickNPCHero.UpdateUnityGoPos(_currentPickHeroInitPos);
                _currentPickNPCHero = null;

                return;
            }

            _currentPickNPCHero = null;


            cmGame.baseInfo.markDirty();
        }

        private void ShowDefArea(bool isShow)
        {
            if (isShow)
            {
                var map = UnityGameApp.Inst.MainScene.map as Map;
                foreach (var area in map.defAreas)
                {
                    if (_defAreaHeros.ContainsKey(area.Key))
                    {
                        area.Value.ShowAreaStyle(true);
                    }
                    else
                    {
                        area.Value.ShowAreaStyle(false);
                    }
                }
            }
            else
            {
                var map = UnityGameApp.Inst.MainScene.map as Map;
                foreach (var area in map.defAreas)
                {
                    area.Value.HideArea();
                }
            }
        }

        protected void OnUpdate()
        {
            OnCheckTouchUp();

            // 处理英雄拖动摆放
            if (_currentPickNPCHero == null)
            {
                return;
            }

            //Debug.Log("OnUpdate" + $"{_currentPickNPCHero.mapHero.unityGameObject.transform.position}");


            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount != 1)
                {
                    // mouse up
                    OnPutEnd();
                    return;
                }
            }
            else
            {
                if (!Input.GetMouseButton(0))
                {
                    // mouse up
                    OnPutEnd();
                    return;
                }
            }

            // pick def area

            ShowDefArea(true);

            UnityEngine.RaycastHit[] hitInfos = UnityEngine.Physics.RaycastAll(
                UnityGameApp.Inst.MainScene.unityCamera.ScreenPointToRay(Input.mousePosition),
                1000.0f,
                UnityEngine.LayerMask.GetMask("DefArea"));

            _currentPickAreaName = null;
            for (int i = 0; i < hitInfos.Length;)
            {
                _currentPickAreaName = hitInfos[i].collider.gameObject.name;
                _currentPickPosition = hitInfos[i].point;

                break;
            }


            //if (_currentPickAreaName == null || !_defAreaHeros.ContainsKey(_currentPickAreaName))
            if (!UnityGameApp.Inst.MainScene.implMap.isInActiveRect(_currentPickNPCHero.mapHero.unityGameObject
                .transform
                .position))
            {
                _atkRangeCircleVFX.ShowAttackRange(false); // can't put
            }
            else
            {
                _atkRangeCircleVFX.ShowAttackRange(true); // can put
            }

            // pick ground position
            hitInfos = UnityEngine.Physics.RaycastAll(
                UnityGameApp.Inst.MainScene.unityCamera.ScreenPointToRay(Input.mousePosition),
                1000.0f,
                UnityEngine.LayerMask.GetMask("Map"));

            if (hitInfos.Length > 0)
            {
                _currentPickNPCHero.mapHero.unityGameObject.transform.position = new Vector3(
                    hitInfos[0].point.x,
                    hitInfos[0].point.y + 0.5f,
                    hitInfos[0].point.z
                );
            }
        }

        public override void showUI()
        {
            base.showUI();

            addUpdate(this.OnUpdate);
        }

        public override void hideUI()
        {
            base.hideUI();
        }
    }
}