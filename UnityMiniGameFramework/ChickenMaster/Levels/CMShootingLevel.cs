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

        public static CMShootingLevel create()
        {
            return new CMShootingLevel();
        }

        public override bool Init(string confName)
        {
            bool ret = base.Init(confName);

            _levelUI = UnityGameApp.Inst.UI.getUIPanel("LevelMainUI") as UILevelMainPanel;

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
                        monLv = monLvRange.levelRangeMin + (monLvRange.levelRangeMax - monLvRange.levelRangeMin) * level / levelRange;
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

            _levelUI.levelStateControl.timeLeftText.text = this.timeLeft.ToString();
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

        protected override void _OnTimeUp()
        {

        }

        protected override void _OnWin()
        {

        }

        protected override void _OnLose()
        {

        }
    }
}
