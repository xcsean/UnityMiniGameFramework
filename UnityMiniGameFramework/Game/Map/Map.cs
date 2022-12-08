using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class SpawnPos
    {
        public UnityEngine.GameObject spawnObject;
        public UnityEngine.Vector3 spawnCenter;
        public UnityEngine.Vector3 spawnHalfSize;
    }

    public class Map : MGGameObject, IMap
    {
        override public string type => "MapObject";

        protected MapLevel _mapLevel;
        public IMapLevel currentLevel => _mapLevel;

        protected MapConf _conf;
        public MapConf mapConf => _conf;

        new public static Map create()
        {
            return new Map();
        }

        protected List<SpawnPos> _randBornPos;
        protected Dictionary<string, SpawnPos> _namedBornPos;

        protected Dictionary<string, MapMonsterSpawn> _monsterSpawns;
        public Dictionary<string, MapMonsterSpawn> monsterSpawns => _monsterSpawns;


        public Map()
        {
            _randBornPos = new List<SpawnPos>();
            _namedBornPos = new Dictionary<string, SpawnPos>();

            _monsterSpawns = new Dictionary<string, MapMonsterSpawn>();
        }


        virtual protected MapConf _getMapConf(string confname)
        {
            if (UnityGameApp.Inst.MapManager.MapConf == null)
            {
                return null;
            }
            return UnityGameApp.Inst.MapManager.MapConf.getMapConf(confname);
        }

        override public void Init(string confname)
        {
            base.Init(confname);

            _conf = _getMapConf(confname);
            if (_conf == null)
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Init map config({confname}) not exist.");
                return;
            }
            _name = _conf.name;

            // init born pos
            foreach(var b in _conf.randomBornObjectList)
            {
                SpawnPos sp = _getSpawnPosByObjectName(b);
                if(sp == null)
                {
                    MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Init map config({confname}) spawn pos ({b}) not exist.");
                    continue;
                }

                _randBornPos.Add(sp);
            }
            foreach(var pair in _conf.namedBornObjects)
            {
                SpawnPos sp = _getSpawnPosByObjectName(pair.Value);
                if(sp == null)
                {
                    MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Init map config({confname}) spawn pos ({pair.Value}) not exist.");
                    continue;
                }

                _namedBornPos[pair.Key] = sp;
            }

            // init monster spawn
            foreach(var pair in _conf.monsterSpawns)
            {
                SpawnPos sp = _getSpawnPosByObjectName(pair.Value.spawnObjectName);
                if (sp == null)
                {
                    MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Init map config({confname}) spawn pos ({pair.Value.spawnObjectName}) not exist.");
                    continue;
                }

                MapMonsterSpawn ms = new MapMonsterSpawn(this, sp);
                if(!ms.Init(pair.Value))
                {
                    continue;
                }
                _monsterSpawns[pair.Key] = ms;
            }
        }

        public MapMonsterSpawn getMonsterSpawn(string name)
        {
            MapMonsterSpawn ms;
            if (_monsterSpawns.TryGetValue(name, out ms))
            {
                return ms;
            }

            return null;
        }

        virtual public void OnLeave()
        {
            this.unityGameObject.SetActive(false);
        }

        virtual public void OnEnter()
        {
            this.unityGameObject.SetActive(true);
        }

        protected SpawnPos _getSpawnPosByObjectName(string objName)
        {
            UnityEngine.Transform tr = _unityGameObject.transform.Find(objName);
            if(tr == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Map [{_name}] _getSpawnPosByObjectName [{objName}] not exist");
                return null;
            }

            UnityEngine.BoxCollider bc = tr.gameObject.GetComponent<UnityEngine.BoxCollider>();
            if (bc == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Map [{_name}] _getSpawnPosByObjectName [{objName}] box collider not exist");
                return null;
            }

            SpawnPos pos = new SpawnPos();
            pos.spawnObject = tr.gameObject;
            pos.spawnCenter = bc.center + tr.position;
            pos.spawnHalfSize = bc.size / 2;

            return pos;
        }

        public UnityEngine.Vector3 getBornPos(SpawnPos sp)
        {
            int x = (int)(sp.spawnHalfSize.x * 1000);
            float xf = (float)UnityGameApp.Inst.Rand.RandomBetween(-x, x) / 1000.0f;
            int y = (int)(sp.spawnHalfSize.y * 1000);
            float yf = (float)UnityGameApp.Inst.Rand.RandomBetween(-y, y) / 1000.0f;
            int z = (int)(sp.spawnHalfSize.z * 1000);
            float zf = (float)UnityGameApp.Inst.Rand.RandomBetween(-z, z) / 1000.0f;

            return sp.spawnCenter + new UnityEngine.Vector3(xf, yf, zf);
        }

        public UnityEngine.Vector3 getRandomBornPos()
        {
            if(_randBornPos.Count <= 0)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Map [{_name}] random pos count <= 0");
                return new UnityEngine.Vector3(0, 0, 0);
            }

            int i = UnityGameApp.Inst.Rand.RandomBetween(0, _randBornPos.Count);
            SpawnPos sp = _randBornPos[i];
            return getBornPos(sp);
        }

        public UnityEngine.Vector3 getNamedBornPos(string name)
        {
            if (!_namedBornPos.ContainsKey(name))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Map [{_name}] named born pos [{name}] not exist");
                return new UnityEngine.Vector3(0, 0, 0);
            }

            SpawnPos sp = _namedBornPos[name];
            return getBornPos(sp);
        }

        override protected void _onAddComponent(IGameObjectComponent comp)
        {
        }

        override public void Dispose()
        {
            base.Dispose();
        }

        override public void OnUpdate(float timeElasped)
        {
            base.OnUpdate(timeElasped);

            foreach (var ms in _monsterSpawns)
            {
                ms.Value.OnUpdate();
            }

            if(_mapLevel != null)
            {
                _mapLevel.OnUpdate(timeElasped);
            }
        }
        override public void OnPostUpdate(float timeElasped)
        {
            base.OnPostUpdate(timeElasped);

            if(_mapLevel != null)
            {
                _mapLevel.OnPostUpdate(timeElasped);
            }
        }

        public IMapLevel CreateLevel(string levelName)
        {
            var levelConf = UnityGameApp.Inst.MapManager.MapConf.getMapLevelConf(levelName);
            if(levelConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Map [{_name}] create level [{levelName}] config not exist");
                return null;
            }

            _mapLevel = UnityGameApp.Inst.MapManager.createMapLevel(levelConf.levelType) as MapLevel;
            if(_mapLevel == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Map [{_name}] create level [{levelName}] type [{levelConf.levelType}] not exist");
                return null;
            }
            _mapLevel.setMap(this);

            if (!_mapLevel.Init(levelName))
            {
                _mapLevel = null;
                return null;
            }

            return _mapLevel;
        }
    }
}
