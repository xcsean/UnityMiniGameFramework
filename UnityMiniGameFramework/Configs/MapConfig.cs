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

    public class MonsterSpawnConf
    {
        public string spawnObjectName { get; set; }
        public uint maxCount { get; set; }
        public uint maxTotalCount { get; set; }

        public float spawnInterval { get; set; }

        public uint perSpawnCount { get; set; }

        public string monsterConfName { get; set; }
    }

    public class MapZoneConf
    {
        public string zoneName { get; set; }

    }

    public class PathConf
    {
        public string pathNode { get; set; }
        public int pathNodeCount { get; set; }
    }

    public class MapConf
    {
        public string name { get; set; }

        public string prefabName { get; set; }

        public List<string> randomBornObjectList { get; set; }
        public Dictionary<string, string> namedBornObjects { get; set; }

        public List<MapZoneConf> zones { get; set; }

        public List<string> npcs { get; set; }

        public List<string> buildings { get; set; }

        public Dictionary<string, MonsterSpawnConf> monsterSpawns { get; set; }

        public Dictionary<string, PathConf> paths { get; set; }
    }

    public class MapLevelSpawn
    {
        public string mapMonsterSpawnName { get; set; }
        public float startTime { get; set; }
    }

    public class MapLevelKMWinCheckConf
    {
        public string mapMonsterName { get; set; }
        public int killCount { get; set; }
    }

    public class MapLevelConf
    {
        public List<MapLevelSpawn> monsterSpawns { get; set; }

        public float levelTime { get; set; }

        public string levelType { get; set; }

        public List<MapLevelKMWinCheckConf> kmWinCheck { get; set; }
    }

    [Serializable]
    public class MapConfs
    {
        public Dictionary<string, MapConf> maps { get; set; }

        public Dictionary<string, MapHeroObjectConf> mapHeros { get; set; }

        public Dictionary<string, MapBuildObjectConf> mapBuildings { get; set; }

        public Dictionary<string, MapMonsterObjectConf> mapMonsters { get; set; }

        public Dictionary<string, MapNPCObjectConf> mapNpcs { get; set; }

        public Dictionary<string, MapLevelConf> mapLevels { get; set; }

        public Dictionary<string, MapMonsterCombatLevelConf> mapMonsterCombatLevelConf { get; set; }
    }

    public class MapConfig : JsonConfig
    {
        override public string type => "MapConfig";
        public static MapConfig create()
        {
            return new MapConfig();
        }

        public MapConfs mapConf => (MapConfs)_conf;

        override protected object _JsonDeserialize(string confStr)
        {
            //return JsonSerializer.Deserialize<MapConfs>(confStr);
            return JsonUtil.FromJson<MapConfs>(confStr);
        }

        public MapConf getMapConf(string mapName)
        {
            if (mapConf.maps == null || !mapConf.maps.ContainsKey(mapName))
            {
                return null;
            }
            return mapConf.maps[mapName];
        }
        public MapHeroObjectConf getMapHeroConf(string mapHeroName)
        {
            if (mapConf.mapHeros == null || !mapConf.mapHeros.ContainsKey(mapHeroName))
            {
                return null;
            }
            return mapConf.mapHeros[mapHeroName];
        }
        public MapBuildObjectConf getMapBuildingConf(string mapBuildingName)
        {
            if (mapConf.mapBuildings == null || !mapConf.mapBuildings.ContainsKey(mapBuildingName))
            {
                return null;
            }
            return mapConf.mapBuildings[mapBuildingName];
        }
        public MapMonsterObjectConf getMapMonsterConf(string mapMonstername)
        {
            if (mapConf.mapMonsters == null || !mapConf.mapMonsters.ContainsKey(mapMonstername))
            {
                return null;
            }
            return mapConf.mapMonsters[mapMonstername];
        }
        public MapNPCObjectConf getMapNPCConf(string maNPCname)
        {
            if (mapConf.mapNpcs == null || !mapConf.mapNpcs.ContainsKey(maNPCname))
            {
                return null;
            }
            return mapConf.mapNpcs[maNPCname];
        }
        public MapLevelConf getMapLevelConf(string mapLevelName)
        {
            if (mapConf.mapLevels == null || !mapConf.mapLevels.ContainsKey(mapLevelName))
            {
                return null;
            }
            return mapConf.mapLevels[mapLevelName];
        }

        public CombatConf getMapMonsterCombatLevelConf(string n, int level)
        {
            if (mapConf.mapMonsterCombatLevelConf == null || !mapConf.mapMonsterCombatLevelConf.ContainsKey(n))
            {
                return null;
            }
            var mmclConf = mapConf.mapMonsterCombatLevelConf[n];
            if(!mmclConf.levelCombatConf.ContainsKey(level))
            {
                return null;
            }

            return mmclConf.levelCombatConf[level];
        }
    }
}
