using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class CMNPCHeros : CMHeros
    {
        protected CMHeroConf _conf;
        protected AIGunFireTarget _fireTargetAI;

        public void Init(LocalHeroInfo heroInfo)
        {
            _conf = (UnityGameApp.Inst.Game as ChickenMasterGame).gameConf.getCMHeroConf(heroInfo.mapHeroName);
            if(_conf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMNPCHeros init with [{heroInfo.mapHeroName}] config not exist");
                return;
            }

            _Init(heroInfo);

            _mapHeroObj.OnMapLevelFinish += _mapHeroObj_OnMapLevelFinish;
        }

        private void _mapHeroObj_OnMapLevelFinish()
        {
            _fireTargetAI.clearTarget();
        }

        public int getUpgradeGoldCost()
        {
            CMHeroLevelConf conf;
            if(_conf.levelConf.TryGetValue(_heroInfo.level, out conf))
            {
                return conf.upgradeGoldCost;
            }
            return 0;
        }

        public bool TryUpgrade()
        {
            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;

            if (_heroInfo.level >= cmGame.Self.userLevel)
            {
                // can't bigger than user level
                return false;
            }

            int upgradeGold = getUpgradeGoldCost();
            if(upgradeGold <= 0)
            {
                // no more level
                return false;
            }

            if (cmGame.Self.TrySubGold(upgradeGold))
            {
                // upgrade 
                _recalcAttack();

                _heroInfo.level = _heroInfo.level + 1;
                cmGame.baseInfo.markDirty();
            }
            else
            {
                // TO DO : not enough gold
                return false;
            }

            return true;
        }

        public CMHeroLevelConf getCurrentHeroLevelConf()
        {
            CMHeroLevelConf conf;
            _conf.levelConf.TryGetValue(_heroInfo.level, out conf);
            return conf;
        }

        protected override void _initAdditionalComponent()
        {
            var aiControlComp = new AIActorControllerComp();
            _mapHeroObj.AddComponent(aiControlComp);
            aiControlComp.Init(_conf.aiStates); // TO DO : add config

            _fireTargetAI = new AIGunFireTarget(_mapHeroObj);
            _fireTargetAI.setGun(_gun);
            aiControlComp.AddAIState(_fireTargetAI);
        }

        protected override void _initCombatConfig()
        {
            CMHeroLevelConf conf;
            if (_conf.levelConf.TryGetValue(_heroInfo.level, out conf))
            {
                _combatComp.Init(conf.combatConf);
            }
            else
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMNPCHeros init with [{heroInfo.mapHeroName}] level [{_heroInfo.level}] config not exist");
            }
        }

        protected override void _recalcAttack()
        {
            base._recalcAttack();

            CMHeroLevelConf conf;
            if (_conf.levelConf.TryGetValue(_heroInfo.level, out conf))
            {
                // attack min/max add with hero base attack
                _gun.addAttackInfo(new WeaponAttack()
                {
                    attackMin = conf.combatConf.attackBase,
                    attackMax = conf.combatConf.attackBase
                });
            }
        }

        protected override void _initPosition(UnityEngine.GameObject unityHeroObj, LocalHeroInfo heroInfo)
        {
            var npc = (UnityGameApp.Inst.MainScene.map as Map).getNPC(heroInfo.mapHeroName);
            if (npc != null)
            {
                // hide npc
                npc.Hide();
            }

            if (heroInfo.position == null)
            {
                // set born position
                ////unityHeroObj.transform.position = UnityGameApp.Inst.MainScene.implMap.getRandomBornPos();
                //unityHeroObj.transform.position = UnityGameApp.Inst.MainScene.implMap.getNamedBornPos(_conf.initSpawnPosName);
                unityHeroObj.transform.position = UnityEngine.Vector3.zero;
                if (npc != null)
                {
                    unityHeroObj.transform.position = npc.unityGameObject.transform.position;
                }
                heroInfo.position = new JsonConfVector3()
                {
                    x = unityHeroObj.transform.position.x,
                    y = unityHeroObj.transform.position.y,
                    z = unityHeroObj.transform.position.z,
                };
            }
            else
            {
                // restore position
                unityHeroObj.transform.position = new UnityEngine.Vector3(heroInfo.position.x, unityHeroObj.transform.position.y, heroInfo.position.z);
            }
        }

        protected override void _initWeapon(LocalHeroInfo heroInfo)
        {
            // create weapon
            if (heroInfo.holdWeaponId == 0)
            {
                heroInfo.holdWeaponId = _conf.initGunId;

                var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
                var weaponInfo = cmGame.AddNewWeaponInfo(_heroInfo.holdWeaponId);
                weaponInfo.level = _conf.initGunLevel;
            }
        }

        public void ChangeWeapon(int weaponId)
        {

        }

        public void AddWeapon(int weaponId)
        {

        }

        public void UpgradeWeapon(int weaponId)
        {

        }
    }
}
