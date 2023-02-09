using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;
using UnityEngine;
using Debug = MiniGameFramework.Debug;
using GameObject = MiniGameFramework.GameObject;

namespace UnityMiniGameFramework
{
    public class MapMonsterSpawn
    {
        protected Map _map;
        protected SpawnPos _spawnPos;

        protected bool _isSpawning;
        public bool isSpawning => _isSpawning;

        protected bool _isFinishSpawn;
        public bool isFinishSpawn => _isFinishSpawn;

        protected List<MapMonsterObject> _monsters;
        public List<MapMonsterObject> monsters => _monsters;

        protected MonsterSpawnConf _conf;
        public MonsterSpawnConf conf => _conf;

        protected MapMonsterObjectConf _monConf;
        protected UnityEngine.GameObject _unityMonsterPrefab;

        protected float _spawnCD;
        protected uint _totalSpawned;

        protected int _spawnMonsterLevel;

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
            _isFinishSpawn = false;
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

        public void SetSpawnMonsterLevel(int level)
        {
            _spawnMonsterLevel = level;
        }

        public void Reset()
        {
            _isSpawning = false;
            _isFinishSpawn = false;
            _totalSpawned = 0;
        }

        public void StartSpawn()
        {
            _isSpawning = true;
            _isFinishSpawn = false;
            _totalSpawned = 0;
        }

        public void StopSpawn()
        {
            _isSpawning = false;
            _isFinishSpawn = true;
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

        private static int _monsterCount = 0;
        public void OnUpdate()
        {
            if(!_isSpawning)
            {
                return;
            }

            if(_isFinishSpawn)
            {
                _monsterCount = 0;
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
                    _isFinishSpawn = true;
                    return;
                }

                _monsterCount++;
                _spawnSingleMonster();
            }
        }

        protected void _spawnSingleMonster()
        {
            var combatConf = UnityGameApp.Inst.MapManager.MapConf.getMapMonsterCombatLevelConf(_monConf.combatLevelConfName, _spawnMonsterLevel);
            if (combatConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"init monster spawn monster [{_conf.monsterConfName}] combat conf [{_monConf.combatLevelConfName}] level[{_spawnMonsterLevel}] not exist");
                return;
            }

            var unityMonsterObj = UnityEngine.GameObject.Instantiate(_unityMonsterPrefab);
            var mgObj = unityMonsterObj.GetComponent<UnityGameObjectBehaviour>();
            if (mgObj == null)
            {
                UnityEngine.GameObject.Destroy(unityMonsterObj);
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"init monster spawn monster [{_conf.monsterConfName}] prefab [{_monConf.prefabName}] without UnityGameObjectBehaviour");
                return;
            }

            mgObj.name = _conf.monsterConfName + _monsterCount;

            var mapMonsterObj = mgObj.mgGameObject as MapMonsterObject;
            if (mapMonsterObj == null)
            {
                mgObj.mgGameObject.Dispose();
                UnityEngine.GameObject.Destroy(unityMonsterObj);
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"init monster spawn monster [{_conf.monsterConfName}] prefab [{_monConf.prefabName}] not MapMonsterObject");
                return;
            }
            unityMonsterObj.layer = LayerMask.NameToLayer($"Monster");
            string str = $"level[{_spawnMonsterLevel}] hp[{combatConf.hpMax}] def[{combatConf.def}] att[{combatConf.attackBase}]";
            Debug.DebugOutput(DebugTraceType.DTT_Debug, $"spawn monster [{_conf.monsterConfName}] {str}");

            mapMonsterObj.setLevel(_spawnMonsterLevel);

            var combatComp = new CMCombatComponent();
            mapMonsterObj.AddComponent(combatComp);
            combatComp.Init(combatConf);
            combatComp.OnDie = MapMonsterObj_OnDie;
            combatComp.OnRecalcAttributes += mapMonsterObj.CombatComp_OnRecalcAttributes;

            var aiControlComp = new AIActorControllerComp();
            mapMonsterObj.AddComponent(aiControlComp);
            aiControlComp.Init(_monConf.aiStates);

            // add to scene
            unityMonsterObj.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);

            unityMonsterObj.transform.position = _spawnPos.randSpawnPos();

            mapMonsterObj.OnDispose += MapMonsterObj_OnDispose;

            _monsters.Add(mapMonsterObj);

            ++_totalSpawned;
        }


        private void MapMonsterObj_OnDie(ActorObject mon)
        {
            if(_map.currentLevel != null)
            {
                (_map.currentLevel as MapLevel).OnMonsterDie(mon as MapMonsterObject);
                PlayDieEff(mon);
            }
        }

        private void MapMonsterObj_OnDispose(GameObject obj)
        {
            _monsters.Remove(obj as MapMonsterObject);
        }

        public void PlayDieEff(ActorObject mon)
        {
            var dieEffect = UnityGameApp.Inst.VFXManager.createVFXObject("MonsterDie");
            if (dieEffect != null)
            {
                dieEffect.unityGameObject.SetActive(true);
                dieEffect.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);
                dieEffect.unityGameObject.transform.position = mon.unityGameObject.transform.position;
                dieEffect.unityGameObject.transform.localScale = new UnityEngine.Vector3(0.5f, 0.5f, 0.5f);
                dieEffect.Play();
            }
        }
    }

}
