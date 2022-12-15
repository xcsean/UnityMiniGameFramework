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

    public class MapNPCObjectConf
    {
        public string actorConfName { get; set; }

        public string prefabName { get; set; }

        public string defaultAniName { get; set; }

        public List<MapConfAIState> aiStates { get; set; }

    }

    public class MapBuildObjectConf
    {
        public string actorConfName { get; set; }

        public string uiPanelName { get; set; }

        public string triggerObjectName { get; set; }
    }

    public class MapConfAIState
    {
        public string aiType { get; set; }

        public string targetName { get; set; }

        public string aiStateConfName { get; set; }
    }

    public class CombatConf
    {
        public int hpMax { get; set; }
        public int def { get; set; }

        public int attackBase { get; set; }
    }

    public class AttackConf
    {
        public int attackMin { get; set; }
        public int attackMax { get; set; }

        public int? missingRate { get; set; }
        public int? criticalHitRate { get; set; }
        public float? criticalHitPer { get; set; }
    }

    public class MapMonsterCombatLevelConf
    {
        public Dictionary<int,CombatConf> levelCombatConf { get; set; }
    }

    public class MapMonsterObjectConf
    {
        public string actorConfName { get; set; }

        public string prefabName { get; set; }

        public List<MapConfAIState> aiStates { get; set; }

        public string combatLevelConfName { get; set; }
    }
}
