using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UIFlagMainPanel : UIPanel
    {

        override public string type => "UIFlagMainPanel";
        public static UIFlagMainPanel create()
        {
            return new UIFlagMainPanel();
        }

        protected Button _quitBtn;
        protected MapBuildingObject _flagBuiding;
        protected MGGameObject _lookAtObj;

        protected VisualElement _clickableArea;

        protected UnityEngine.Vector3 _currentPickHeroInitPos;
        protected CMNPCHeros _currentPickNPCHero;
        protected VFXRangeCricle _atkRangeCircleVFX;

        protected string _currentPickAreaName;
        protected UnityEngine.Vector3 _currentPickPosition;

        protected Dictionary<string, CMNPCHeros> _defAreaHeros;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _quitBtn = this._uiObjects["QuitBtn"].unityVisualElement as Button;
            _quitBtn.clicked += onQuitLevelClick;

            _clickableArea = this._uiObjects["Clickable"].unityVisualElement;
            _clickableArea.RegisterCallback<MouseDownEvent>(onMouseDownCA);

            (UnityGameApp.Inst.MainScene.map as Map).buildings.TryGetValue("flagBuilding", out _flagBuiding);

            if(_flagBuiding != null)
            {
                var tr = _flagBuiding.unityGameObject.transform.Find("FlagLookCenter");
                if(tr != null)
                {
                    var ugComp = tr.gameObject.GetComponent<UnityGameObjectBehaviour>();
                    if(ugComp != null)
                    {
                        _lookAtObj = ugComp.mgGameObject;
                    }
                }
            }

            _atkRangeCircleVFX = UnityGameApp.Inst.VFXManager.createVFXObject("AttackRangeCircle") as VFXRangeCricle;
            _atkRangeCircleVFX.unityGameObject.SetActive(false);
        }

        public override void showUI()
        {
            base.showUI(); 
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.uiMainPanel.Joystick.OnMouseUp(null);
            cmGame.uiMainPanel.hideUI();
            cmGame.Egg.eggUI.hideUI();

            _defAreaHeros = cmGame.GetDefAreaHeros();

            if (_lookAtObj != null)
            {
                UnityGameApp.Inst.MainScene.camera.follow(_lookAtObj);
            }

            UnityGameApp.Inst.addUpdateCall(this.OnUpdate);
        }

        public override void hideUI()
        {
            base.hideUI();

            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;

            cmGame.uiMainPanel.showUI();
            cmGame.Egg.eggUI.showUI();

            UnityGameApp.Inst.MainScene.camera.follow(cmGame.Self.mapHero);
        }

        public void onQuitLevelClick()
        {
            this.hideUI();
        }

        public void onMouseDownCA(MouseDownEvent e)
        {
            _currentPickNPCHero = null;

            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;

            UnityEngine.RaycastHit[] hitInfos = UnityEngine.Physics.RaycastAll(
                UnityGameApp.Inst.MainScene.unityCamera.ScreenPointToRay(UnityEngine.Input.mousePosition), 
                1000.0f, 
                UnityEngine.LayerMask.GetMask("Self"));

            for (int i = 0; i < hitInfos.Length; ++i)
            {
                var obj = hitInfos[i].collider.gameObject;

                var ugComp = obj.GetComponent<UnityGameObjectBehaviour>();
                if(ugComp == null)
                {
                    // no  UnityGameObjectBehaviour component
                    continue;
                }

                var mapHeroObj = ugComp.mgGameObject as MapHeroObject;
                if(mapHeroObj == null)
                {
                    // not map hero object
                    continue;
                }

                foreach (var hPair in cmGame.cmNPCHeros)
                {
                    if(hPair.Value.mapHero == mapHeroObj)
                    {
                        _currentPickNPCHero = hPair.Value;
                        break;
                    }
                }

                if(_currentPickNPCHero != null)
                {
                    _currentPickHeroInitPos = _currentPickNPCHero.mapHero.unityGameObject.transform.position;
                    _currentPickNPCHero.mapHero.unityGameObject.transform.position = new UnityEngine.Vector3(
                        _currentPickNPCHero.mapHero.unityGameObject.transform.position.x,
                        _currentPickNPCHero.mapHero.unityGameObject.transform.position.y+0.5f,
                        _currentPickNPCHero.mapHero.unityGameObject.transform.position.z
                    );

                    _atkRangeCircleVFX.unityGameObject.SetActive(true);
                    _atkRangeCircleVFX.unityGameObject.transform.SetParent(_currentPickNPCHero.mapHero.unityGameObject.transform);
                    _atkRangeCircleVFX.unityGameObject.transform.localPosition = new UnityEngine.Vector3(0, -0.5f, 0);

                    _atkRangeCircleVFX.SetCircleRange(_currentPickNPCHero.gun.attackRange);

                    break;
                }
            }
        }

        public void onMouseUpCA()
        {
            if (_currentPickNPCHero == null)
            {
                return;
            }

            _atkRangeCircleVFX.unityGameObject.SetActive(false);
            _atkRangeCircleVFX.unityGameObject.transform.SetParent(null);

            if (_currentPickAreaName == null)
            {
                // no pick area, put back

                _currentPickNPCHero.mapHero.unityGameObject.transform.position = _currentPickHeroInitPos;

                _currentPickNPCHero = null;
                return;
            }

            // put hero
            if (_defAreaHeros.ContainsKey(_currentPickAreaName))
            {
                var toSwapHero = _defAreaHeros[_currentPickAreaName];
                if (toSwapHero == _currentPickNPCHero)
                {
                    // same def area, just put

                    _currentPickNPCHero.mapHero.unityGameObject.transform.position = _currentPickPosition;
                    _currentPickNPCHero.heroInfo.position.x = _currentPickPosition.x;
                    _currentPickNPCHero.heroInfo.position.y = _currentPickPosition.y;
                    _currentPickNPCHero.heroInfo.position.z = _currentPickPosition.z;

                }
                else
                {
                    // swap
                    toSwapHero.heroInfo.defAreaName = _currentPickNPCHero.heroInfo.defAreaName;
                    _defAreaHeros[toSwapHero.heroInfo.defAreaName] = toSwapHero;

                    toSwapHero.mapHero.unityGameObject.transform.position = _currentPickHeroInitPos;
                    toSwapHero.heroInfo.position.x = _currentPickHeroInitPos.x;
                    toSwapHero.heroInfo.position.y = _currentPickHeroInitPos.y;
                    toSwapHero.heroInfo.position.z = _currentPickHeroInitPos.z;

                    _currentPickNPCHero.heroInfo.defAreaName = _currentPickAreaName;
                    _defAreaHeros[_currentPickNPCHero.heroInfo.defAreaName] = _currentPickNPCHero;
                    _currentPickNPCHero.mapHero.unityGameObject.transform.position = _currentPickPosition;
                    _currentPickNPCHero.heroInfo.position.x = _currentPickPosition.x;
                    _currentPickNPCHero.heroInfo.position.y = _currentPickPosition.y;
                    _currentPickNPCHero.heroInfo.position.z = _currentPickPosition.z;
                }
            }
            else
            {
                _defAreaHeros.Remove(_currentPickNPCHero.heroInfo.defAreaName);

                _currentPickNPCHero.heroInfo.defAreaName = _currentPickAreaName;
                _defAreaHeros[_currentPickNPCHero.heroInfo.defAreaName] = _currentPickNPCHero;
                _currentPickNPCHero.mapHero.unityGameObject.transform.position = _currentPickPosition;
                _currentPickNPCHero.heroInfo.position.x = _currentPickPosition.x;
                _currentPickNPCHero.heroInfo.position.y = _currentPickPosition.y;
                _currentPickNPCHero.heroInfo.position.z = _currentPickPosition.z;
            }
            _currentPickNPCHero = null;

            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.baseInfo.markDirty();
        }
        protected void OnUpdate()
        {
            if (_currentPickNPCHero == null)
            {
                return;
            }

            if(!UnityEngine.Input.GetMouseButton(0))
            {
                // mouse up
                onMouseUpCA();
                return;
            }

            // pick def area
            UnityEngine.RaycastHit[] hitInfos = UnityEngine.Physics.RaycastAll(
                UnityGameApp.Inst.MainScene.unityCamera.ScreenPointToRay(UnityEngine.Input.mousePosition),
                1000.0f,
                UnityEngine.LayerMask.GetMask("DefArea"));

            _currentPickAreaName = null;
            for (int i = 0; i < hitInfos.Length;)
            {
                _currentPickAreaName = hitInfos[i].collider.gameObject.name;
                _currentPickPosition = hitInfos[i].point;

                break;
            }

            if (_currentPickAreaName == null)
            {
                _atkRangeCircleVFX.ShowAttackRange(false); // can't put

                // pick ground position
                hitInfos = UnityEngine.Physics.RaycastAll(
                    UnityGameApp.Inst.MainScene.unityCamera.ScreenPointToRay(UnityEngine.Input.mousePosition),
                    1000.0f,
                    UnityEngine.LayerMask.GetMask("Ground"));

                if (hitInfos.Length > 0)
                {
                    _currentPickNPCHero.mapHero.unityGameObject.transform.position = new UnityEngine.Vector3(
                       hitInfos[0].point.x,
                       hitInfos[0].point.y + 0.5f,
                       hitInfos[0].point.z
                   );
                }
            }
            else
            {
                _atkRangeCircleVFX.ShowAttackRange(false); // can put

                _currentPickNPCHero.mapHero.unityGameObject.transform.position = new UnityEngine.Vector3(
                    _currentPickPosition.x,
                    _currentPickPosition.y + 0.5f,
                    _currentPickPosition.z
                );
            }
        }
    }
}
