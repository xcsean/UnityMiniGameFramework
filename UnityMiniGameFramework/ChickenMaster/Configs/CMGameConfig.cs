using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Text.Json;
using System.Threading.Tasks;

using MiniGameFramework;
using UnityEngine;

namespace UnityMiniGameFramework
{
    public class CMGunLevelConf
    {
        public int upgrageCostItemCost { get; set; }

        public AttackConf attack { get; set; }

        public float rangeAdd { get; set; }

        public int IncreasedAttackSpeed { get; set; }
    }

    public class CMGunConf
    {
        public int id { get; set; }
        public string weaponIcon { get; set; }
        public string prefabName { get; set; }
        public string name { get; set; }

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
        public string halfHead { get; set; }
        public int userLevelRequire { get; set; }
        public int initGunId { get; set; }
        public int initGunLevel { get; set; }
        public string initSpawnPosName { get; set; }
        public string initDefAreaName { get; set; }

        public int activateGoldCost { get; set; }
        public Dictionary<int, CMHeroLevelConf> levelConf { get; set; }

        public List<int> guns { get; set; }

        public List<MapConfAIState> aiStates { get; set; }
    }

    public class CMFactoryLevelConf
    {
        public int maxInputProductStore { get; set; }
        public int maxOutputProductStore { get; set; }
        public float produceCD { get; set; }

        public int costInputCount { get; set; }
        public int produceOutputCount { get; set; }

        public int fetchPackCount { get; set; }

        public int upgradeGoldCost { get; set; }
    }

    public class CMFactoryConf
    {
        public string mapBuildName { get; set; }
        public string factoryText { get; set; }
        public string factoryName { get; set; }
        public int userLevelRequire { get; set; }

        public int activateGoldCost { get; set; }

        public string inputProductName { get; set; }
        public string outputProductName { get; set; }

        public string inputPutPosName { get; set; }
        public string inputStorePosName { get; set; }
        public string outputFetchingPosName { get; set; }
        public string outputStorePosName { get; set; }
        public string inputStorePrefabPath { get; set;}
        public string outputStorePrefabPath { get; set; }
        public JsonConfVector2 TopUIOffset { get; set; }

        public Dictionary<int, CMFactoryLevelConf> levelConfs { get; set; }
    }

    public class CMWorkerConf
    {
        public string mapNpcName { get; set; }
        public string initSpawnPosName { get; set; }

        public string fetchingAniName { get; set; }
        public string putAniName { get; set; }
        public string carryMovingAniName { get; set; }

        public Dictionary<int, int> levelCarryCount { get; set; }

        public string productBone { get; set; }
        public JsonConfVector3 productPos { get; set; }
        public string productPrefab { get; set; }
    }

    public class CMStoreHouseLevelConf
    {
        public int MaxstoreCount { get; set; }
        public int fetchPackCount { get; set; }
        public int upgradeGoldCost { get; set; }
    }

    public class CMStoreHouseConf
    {
        public string mapBuildName { get; set; }

        public string fetchingPosName { get; set; }
        public string storePosName { get; set; }

        public string storeProductName { get; set; }

        public Dictionary<int, CMStoreHouseLevelConf> levelConfs { get; set; }

        public CMWorkerConf workerConf { get; set; }
    }

    public class CMTrainStationLevelConf
    {
        public int MaxstoreCount { get; set; }
        public int maxSellCountPerRound { get; set; }
        public int upgradeGoldCost { get; set; }
    }

    public class CMTrainStationConf
    {
        public string mapBuildName { get; set; }

        public string trainMapNpcName { get; set; }

        public string putPosName { get; set; }
        public string storePosName { get; set; }

        public string trainStartPosName { get; set; }
        public string trainStopPosName { get; set; }

        public string trainMoveoutPosName { get; set; }

        public float trainOnboardTime { get; set; }
        public float trainArriveTime { get; set; }

        public Dictionary<int, CMTrainStationLevelConf> levelConfs { get; set; }

        public CMWorkerConf workerConf { get; set; }
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

    public class CMItemAward
    {
        public string itemName { get; set; }

        public int count { get; set; }
    }

    public class CMProductAward
    {
        public string productName { get; set; }

        public int count { get; set; }
    }

    public class CMDefenseLevelAward
    {
        public int gold { get; set; }
        public int exp { get; set; }

