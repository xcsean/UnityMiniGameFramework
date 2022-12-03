using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class SelfControl
    {
        bool _isInited;

        MapHeroObject _selfMapHeroObj;
        InputActorControllerComp _inputControl;
        CMSelfComponent _selfCombatComp;
        UIMainPanel _uiMainPanel;
        RigibodyMoveAct _movAct;

        GunObject _gun;
        
        public MapHeroObject selfMapHero => _selfMapHeroObj;

        public SelfControl()
        {
            _isInited = false;
        }

        public void Init()
        {
            _uiMainPanel = UnityGameApp.Inst.UI.getUIPanel("MainUI") as UIMainPanel;

            _initSelfMapHero();
        }

        protected void _initSelfMapHero()
        {
            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            var heroConf = UnityGameApp.Inst.MapManager.MapConf.getMapHeroConf(cmGame.baseInfo.hero.mapHeroName);

            // create hero
            var unityHeroObj = UnityGameApp.Inst.UnityResource.LoadUnityPrefabObject(heroConf.prefabName);
            unityHeroObj = UnityEngine.GameObject.Instantiate(unityHeroObj);
            var mgObj = unityHeroObj.GetComponent<UnityGameObjectBehaviour>();
            if(mgObj == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"SelfControl init map hero prefab [{heroConf.prefabName}] without UnityGameObjectBehaviour");
                return;
            }

            _selfMapHeroObj = mgObj.mgGameObject as MapHeroObject;
            if (_selfMapHeroObj == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"SelfControl init map hero prefab [{heroConf.prefabName}] not MapHeroObject");
                return;
            }

            _inputControl = new InputActorControllerComp();
            _selfMapHeroObj.AddComponent(_inputControl);
            _inputControl.Init(null); // TO DO : add config

            _selfCombatComp = new CMSelfComponent();
            _selfMapHeroObj.AddComponent(_selfCombatComp);
            _selfCombatComp.Init(null); // TO DO : add config

            _movAct = new RigibodyMoveAct(_selfMapHeroObj);
            _selfMapHeroObj.actionComponent.AddAction(_movAct);

            // add to scene
            unityHeroObj.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);

            if(cmGame.baseInfo.hero.position == null)
            {                
                // set born position
                //unityHeroObj.transform.position = UnityGameApp.Inst.MainScene.implMap.getRandomBornPos();
                unityHeroObj.transform.position = UnityGameApp.Inst.MainScene.implMap.getNamedBornPos("b1");
            }
            else
            {
                // restore position
                unityHeroObj.transform.position = new UnityEngine.Vector3(cmGame.baseInfo.hero.position.x, unityHeroObj.transform.position.y, cmGame.baseInfo.hero.position.z);
            }

            UnityGameApp.Inst.MainScene.camera.follow(_selfMapHeroObj);

            // create weapon
            LocalWeaponInfo weaponInfo;
            if (cmGame.baseInfo.hero.holdWeapon != null)
            {
                weaponInfo = cmGame.baseInfo.hero.holdWeapon;
            }
            else
            {
                weaponInfo = new LocalWeaponInfo()
                {
                    id = 1,
                    level = 1
                };

                cmGame.baseInfo.hero.holdWeapon = weaponInfo;
            }

            _onChangeGun();

            _isInited = true;
        }

        protected void _onChangeGun()
        {
            if(_gun != null)
            {
                _gun.Dispose();
                UnityEngine.GameObject.Destroy(_gun.unityGameObject);
                _gun = null;
            }

            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            var cmGunConf = cmGame.gameConf.getCMGunConf(cmGame.baseInfo.hero.holdWeapon.id);
            if(cmGunConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"init self gun id[{cmGame.baseInfo.hero.holdWeapon.id}] not exist");
                return;
            }

            var unitGunObj = UnityGameApp.Inst.UnityResource.LoadUnityPrefabObject(cmGunConf.prefabName);
            unitGunObj = UnityEngine.GameObject.Instantiate(unitGunObj);
            var mgObj = unitGunObj.GetComponent<UnityGameObjectBehaviour>();
            if (mgObj == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"SelfControl init gun prefab [{cmGunConf.prefabName}] without UnityGameObjectBehaviour");
                return;
            }

            _gun = mgObj.mgGameObject as GunObject;
            if (_gun == null)
            {
                mgObj.mgGameObject.Dispose();
                UnityEngine.GameObject.Destroy(unitGunObj);
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"SelfControl init gun prefab [{cmGunConf.prefabName}] not GunObject");
                return;
            }

            // attach gun to hero
            string attachToName;
            if(_gun.conf.attachToBone != null)
            {
                // attach to node indicate by gun config
                attachToName = _gun.conf.attachToBone;
            }
            else
            {
                // attach to default bone
                attachToName = "";
            }

            var trAttachTo = _selfMapHeroObj.unityGameObject.transform.Find(attachToName);
            if(trAttachTo == null)
            {
                _gun.Dispose();
                UnityEngine.GameObject.Destroy(_gun.unityGameObject);
                _gun = null;

                Debug.DebugOutput(DebugTraceType.DTT_Error, $"gun attach to hero[{_selfMapHeroObj.name}] bone [{trAttachTo}] not exist");
                return;
            }
            _gun.unityGameObject.transform.SetParent(trAttachTo);
            _gun.unityGameObject.transform.forward = trAttachTo.forward;
            _gun.unityGameObject.transform.localPosition = UnityEngine.Vector3.zero;

            // TO DO : init gun combat attribute
        }

        public void OnUpdate()
        {
            if(!_isInited)
            {
                return;
            }

            if(_uiMainPanel.Joystick.isMoving)
            {
                // TO DO : use rigibody mov
                //_movAct.moveTo(_selfMapHeroObj.unityGameObject.transform.position + _uiMainPanel.Joystick.movVector3 * 10.0f);
                _movAct.moveToward(_uiMainPanel.Joystick.movVector3);

                // for Debug ...
                if(_gun != null)
                {
                    _gun.Fire();
                }
            }
            else if(_movAct.isMoving)
            {
                _movAct.stop();

                // for Debug ...
                if (_gun != null)
                {
                    _gun.StopFire();
                }
            }
        }
    }
}
