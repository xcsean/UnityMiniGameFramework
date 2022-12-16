using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class CMShootingLevel : MapLevel
    {
        UILevelMainPanel _levelUI;
        UIMainPanel _mainUI;

        public static CMShootingLevel create()
        {
            return new CMShootingLevel();
        }

        public override bool Init(string confName)
        {
            bool ret = base.Init(confName);

            _levelUI = UnityGameApp.Inst.UI.getUIPanel("LevelMainUI") as UILevelMainPanel;
            _mainUI = UnityGameApp.Inst.UI.getUIPanel("MainUI") as UIMainPanel;

            return ret;
        }

        public void SetDefenseLevelConf(CMDefenseLevelConf defLevelConf, int level)
        {
            int levelRange = defLevelConf.levelRangeMax - defLevelConf.levelRangeMin;

            for(int i=0; i< _monSpawns.Count; ++i)
            {
                var sp = _monSpawns[i];
                if(defLevelConf.monsterLvRanges != null && defLevelConf.monsterLvRanges.ContainsKey(sp.mapMonSpawn.conf.monsterConfName))
                {
                    var monLvRange = defLevelConf.monsterLvRanges[sp.mapMonSpawn.conf.monsterConfName];
                    int monLv = monLvRange.levelRangeMin;
                    if (levelRange > 0)
                    {
                        // calc monster level
                        monLv = monLvRange.levelRangeMin + (monLvRange.levelRangeMax - monLvRange.levelRangeMin) * (level-1) / levelRange;
                    }
                    sp.mapMonSpawn.SetSpawnMonsterLevel(monLv);
                }
                else
                {
                    sp.mapMonSpawn.SetSpawnMonsterLevel(1);
                }
            }
        }

        public override void OnUpdate(float timeElasped)
        {
            base.OnUpdate(timeElasped);

            //_levelUI.levelStateControl.timeLeftText.text = this.timeLeft.ToString();
            _mainUI.refreshLevelInfo(this);
        }


        override public void OnMonsterDie(MapMonsterObject mon)
        {
            base.OnMonsterDie(mon);

            // read drop from config and do drop
            var drop = (UnityGameApp.Inst.Game as ChickenMasterGame).gameConf.getMonsterDrops(mon.name, mon.level);

            // TO DO : do drop

            // for Debug ...
            (UnityGameApp.Inst.Game as ChickenMasterGame).Self.AddBackpackProduct("meat", 10);
            (UnityGameApp.Inst.Game as ChickenMasterGame).Self.AddExp(10);
        }
        public override void OnMapNPCTriggerEnter(string tirggerObjName, MapNPCObject npcObj, UnityEngine.Collider other)
        {
        }
        public override void OnMapNPCTriggerExit(string tirggerObjName, MapNPCObject npcObj, UnityEngine.Collider other)
        {
        }

        public override void OnMapLevelTriggerEnter(string triggerObjName, UnityEngine.Collider other)
        {
            if(triggerObjName == "EggTrigger")
            {
                _OnEggTriggerEnter(other);
            }
        }
        public override void OnMapLevelTriggerExit(string triggerObjName, UnityEngine.Collider other)
        {
        }

        protected void _OnEggTriggerEnter(UnityEngine.Collider other)
        {
            var ugObj = other.gameObject.GetComponent<UnityGameObjectBehaviour>();
            if(ugObj == null)
            {
                return;
            }

            var monObj = ugObj.mgGameObject as MapMonsterObject;
            if (monObj == null)
            {
                // not monster
                return;
            }

            // sub egg hp
            (UnityGameApp.Inst.Game as ChickenMasterGame).Egg.subHp();

            // monster reach egg
            AniVanishAct va = new AniVanishAct(monObj);
            va.setVanishAnimaionName(ActAnis.JumpAni);
            monObj.actionComponent.AddAction(va);

        }

        protected override bool _checkFinish()
        {
            if((UnityGameApp.Inst.Game as ChickenMasterGame).Egg.HP <= 0)
            {
                // egg hp <= 0, failed
                _OnLose();
                return true;
            }

            return base._checkFinish();
        }

        protected override void _OnTimeUp()
        {

        }

        protected override void _OnWin()
        {
            // TO DO : show win ui, give reward

            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var bi = (cmGame.baseInfo.getData() as LocalBaseInfo);

            bi.currentLevel++;
            if(bi.currentLevel > cmGame.gameConf.maxDefenseLevelCount)
            {
                // max level reached
                bi.currentLevel = cmGame.gameConf.maxDefenseLevelCount;
            }

            _mainUI.refreshCurrentLevel(bi.currentLevel);

            cmGame.baseInfo.markDirty();
        }

        protected override void _OnLose()
        {

        }
    }
}
