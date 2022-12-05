using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class MapHeroObjectConf
    {
        public string actorConfName { get; set; }

        public string prefabName { get; set; }
    }

    public class MapBuildObjectConf
    {
        public string actorConfName { get; set; }

        public string uiPanelName { get; set; }

        public string triggerObjectName { get; set; }
    }

    public class MapMonsterAIConf
    {

    }

    public class CombatConf
    {
        public int hpMax { get; set; }
        public int def { get; set; }

        public int hpAddPerLevel { get; set; }
        public int defAddPerLevel { get; set; }
    }

    public class AttackConf
    {
        public int attackMin { get; set; }
        public int attackMax { get; set; }
        public int attackMinAddPerLevel { get; set; }
        public int attackMaxAddPerLevel { get; set; }

        public int? missingRate { get; set; }
        public int? criticalHitRate { get; set; }
        public int? criticalHitPer { get; set; }
    }

    public class MapMonsterObjectConf
    {
        public string actorConfName { get; set; }

        public string prefabName { get; set; }

        public MapMonsterAIConf ai { get; set; }

        public CombatConf combat { get; set; }
    }
}
