using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class CMGunLevelConf
    {
        public int upgrageCostItemCost { get; set; }

        public AttackConf attack { get; set; }

        public float rangeAdd { get; set; }
    }

    public class CMGunConf
    {
        public int id { get; set; }
        public string prefabName { get; set; }

        public string upgradeItemName { get; set; }
        public int activateItemCost { get; set; }

        public Dictionary<int, CMGunLevelConf> gunLevelConf { get; set; }
    }

    public class CMHeroLevelConf
    {
        public CombatConf combatConf { get; set; }

        public int upgradeGoldCost { get; set; }
    }

    public class CMHeroConf
    {
        public string mapHeroName { get; set; }
        public int userLevelRequire { get; set; }
        public int initGunId { get; set; }
        public int initGunLevel { get; set; }
        public string initSpawnPosName { get; set; }

        public int activateGoldCost { get; set; }
        public Dictionary<int, CMHeroLevelConf> levelConf { get; set; }

        public List<int> guns { get; set; }

        public List<MapConfAIState> aiStates { get; set; }
    }

    public class CMProductMakerConf
    {
        public int factoryLevelRequire { get; set; }
        public string inputProductName { get; set; }
        public string outputProductName { get; set; }

        public int costInputCount { get; set; }
        public int produceOutputCount { get; set; }


        public int costInputCountAddPerLevel { get; set; }
        public int produceOutputCountAddPerLevel { get; set; }
    }

    public class CMFactoryConf
    {
        public string mapBuildName { get; set; }
        public int userLevelRequire { get; set; }

        public int maxInputProductStore { get; set; }
        public int maxOutputProductStore { get; set; }
        public float produceCD { get; set; }

        public int maxInputProductStoreAddPerLevel { get; set; }
        public int maxOutputProductStoreAddPerLevel { get; set; }

        public float produceCDAddPerLevel { get; set; }

        public int activateGoldCost { get; set; }
        public Dictionary<int, int> upgradeGoldCostPerLevel { get; set; }

        public List<CMProductMakerConf> productMaker { get; set; }
    }

    public class CMProductConf
    {
        public string productName { get; set; }

        public int price { get; set; } 

    }

    public class CMDefenseLevelMonsterLvRange
    {
        public int levelRangeMin { get; set; }
        public int levelRangeMax { get; set; }
    }

    public class CMDefenseLevelConf
    {
        public int levelRangeMin { get; set; }
        public int levelRangeMax { get; set; }

        public Dictionary<string, CMDefenseLevelMonsterLvRange> monsterLvRanges { get; set; }

        public string mapLevelName { get; set; }
    }

    public class CMDropRoll
    {
        public int rate { get; set; }

        public int min { get; set; }
        public int max { get; set; }
    }

    public class CMNamedDropSet
    {
        public int rate { get; set; }
        public int min { get; set; }
        public int max { get; set; }

        public string name { get; set; }
    }

    public class CMMonsterDropConf
    {
        public CMDropRoll exp { get; set; }
        public CMDropRoll gold { get; set; }
        public List<CMNamedDropSet> product { get; set; }
        public List<CMNamedDropSet> item { get; set; }
    }

    public class CMEggConf
    {
        public string EggObjectName { get; set; }
        public int maxHp { get; set; }
        public int recoverTime { get; set; }
        public int hpIncTime { get; set; }
        public JsonConfVector2 EggUIOffset { get; set; }
        public float EggUIShowRange { get; set; }
    }

    public class CMGameConf
    {
        public string levelCenterObjectName { get; set; }

        public Dictionary<int, CMGunConf> gunConfs { get; set; }

        public Dictionary<string, CMHeroConf> heros { get; set; }

        public Dictionary<string, CMFactoryConf> factories { get; set; }

        public Dictionary<string, CMProductConf> products { get; set; }

        public CombatConf selfCombatConf { get; set; }

        public Dictionary<int, int> levelUpExpRequire { get; set; }

        public List<CMDefenseLevelConf> defenseLevels { get; set; }

        public Dictionary<string, Dictionary<int, CMMonsterDropConf>> monsterDrops { get; set; }

        public CMEggConf eggConf { get; set; }
    }

    public class CMGameConfig : JsonConfig
    {
        override public string type => "CMGameConfig";
        public static CMGameConfig create()
        {
            return new CMGameConfig();
        } 
        
        public CMGameConf gameConfs => (CMGameConf)_conf;

        protected int _maxDefenseLevelCount;
        public int maxDefenseLevelCount => _maxDefenseLevelCount;

        public override void Init(string filename, string n)
        {
            base.Init(filename, n);

            // format config
            _maxDefenseLevelCount = 0;
            for (int i=0; i< gameConfs.defenseLevels.Count; ++i)
            {
                if(_maxDefenseLevelCount < gameConfs.defenseLevels[i].levelRangeMax)
                {
                    _maxDefenseLevelCount = gameConfs.defenseLevels[i].levelRangeMax;
                }
            }
        }

        override protected object _JsonDeserialize(string confStr)
        {
            return JsonSerializer.Deserialize<CMGameConf>(confStr);
        }

        public CMGunConf getCMGunConf(int cmGunID)
        {
            if (gameConfs.gunConfs == null || !gameConfs.gunConfs.ContainsKey(cmGunID))
            {
                return null;
            }
            return gameConfs.gunConfs[cmGunID];
        }

        public CMHeroConf getCMHeroConf(string mapHeroName)
        {
            if (gameConfs.heros == null || !gameConfs.heros.ContainsKey(mapHeroName))
            {
                return null;
            }
            return gameConfs.heros[mapHeroName];
        }
        public CMFactoryConf getCMFactoryConf(string mapBuildingName)
        {
            if (gameConfs.factories == null || !gameConfs.factories.ContainsKey(mapBuildingName))
            {
                return null;
            }
            return gameConfs.factories[mapBuildingName];
        }
        public CMProductConf getCMProductConf(string productName)
        {
            if (gameConfs.products == null || !gameConfs.products.ContainsKey(productName))
            {
                return null;
            }
            return gameConfs.products[productName];
        }
        public int getLevelUpExpRequire(int currentLevel)
        {
            if (gameConfs.levelUpExpRequire == null || !gameConfs.levelUpExpRequire.ContainsKey(currentLevel))
            {
                return 0;
            }
            return gameConfs.levelUpExpRequire[currentLevel];
        }
        public CMMonsterDropConf getMonsterDrops(string mapMonsterName, int monLevel)
        {
            if (gameConfs.monsterDrops == null || !gameConfs.monsterDrops.ContainsKey(mapMonsterName))
            {
                return null;
            }
            var drops = gameConfs.monsterDrops[mapMonsterName];
            if(!drops.ContainsKey(monLevel))
            {
                return null;
            }
            return drops[monLevel];
        }
    }
}
