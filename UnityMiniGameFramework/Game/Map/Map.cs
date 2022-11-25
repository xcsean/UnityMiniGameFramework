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

    public class MonsterSpawn
    {
        protected SpawnPos spawnPos;

        public bool Init(MonsterSpawnConf conf)
        {
            return true;
        }
    }

    public class Map : MGGameObject, IMap
    {
        override public string type => "MapObject";
        new public static Map create()
        {
            return new Map();
        }

        protected List<SpawnPos> _randBornPos;
        protected Dictionary<string, SpawnPos> _namedBornPos;

        protected List<MonsterSpawn> _monsterSpawns;

        public Map()
        {
            _randBornPos = new List<SpawnPos>();
            _namedBornPos = new Dictionary<string, SpawnPos>();

            _monsterSpawns = new List<MonsterSpawn>();
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

            MapConf conf = _getMapConf(confname);
            if (conf == null)
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Init map config({confname}) not exist.");
                return;
            }
            _name = conf.name;

            // init born pos
            foreach(var b in conf.randomBornObjectList)
            {
                SpawnPos sp = _getSpawnPosByObjectName(b);
                if(sp == null)
                {
                    continue;
                }

                _randBornPos.Add(sp);
            }
            foreach(var pair in conf.namedBornObjects)
            {
                SpawnPos sp = _getSpawnPosByObjectName(pair.Value);
                if(sp == null)
                {
                    continue;
                }

                _namedBornPos[pair.Key] = sp;
            }

            // init monster spawn
            foreach(var m in conf.monsterSpwanList)
            {
                MonsterSpawn ms = new MonsterSpawn();
                if(!ms.Init(m))
                {
                    continue;
                }
                _monsterSpawns.Add(ms);
            }
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

        public UnityEngine.Vector3 getRandomBornPos()
        {
            if(_randBornPos.Count <= 0)
            {
                return new UnityEngine.Vector3(0, 0, 0);
            }

            int i = UnityGameApp.Inst.Rand.RandomBetween(0, _randBornPos.Count);
            SpawnPos sp = _randBornPos[i];
            int x = (int)(sp.spawnHalfSize.x * 1000);
            float xf = (float)UnityGameApp.Inst.Rand.RandomBetween(-x, x) / 1000.0f;
            int y = (int)(sp.spawnHalfSize.y * 1000);
            float yf = (float)UnityGameApp.Inst.Rand.RandomBetween(-y, y) / 1000.0f;
            int z = (int)(sp.spawnHalfSize.z * 1000);
            float zf = (float)UnityGameApp.Inst.Rand.RandomBetween(-z, z) / 1000.0f;

            return sp.spawnCenter + new UnityEngine.Vector3(xf, yf, zf);
        }

        public UnityEngine.Vector3 getNamedBornPos(string name)
        {
            if (!_namedBornPos.ContainsKey(name))
            {
                return new UnityEngine.Vector3(0, 0, 0);
            }

            SpawnPos sp = _namedBornPos[name];
            int x = (int)(sp.spawnHalfSize.x * 1000);
            float xf = (float)UnityGameApp.Inst.Rand.RandomBetween(-x, x) / 1000.0f;
            int y = (int)(sp.spawnHalfSize.y * 1000);
            float yf = (float)UnityGameApp.Inst.Rand.RandomBetween(-y, y) / 1000.0f;
            int z = (int)(sp.spawnHalfSize.z * 1000);
            float zf = (float)UnityGameApp.Inst.Rand.RandomBetween(-z, z) / 1000.0f;

            return sp.spawnCenter + new UnityEngine.Vector3(xf, yf, zf);
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


        }
        override public void OnPostUpdate(float timeElasped)
        {


            base.OnPostUpdate(timeElasped);
        }
    }
}
