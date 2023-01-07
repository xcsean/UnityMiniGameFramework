using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    abstract public class CMHeros
    {
        protected MapHeroObject _mapHeroObj;
        public MapHeroObject mapHero => _mapHeroObj;

        protected CMCombatComponent _combatComp;
        public CMCombatComponent combatComp => _combatComp;

        protected GunObject _gun;
        public GunObject gun => _gun;

        protected LocalHeroInfo _heroInfo;
        public LocalHeroInfo heroInfo => _heroInfo;

        protected ChickenMasterGame _cmGame;


        public CMHeros()
        {

        }

        protected virtual void _Init(LocalHeroInfo heroInfo)
        {
            _initMapHero(heroInfo);
        }

        protected virtual void _initMapHero(LocalHeroInfo heroInfo)
        {
            _heroInfo = heroInfo;
            var heroConf = UnityGameApp.Inst.MapManager.MapConf.getMapHeroConf(heroInfo.mapHeroName);

            // create hero
            var unityHeroObj = UnityGameApp.Inst.UnityResource.LoadUnityPrefabObject(heroConf.prefabName);
            unityHeroObj = UnityEngine.GameObject.Instantiate(unityHeroObj);
            var mgObj = unityHeroObj.GetComponent<UnityGameObjectBehaviour>();
            if (mgObj == null)
            {
                UnityEngine.GameObject.Destroy(unityHeroObj);
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMHeros init map hero prefab [{heroConf.prefabName}] without UnityGameObjectBehaviour");
                return;
            }

            _mapHeroObj = mgObj.mgGameObject as MapHeroObject;
            if (_mapHeroObj == null)
            {
                mgObj.mgGameObject.Dispose();
                UnityEngine.GameObject.Destroy(unityHeroObj);
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMHeros init map hero prefab [{heroConf.prefabName}] not MapHeroObject");
                return;
            }

            _combatComp = new CMCombatComponent();
            _mapHeroObj.AddComponent(_combatComp);
            _initCombatConfig();
            _combatComp.hpBar.hide();
            _combatComp.OnRecalcAttributes += _combatComp_OnRecalcAttributes;

            // add to scene
            unityHeroObj.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);

            _initPosition(unityHeroObj, heroInfo);

            _initWeapon(heroInfo);

            onChangeGun();

            _initAdditionalComponent();
        }

        protected virtual void _initAdditionalComponent()
        {
        }

        protected virtual void _initCombatConfig()
        {
        }

        protected virtual void _initPosition(UnityEngine.GameObject unityHeroObj, LocalHeroInfo heroInfo)
        {
        }

        protected virtual void _initWeapon(LocalHeroInfo heroInfo)
        {
        }

        public void onChangeGun()
        {
            if (_gun != null)
            {
                _gun.Dispose();
                UnityEngine.GameObject.Destroy(_gun.unityGameObject);
                _gun = null;
            }

            if(_heroInfo.holdWeaponId == 0)
            {
                return;
            }

            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            var cmGunConf = cmGame.gameConf.getCMGunConf(_heroInfo.holdWeaponId);
            if (cmGunConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMHeros init gun id[{_heroInfo.holdWeaponId}] not exist");
                return;
            }

            var unitGunObj = UnityGameApp.Inst.UnityResource.LoadUnityPrefabObject(cmGunConf.prefabName);
            unitGunObj = UnityEngine.GameObject.Instantiate(unitGunObj);
            var mgObj = unitGunObj.GetComponent<UnityGameObjectBehaviour>();
            if (mgObj == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMHeros init gun prefab [{cmGunConf.prefabName}] without UnityGameObjectBehaviour");
                return;
            }

            _gun = mgObj.mgGameObject as GunObject;
            if (_gun == null)
            {
                mgObj.mgGameObject.Dispose();
                UnityEngine.GameObject.Destroy(unitGunObj);
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMHeros init gun prefab [{cmGunConf.prefabName}] not GunObject");
                return;
            }

            // attach gun to hero
            string attachToName;
            if (_gun.conf.attachToBone != null)
            {
                // attach to node indicate by gun config
                attachToName = _gun.conf.attachToBone;
            }
            else
            {
                // attach to default bone
                attachToName = "";
            }

            var trAttachTo = _mapHeroObj.unityGameObject.transform.Find(attachToName);
            if (trAttachTo == null)
            {
                _gun.Dispose();
                UnityEngine.GameObject.Destroy(_gun.unityGameObject);
                _gun = null;

                Debug.DebugOutput(DebugTraceType.DTT_Error, $"gun attach to hero[{_mapHeroObj.name}] bone [{trAttachTo}] not exist");
                return;
            }
            _gun.unityGameObject.transform.SetParent(trAttachTo);
            _gun.unityGameObject.transform.localScale = UnityEngine.Vector3.one;

            if (_gun.conf.attachPos != null)
            {
                _gun.unityGameObject.transform.localPosition = new UnityEngine.Vector3(_gun.conf.attachPos.x, _gun.conf.attachPos.y, _gun.conf.attachPos.z);
            }
            else
            {
                _gun.unityGameObject.transform.localPosition = UnityEngine.Vector3.zero;
            }

            if (_gun.conf.attachRot != null)
            {
                _gun.unityGameObject.transform.localRotation = UnityEngine.Quaternion.Euler(_gun.conf.attachRot.x, _gun.conf.attachRot.y, _gun.conf.attachRot.z);
            }
            else
            {
                _gun.unityGameObject.transform.forward = trAttachTo.forward;
            }

            // init gun level
            _recalcAttack();
        }

        protected virtual void _recalcAttack()
        {
            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            var cmGunConf = cmGame.gameConf.getCMGunConf(_heroInfo.holdWeaponId);

            var weaponInfo = cmGame.GetWeaponInfo(_heroInfo.holdWeaponId);

            CMGunLevelConf gunLevelConf = null;
            if (cmGunConf.gunLevelConf.TryGetValue(weaponInfo.level, out gunLevelConf))
            {
                _gun.initAttack(gunLevelConf.attack);
                if (gunLevelConf.rangeAdd > 0)
                {
                    _gun.AddAttackRange(gunLevelConf.rangeAdd);
                }
            }
            else
            {
                // no gun level config
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"gun id[{_heroInfo.holdWeaponId}] level [{weaponInfo.level}] config not exist");
            }
        }

        private void _combatComp_OnRecalcAttributes()
        {
            _recalcAttack();

            var bufAttrs = _combatComp.bufAttrs.ToArray();

            // calc attack
            long nowMillisecond = (long)(DateTime.Now.Ticks / 10000);
            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            LocalBaseInfo bi = cmGame.baseInfo.getData() as LocalBaseInfo;
            float extraAtkMul = bi.buffs.doubleAtk > nowMillisecond ? 1f : 0;
            _gun.onRecalcAttributes(bufAttrs, extraAtkMul);

            // calc speed
            _mapHeroObj.moveAct.onRecalcAttributes(bufAttrs);
        }
    }
}
