using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{

    public class MapLevel : IMapLevel
    {
        protected class LevelSpawn
        {
            public MapMonsterSpawn mapMonSpawn;
            public MapLevelSpawn levelSpawnConf;
            public float startTimeLeft;
        }

        protected Map _map;
        public IMap map => _map;

        protected bool _isStarted;
        public bool isStarted => _isStarted;

        protected float _timeLeft;
        public float timeLeft => _timeLeft;

        protected MapLevelConf _conf;
        protected List<LevelSpawn> _monSpawns;

        public MapLevel()
        {
        }

        public void setMap(Map m)
        {
            _map = m;
        }

        virtual public bool Init(string confName)
        {
            _conf = UnityGameApp.Inst.MapManager.MapConf.getMapLevelConf(confName);
            if(_conf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Map level init config [{confName}] not exist");
                return false;
            }

            _monSpawns = new List<LevelSpawn>();
            foreach(var levelSpawn in _conf.monsterSpawns)
            {
                var sp = _map.getMonsterSpawn(levelSpawn.mapMonsterSpawnName);
                if(sp == null)
                {
                    Debug.DebugOutput(DebugTraceType.DTT_Error, $"Map [{_map.name}] level [{confName}] get monster spawn [{levelSpawn.mapMonsterSpawnName}] failed");
                    continue;
                }

                _monSpawns.Add(new LevelSpawn()
                {
                    mapMonSpawn = sp,
                    levelSpawnConf = levelSpawn,
                    startTimeLeft = levelSpawn.startTime
                });; 
            }

            _isStarted = false;

            return true;
        }

        virtual public void Start()
        {
            _timeLeft = _conf.levelTime;
            foreach (var sp in _monSpawns)
            {
                sp.startTimeLeft = sp.levelSpawnConf.startTime;
            }

            _isStarted = true;
        }

        virtual public void Finish()
        {
            foreach (var sp in _monSpawns)
            {
                sp.mapMonSpawn.StopSpawn();
                sp.mapMonSpawn.ClearAllMonsters();
            }
            _isStarted = false;
        }

        protected bool _checkFinish()
        {
            if(_timeLeft <= 0)
            {
                return true;
            }

            // TO DO : check win or other finish condition

            return false;
        }

        virtual public void OnUpdate(float timeElasped)
        {
            if(!_isStarted)
            {
                return;
            }

            for (int i=0; i< _monSpawns.Count; ++i)
            {
                var sp = _monSpawns[i];
                if (sp.mapMonSpawn.isSpawning)
                {
                    continue;
                }

                sp.startTimeLeft -= UnityEngine.Time.deltaTime;
                if (sp.startTimeLeft > 0)
                {
                    continue;
                }

                sp.mapMonSpawn.StartSpawn();
            }

            _timeLeft -= UnityEngine.Time.deltaTime;

            if (_checkFinish())
            {
                Finish();
            }
        }

        virtual public void OnPostUpdate(float timeElasped)
        {

        }

    }
}
