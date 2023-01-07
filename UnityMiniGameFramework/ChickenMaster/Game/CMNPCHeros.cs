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
        // 选中拖动
        public bool isPicked;

        public void Init(LocalHeroInfo heroInfo)
        {
            _cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            _conf = _cmGame.gameConf.getCMHeroConf(heroInfo.mapHeroName);
            if(_conf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"CMNPCHeros init with [{heroInfo.mapHeroName}] config not exist");
                return;
            }

            _Init(heroInfo);

            if(heroInfo.defAreaName == null)
            {
                heroInfo.defAreaName = _conf.initDefAreaName;
            }

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
                // for Debug ...
                cmGame.uiMainPanel.NofityMessage(CMGNotifyType.CMG_ERROR, "insuffcient gold !");

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
        
        public CMHeroLevelConf getNextHeroLevelConf()
        {
            CMHeroLevelConf conf;
            _conf.levelConf.TryGetValue(_heroInfo.level + 1, out conf);
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

            // calc attack
            var bufAttrs = _combatComp.bufAttrs.ToArray();
            long nowMillisecond = (long)(DateTime.Now.Ticks / 10000);
            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            LocalBaseInfo bi = cmGame.baseInfo.getData() as LocalBaseInfo;
            float extraAtkMul = bi.buffs.doubleAtk > nowMillisecond ? 1f : 0;
            _gun.onRecalcAttributes(bufAttrs, extraAtkMul);
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
            var gunInfo = _cmGame.GetWeaponInfo(weaponId);
            if(gunInfo == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Npc hero [{this._heroInfo.mapHeroName}] change weapon id[{weaponId}] not exist");
                return;
            }

            _heroInfo.holdWeaponId = weaponId;
            this.onChangeGun();

            _fireTargetAI.setGun(_gun);
        }

        public bool TryActiveWeapon(int weaponId)
        {
            var cmGunConf = _cmGame.gameConf.getCMGunConf(weaponId);
            var itemInfo = _cmGame.Self.GetBackpackItemInfo(cmGunConf.upgradeItemName);
            if (itemInfo != null)
            {
                // try active
                if (itemInfo.count >= cmGunConf.activateItemCost)
                {
                    if (cmGunConf.activateItemCost == _cmGame.Self.SubBackpackItem(cmGunConf.upgradeItemName, cmGunConf.activateItemCost))
                    {
                        // sub success

                        // active
                        _cmGame.AddNewWeaponInfo(weaponId);
                        return true;
                    }
                }
                else
                {
                    // not enough item
                    _cmGame.uiMainPanel.NofityMessage(CMGNotifyType.CMG_ERROR, $"insuffcient {cmGunConf.upgradeItemName} for activate");
                }
            }
            else
            {
                // not enough item

                // for Debug ...
                _cmGame.uiMainPanel.NofityMessage(CMGNotifyType.CMG_ERROR, $"insuffcient {cmGunConf.upgradeItemName} for activate");
            }

            return false;
        }

        public bool TryUpgradeWeapon(int weaponId)
        {
            var gunInfo = _cmGame.GetWeaponInfo(weaponId);
            var cmGunConf = _cmGame.gameConf.getCMGunConf(weaponId);
            var gunLevelConf = cmGunConf.gunLevelConf[gunInfo.level];
            
            if(!cmGunConf.gunLevelConf.ContainsKey(gunInfo.level+1))
            {
                // max level

                // for Debug ...
                _cmGame.uiMainPanel.NofityMessage(CMGNotifyType.CMG_ERROR, $"Already max level");

                return false;
            }

            var itemInfo = _cmGame.Self.GetBackpackItemInfo(cmGunConf.upgradeItemName);
            if (itemInfo != null)
            {
                // try upgrade
                if (itemInfo.count >= gunLevelConf.upgrageCostItemCost)
                {
                    if (gunLevelConf.upgrageCostItemCost == _cmGame.Self.SubBackpackItem(cmGunConf.upgradeItemName, gunLevelConf.upgrageCostItemCost))
                    {
                        // sub success

                        // upgrade
                        gunInfo.level++;

                        if(weaponId == _heroInfo.holdWeaponId)
                        {
                            _recalcAttack();
                        }

                        return true;
                    }
                }
                else
                {
                    // not enough item

                    // for Debug ...
                    _cmGame.uiMainPanel.NofityMessage(CMGNotifyType.CMG_ERROR, $"insuffcient {cmGunConf.upgradeItemName} for upgrade");
                }
            }
            else
            {
                // not enough item

                // for Debug ...
                _cmGame.uiMainPanel.NofityMessage(CMGNotifyType.CMG_ERROR, $"insuffcient {cmGunConf.upgradeItemName} for upgrade");
            }

            return false;
        }
    }
}