        public List<CMItemAward> items { get; set; }
    }

    public class CMDefenseLevelConf
    {
        public int levelDivide { get; set; }
        public int level { get; set; }
        public int levelRangeMin { get; set; }
        public int levelRangeMax { get; set; }

        public Dictionary<string, CMDefenseLevelMonsterLvRange> monsterLvRanges { get; set; }

        public string mapLevelName { get; set; }
        public Dictionary<string, int> passReward { get; set; }
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

    public class CMOfflineItemAward
    {
        public string itemName { get; set; }
        public float countPerSec { get; set; }
    }
    public class CMOfflineProductAward
    {
        public string productName { get; set; }
        public float countPerSec { get; set; }
    }

    public class CMOfflineAwardConf
    {
        public float goldPerM { get; set; }
        public float expPerM { get; set; }
        public int maxTime { get; set; }
        public List<CMOfflineProductAward> products { get; set; }
    }

    public class CMBuffsConf
    {
        public CMSingleBuffConf doubleExp { get; set; }
        public CMSingleBuffConf doubleAtk { get; set; }
        public CMSingleBuffConf trainProterSpeed { get; set; }
        public CMSingleBuffConf storehouseProterSpeed { get; set; }
        public CMSingleBuffConf factory1Productivity { get; set; }
        public CMSingleBuffConf factory2Productivity { get; set; }
        public CMSingleBuffConf factory3Productivity { get; set; }
        public CMSingleBuffConf factory4Productivity { get; set; }
        public CMSingleBuffConf factory5Productivity { get; set; }
        public CMSingleBuffConf factory6Productivity { get; set; }
    }

    public class CMSingleBuffConf
    {
        public int videoGet { get; set; }
        public int maxBuff { get; set; }
    }

    public class CMSkillLevelConf
    {
        public int buff { get; set; }
        public int upgradeGold { get; set; }
    }


    public class CMSkillConf
    {
        public int id { get; set; }
        public string title { get; set; }
        public string icon { get; set; }
        public string desc { get; set; }
        public string type { get; set; }
        public Dictionary<int, CMSkillLevelConf> levelsConf { get; set; }
    }

    [Serializable]
    public class CMGameConf
    {
        public string levelCenterObjectName { get; set; }

        public float autoSaveTime { get; set; }

        public float offlineAwardMinTime { get; set; }

        public Dictionary<int, CMGunConf> gunConfs { get; set; }

        public Dictionary<string, CMHeroConf> heros { get; set; }

        public Dictionary<string, CMFactoryConf> factories { get; set; }

        public Dictionary<string, CMProductConf> products { get; set; }

        public CombatConf selfCombatConf { get; set; }

        public Dictionary<int, int> levelUpExpRequire { get; set; }

        public List<CMDefenseLevelConf> defenseLevels { get; set; }

        public Dictionary<int, CMDefenseLevelAward> defenseFCLevelAwards { get; set; }

        public Dictionary<string, Dictionary<int, CMMonsterDropConf>> monsterDrops { get; set; }

        public CMEggConf eggConf { get; set; }

        public CMStoreHouseConf storeHouseConf { get; set; }

        public CMTrainStationConf trainStationConf { get; set; }

        public Dictionary<int, CMOfflineAwardConf> offlineAwardsByUserLevel { get; set; }

        public CMBuffsConf buffsConf { get; set; }

        public Dictionary<int, CMSkillConf> skillsConf { get; set; }
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
            //return JsonSerializer.Deserialize<CMGameConf>(confStr);
            return JsonUtil.FromJson<CMGameConf>(confStr);
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

            // TO DO : change config to make it efficient
            while(monLevel > 0)
            {
                var drops = gameConfs.monsterDrops[mapMonsterName];
                if (drops.ContainsKey(monLevel))
                {
                    return drops[monLevel];
                }

                --monLevel;
            }
            return null;
        }
        public CMDefenseLevelAward getLevelFirstCompleteAward(int currentLevel)
        {
            if (gameConfs.defenseFCLevelAwards == null || !gameConfs.defenseFCLevelAwards.ContainsKey(currentLevel))
            {
                return null;
            }
            return gameConfs.defenseFCLevelAwards[currentLevel];
        }
    }
}
