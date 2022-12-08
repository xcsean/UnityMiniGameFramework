using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class MapMonsterSpawn
    {
        protected Map _map;
        protected SpawnPos _spawnPos;

        protected bool _isSpawning;
        public bool isSpawning => _isSpawning;

        protected List<MapMonsterObject> _monsters;
        public List<MapMonsterObject> monsters => _monsters;

        protected MonsterSpawnConf _conf;
        protected MapMonsterObjectConf _monConf;
        protected UnityEngine.GameObject _unityMonsterPrefab;

        protected float _spawnCD;
        protected uint _totalSpawned;

        public MapMonsterSpawn(Map map, SpawnPos sp)
        {
            _map = map;
            _spawnPos = sp;

            _monsters = new List<MapMonsterObject>();
        }

        public bool Init(MonsterSpawnConf conf)
        {
            _conf = conf;
            _isSpawning = false;
            _totalSpawned = 0;

            _monConf = UnityGameApp.Inst.MapManager.MapConf.getMapMonsterConf(_conf.monsterConfName);
            if(_monConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"init monster spawn monster [{_conf.monsterConfName}] config not exist");
                return false;
            }

            _unityMonsterPrefab = UnityGameApp.Inst.UnityResource.LoadUnityPrefabObject(_monConf.prefabName);
            if (_unityMonsterPrefab == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"init monster spawn monster [{_conf.monsterConfName}] prefab [{_monConf.prefabName}] not exist");
                return false;
            }

            return true;
        }

        public void StartSpawn()
        {
            _isSpawning = true;
            _totalSpawned = 0;
        }

        public void StopSpawn()
        {
            _isSpawning = false;
        }

        public void ClearAllMonsters()
        {
            foreach(var mon in _monsters)
            {
                UnityEngine.GameObject.Destroy(mon.unityGameObject);
                mon.OnDispose -= MapMonsterObj_OnDispose;
                mon.Dispose();
            }
            _monsters.Clear();
        }

        public void OnUpdate()
        {
            if(!_isSpawning)
            {
                return;
            }

            _spawnCD -= UnityEngine.Time.deltaTime;
            if (_spawnCD > 0)
            {
                return;
            }

            _spawnCD = _conf.spawnInterval;

            for(uint i=0; i< _conf.perSpawnCount; ++i)
            {
                if (_monsters.Count >= _conf.maxCount)
                {
                    return;
                }

                if (_totalSpawned >= _conf.maxTotalCount)
                {
                    return;
                }

                _spawnSingleMonster();
            }
        }

        protected void _spawnSingleMonster()
        {
            var unityMonsterObj = UnityEngine.GameObject.Instantiate(_unityMonsterPrefab);
            var mgObj = unityMonsterObj.GetComponent<UnityGameObjectBehaviour>();
            if (mgObj == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"init monster spawn monster [{_conf.monsterConfName}] prefab [{_monConf.prefabName}] without UnityGameObjectBehaviour");
                return;
            }

            var mapMonsterObj = mgObj.mgGameObject as MapMonsterObject;
            if (mapMonsterObj == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"init monster spawn monster [{_conf.monsterConfName}] prefab [{_monConf.prefabName}] not MapMonsterObject");
                return;
            }

            var aiControlComp = new AIActorControllerComp();
            mapMonsterObj.AddComponent(aiControlComp);
            aiControlComp.Init(_monConf.ai); // TO DO : add config


            var combatComp = new CMCombatComponent();
            mapMonsterObj.AddComponent(combatComp);
            combatComp.Init(_monConf.combat);

            // for Debug ...
            // trace and attack
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var traceAi = new AITrace(mapMonsterObj);
            traceAi.setTraceTarget(cmGame.Self.selfMapHero.unityGameObject);
            aiControlComp.AddAIState(traceAi);
            var tryAttackAi = new AITryAttack(mapMonsterObj);
            tryAttackAi.setTraceTarget(cmGame.Self.selfMapHero.unityGameObject);
            aiControlComp.AddAIState(tryAttackAi);

            // add to scene
            unityMonsterObj.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);

            unityMonsterObj.transform.position = _map.getBornPos(_spawnPos);

            mapMonsterObj.OnDispose += MapMonsterObj_OnDispose;

            _monsters.Add(mapMonsterObj);

            ++_totalSpawned;
        }

        private void MapMonsterObj_OnDispose(GameObject obj)
        {
            _monsters.Remove(obj as MapMonsterObject);
        }
    }

}
