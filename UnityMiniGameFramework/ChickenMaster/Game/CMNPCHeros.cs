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
        }

        protected override void _initAdditionalComponent()
        {
            var aiControlComp = new AIActorControllerComp();
            _mapHeroObj.AddComponent(aiControlComp);
            aiControlComp.Init(_conf.ai); // TO DO : add config

            _fireTargetAI = new AIGunFireTarget(_mapHeroObj);
            _fireTargetAI.setGun(_gun);
            aiControlComp.AddAIState(_fireTargetAI);
        }

        protected override void _initCombatConfig()
        {
            _combatComp.Init(_conf.combatConf);
        }

        protected override void _initPosition(UnityEngine.GameObject unityHeroObj, LocalHeroInfo heroInfo)
        {
            if (heroInfo.position == null)
            {
                // set born position
                //unityHeroObj.transform.position = UnityGameApp.Inst.MainScene.implMap.getRandomBornPos();
                unityHeroObj.transform.position = UnityGameApp.Inst.MainScene.implMap.getNamedBornPos(_conf.initSpawnPosName);
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
            if (heroInfo.holdWeapon == null)
            {
                var weaponInfo = new LocalWeaponInfo()
                {
                    id = _conf.initGunId,
                    level = _conf.initGunLevel
                };

                heroInfo.holdWeapon = weaponInfo;
            }
        }
    }
}
