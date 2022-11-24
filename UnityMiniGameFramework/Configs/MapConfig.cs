using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class MonsterSpawnConf
    {
        public string spawnObjectName { get; set; }
        public uint maxCount { get; set; }

        public uint respawnTime { get; set; }

        public string monsterConfName { get; set; }
    }

    public class MapConf
    {
        public string name { get; set; }

        public string prefabName { get; set; }

        public List<string> randomBornObjectList { get; set; }
        public Dictionary<string, string> namedBornObjects { get; set; }

        public List<MonsterSpawnConf> monsterSpwanList { get; set; }

    }

    public class MapConfs
    {
        public Dictionary<string, MapConf> maps { get; set; }

        public Dictionary<string, MapHeroObjectConf> mapHeros { get; set; }

        public Dictionary<string, MapBuildObjectConf> mapBuildings { get; set; }
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
            return JsonSerializer.Deserialize<MapConfs>(confStr);
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
    }
}
