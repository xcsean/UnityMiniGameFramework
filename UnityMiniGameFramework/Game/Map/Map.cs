﻿using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Debug = MiniGameFramework.Debug;

namespace UnityMiniGameFramework
{
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

        protected Dictionary<string, MapNPCObject> _npcs;
        public Dictionary<string, MapNPCObject> npcs => _npcs;

        protected Dictionary<string, MapBuildingObject> _buildings;
        public Dictionary<string, MapBuildingObject> buildings => _buildings;

        protected Dictionary<string, MapDefAreaObject> _defAreas;
        public Dictionary<string, MapDefAreaObject> defAreas => _defAreas;

        protected Dictionary<string, List<UnityEngine.Vector3>> _paths;

        protected HashSet<MapActorObject> _mapActors;

        protected Rect _activeRect;

        public Rect ActiveRect => _activeRect;

        public Map()
        {
            _randBornPos = new List<SpawnPos>();
            _namedBornPos = new Dictionary<string, SpawnPos>();

            _monsterSpawns = new Dictionary<string, MapMonsterSpawn>();

            _npcs = new Dictionary<string, MapNPCObject>();
            _buildings = new Dictionary<string, MapBuildingObject>();
            _defAreas = new Dictionary<string, MapDefAreaObject>();
            _paths = new Dictionary<string, List<UnityEngine.Vector3>>();

            _mapActors = new HashSet<MapActorObject>();
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
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error,
                    $"Init map config({confname}) not exist.");
                return;
            }

            _name = _conf.name;

            // init born pos
            foreach (var b in _conf.randomBornObjectList)
            {
                SpawnPos sp = getSpawnPosByObjectName(b);
                if (sp == null)
                {
                    MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error,
                        $"Init map config({confname}) spawn pos ({b}) not exist.");
                    continue;
                }

                _randBornPos.Add(sp);
            }

            foreach (var pair in _conf.namedBornObjects)
            {
                SpawnPos sp = getSpawnPosByObjectName(pair.Value);
                if (sp == null)
                {
                    MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error,
                        $"Init map config({confname}) spawn pos ({pair.Value}) not exist.");
                    continue;
                }

                _namedBornPos[pair.Key] = sp;
            }

            // init monster spawn
            foreach (var pair in _conf.monsterSpawns)
            {
                SpawnPos sp = getSpawnPosByObjectName(pair.Value.spawnObjectName);
                if (sp == null)
                {
                    MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error,
                        $"Init map config({confname}) spawn pos ({pair.Value.spawnObjectName}) not exist.");
                    continue;
                }

                MapMonsterSpawn ms = new MapMonsterSpawn(this, sp);
                if (!ms.Init(pair.Value, pair.Key))
                {
                    continue;
                }

                _monsterSpawns[pair.Key] = ms;
            }

            float _x = _conf.activeArea[0][0];
            float _y = _conf.activeArea[0][1];
            float _width = Math.Abs(_conf.activeArea[3][0] - _x);
            float _height = Math.Abs(_conf.activeArea[3][1] - _y);

            _activeRect = new Rect(_x, _y, _width, _height);

            // init paths
            // if(_conf.paths != null)
            // {
            //     foreach(var pathPair in _conf.paths)
            //     {
            //         var tr = this._unityGameObject.transform.Find(pathPair.Value.pathNode);
            //         if (tr == null)
            //         {
            //             MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Init map config({_conf.name}) path ({pathPair.Value.pathNode}) not exist.");
            //             continue;
            //         }
            //     
            //         var pathList = new List<UnityEngine.Vector3>();
            //         for(int i=1; i<= pathPair.Value.pathNodeCount; ++i)
            //         {
            //             var trn = tr.Find((i).ToString());
            //             if (trn == null)
            //             {
            //                 MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Init map config({_conf.name}) path node({i}) not exist.");
            //                 continue;
            //             }
            //     
            //             pathList.Add(trn.position);
            //         }
            //         _paths[pathPair.Key] = pathList;
            //     }
            // }
        }

        public override void PostInit()
        {
            base.PostInit();

            // init npc
            if (_conf.npcs != null)
            {
                foreach (var npcObjName in _conf.npcs)
                {
                    var tr = this._unityGameObject.transform.Find(npcObjName);
                    if (tr == null)
                    {
                        MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error,
                            $"Init map config({_conf.name}) npc ({npcObjName}) not exist.");
                        continue;
                    }

                    var ugo = tr.gameObject.GetComponent<UnityGameObjectBehaviour>();
                    if (ugo == null)
                    {
                        MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error,
                            $"Init map config({_conf.name}) npc ({npcObjName}) no UnityGameObjectBehaviour.");
                        continue;
                    }

                    var npcObj = ugo.mgGameObject as MapNPCObject;
                    if (npcObj == null)
                    {
                        MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error,
                            $"Init map config({_conf.name}) npc ({npcObjName}) not MapNPCObject.");
                        continue;
                    }

                    this._npcs[npcObj.name] = npcObj;
                }
            }

            // init buildings
            if (_conf.buildings != null)
            {
                foreach (var buildObjName in _conf.buildings)
                {
                    var tr = this._unityGameObject.transform.Find(buildObjName);
                    if (tr == null)
                    {
                        MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error,
                            $"Init map config({_conf.name}) building ({buildObjName}) not exist.");
                        continue;
                    }

                    var ugo = tr.gameObject.GetComponent<UnityGameObjectBehaviour>();
                    if (ugo == null)
                    {
                        MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error,
                            $"Init map config({_conf.name}) building ({buildObjName}) no UnityGameObjectBehaviour.");
                        continue;
                    }

                    var buildingObj = ugo.mgGameObject as MapBuildingObject;
                    if (buildingObj == null)
                    {
                        MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error,
                            $"Init map config({_conf.name}) building ({buildObjName}) not MapBuildingObject.");
                        continue;
                    }

                    this._buildings[buildingObj.name] = buildingObj;
                }
            }

            // init defAreas
            if (_conf.defAreas != null)
            {
                foreach (var areaObjName in _conf.defAreas)
                {
                    var tr = this._unityGameObject.transform.Find(areaObjName);
                    if (tr == null)
                    {
                        MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error,
                            $"Init map config({_conf.name}) defArea ({areaObjName}) not exist.");
                        continue;
                    }

                    var ugo = tr.gameObject.GetComponent<UnityGameObjectBehaviour>();
                    if (ugo == null)
                    {
                        MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error,
                            $"Init map config({_conf.name}) defArea ({areaObjName}) no UnityGameObjectBehaviour.");
                        continue;
                    }

                    var defareaObj = ugo.mgGameObject as MapDefAreaObject;
                    if (defareaObj == null)
                    {
                        MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error,
                            $"Init map config({_conf.name}) defArea ({areaObjName}) not MapBuildingObject.");
                        continue;
                    }

                    this._defAreas[defareaObj.unityGameObject.name] = defareaObj;
                }
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

        public SpawnPos getSpawnPosByObjectName(string objName)
        {
            UnityEngine.Transform tr = _unityGameObject.transform.Find(objName);
            if (tr == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error,
                    $"Map [{_name}] _getSpawnPosByObjectName [{objName}] not exist");
                return null;
            }

            UnityEngine.BoxCollider bc = tr.gameObject.GetComponent<UnityEngine.BoxCollider>();
            if (bc == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error,
                    $"Map [{_name}] _getSpawnPosByObjectName [{objName}] box collider not exist");
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
            if (_randBornPos.Count <= 0)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Map [{_name}] random pos count <= 0");
                return new UnityEngine.Vector3(0, 0, 0);
            }

            int i = UnityGameApp.Inst.Rand.RandomBetween(0, _randBornPos.Count);
            SpawnPos sp = _randBornPos[i];
            return sp.randSpawnPos();
        }

        public UnityEngine.Vector3 getNamedBornPos(string name)
        {
            if (!_namedBornPos.ContainsKey(name))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Map [{_name}] named born pos [{name}] not exist");
                return new UnityEngine.Vector3(0, 0, 0);
            }

            SpawnPos sp = _namedBornPos[name];
            return sp.randSpawnPos();
        }

        public MapNPCObject getNPC(string npcName)
        {
            MapNPCObject n = null;
            _npcs.TryGetValue(npcName, out n);
            return n;
        }

        public List<UnityEngine.Vector3> getPath(string pathName)
        {
            List<UnityEngine.Vector3> n;
            if (!_paths.ContainsKey(pathName))
            {
                OnInitPath(pathName);
            }

            _paths.TryGetValue(pathName, out n);
            return n;
        }

        public void ClearPath()
        {
            _paths.Clear();
        }
        private void OnInitPath(string pathName)
        {
            if (!_monsterSpawns.ContainsKey(pathName))
                return;
            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            var sp = _monsterSpawns[pathName];
            Vector2Int spInitPos = AstarUtility.GetLogicPos(sp.SpawnPos.spawnObject.transform.position);
            List<AStarPathFinding.ResultPoint> resultPoints =
                AStarPathFinding.GetInstance().GoToTargetPosSync(spInitPos, cmGame.Egg.LogicPos);
            List<Vector3> pathPos = new List<Vector3>();
            int n = resultPoints.Count;
            // 出生点会随机，所以去掉第一个路径点
            for (int i = n - 2; i >= 0; i--)
            {
                var point = resultPoints[i];
                var logicPos = new Vector2Int(point.x, point.y);
                if (i != 0)
                {
                    pathPos.Add(AstarUtility.GetRendererPos(logicPos) + new Vector3(0.5f, 0, 0.5f));
                }
                else
                {
                    pathPos.Add(AstarUtility.GetRendererPos(logicPos));    
                }
                
            }

            _paths[pathName] = pathPos;
        }

        public virtual void OnMapBuildingTriggerEnter(string tirggerObjName, MapBuildingObject buildingObj,
            UnityEngine.Collider other)
        {
            if (_mapLevel != null)
            {
                _mapLevel.OnMapBuildingTriggerEnter(tirggerObjName, buildingObj, other);
            }
        }

        public virtual void OnMapBuildingTriggerExit(string tirggerObjName, MapBuildingObject buildingObj,
            UnityEngine.Collider other)
        {
            if (_mapLevel != null)
            {
                _mapLevel.OnMapBuildingTriggerExit(tirggerObjName, buildingObj, other);
            }
        }

        public virtual void OnMapLevelFinish()
        {
            foreach (var mapActor in _mapActors)
            {
                mapActor.DispatchMapLevelFinish();
            }

            _mapActors.Clear();
        }

        public virtual void OnMapNPCTriggerEnter(string tirggerObjName, MapNPCObject npcObj, UnityEngine.Collider other)
        {
            if (_mapLevel != null)
            {
                _mapLevel.OnMapNPCTriggerEnter(tirggerObjName, npcObj, other);
            }
        }

        public virtual void OnMapNPCTriggerExit(string tirggerObjName, MapNPCObject npcObj, UnityEngine.Collider other)
        {
            if (_mapLevel != null)
            {
                _mapLevel.OnMapNPCTriggerExit(tirggerObjName, npcObj, other);
            }
        }

        public virtual void OnAddMapActor(MapActorObject mapActor)
        {
            _mapActors.Add(mapActor);
        }

        public virtual void OnRemoveMapActor(MapActorObject mapActor)
        {
            _mapActors.Remove(mapActor);
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

            if (_mapLevel != null)
            {
                _mapLevel.OnUpdate(timeElasped);
            }
        }

        override public void OnPostUpdate(float timeElasped)
        {
            base.OnPostUpdate(timeElasped);

            if (_mapLevel != null)
            {
                _mapLevel.OnPostUpdate(timeElasped);
            }
        }

        public IMapLevel CreateLevel(string levelName)
        {
            if (_mapLevel != null)
            {
                _mapLevel.Clear();
                _mapLevel = null;
            }

            var levelConf = UnityGameApp.Inst.MapManager.MapConf.getMapLevelConf(levelName);
            if (levelConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error,
                    $"Map [{_name}] create level [{levelName}] config not exist");
                return null;
            }

            _mapLevel = UnityGameApp.Inst.MapManager.createMapLevel(levelConf.levelType) as MapLevel;
            if (_mapLevel == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error,
                    $"Map [{_name}] create level [{levelName}] type [{levelConf.levelType}] not exist");
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

        public bool isInActiveRect(Vector3 pos)
        {
            return _activeRect.Contains(new Vector2(pos.x, pos.z));
        }
    }


    public class SpawnPos
    {
        public UnityEngine.GameObject spawnObject;
        public UnityEngine.Vector3 spawnCenter;
        public UnityEngine.Vector3 spawnHalfSize;


        public UnityEngine.Vector3 randSpawnPos()
        {
            int x = (int) (spawnHalfSize.x * 1000);
            float xf = (float) UnityGameApp.Inst.Rand.RandomBetween(-x, x) / 1000.0f;
            int y = (int) (spawnHalfSize.y * 1000);
            float yf = (float) UnityGameApp.Inst.Rand.RandomBetween(-y, y) / 1000.0f;
            int z = (int) (spawnHalfSize.z * 1000);
            float zf = (float) UnityGameApp.Inst.Rand.RandomBetween(-z, z) / 1000.0f;

            return spawnCenter + new UnityEngine.Vector3(xf, yf, zf);
        }

        public UnityEngine.Vector3 getNearCorner(UnityEngine.Vector3 nearTo, bool ignoreY = true)
        {
            var list = new List<UnityEngine.Vector3>();
            if (ignoreY)
            {
                list.Add(new UnityEngine.Vector3(spawnCenter.x - spawnHalfSize.x, spawnCenter.y,
                    spawnCenter.z - spawnHalfSize.z));
                list.Add(new UnityEngine.Vector3(spawnCenter.x - spawnHalfSize.x, spawnCenter.y,
                    spawnCenter.z + spawnHalfSize.z));
                list.Add(new UnityEngine.Vector3(spawnCenter.x + spawnHalfSize.x, spawnCenter.y,
                    spawnCenter.z + spawnHalfSize.z));
                list.Add(new UnityEngine.Vector3(spawnCenter.x + spawnHalfSize.x, spawnCenter.y,
                    spawnCenter.z - spawnHalfSize.z));
            }
            else
            {
                list.Add(new UnityEngine.Vector3(spawnCenter.x - spawnHalfSize.x, spawnCenter.y - spawnHalfSize.y,
                    spawnCenter.z - spawnHalfSize.z));
                list.Add(new UnityEngine.Vector3(spawnCenter.x - spawnHalfSize.x, spawnCenter.y - spawnHalfSize.y,
                    spawnCenter.z + spawnHalfSize.z));
                list.Add(new UnityEngine.Vector3(spawnCenter.x + spawnHalfSize.x, spawnCenter.y - spawnHalfSize.y,
                    spawnCenter.z + spawnHalfSize.z));
                list.Add(new UnityEngine.Vector3(spawnCenter.x + spawnHalfSize.x, spawnCenter.y - spawnHalfSize.y,
                    spawnCenter.z - spawnHalfSize.z));
                list.Add(new UnityEngine.Vector3(spawnCenter.x - spawnHalfSize.x, spawnCenter.y + spawnHalfSize.y,
                    spawnCenter.z - spawnHalfSize.z));
                list.Add(new UnityEngine.Vector3(spawnCenter.x - spawnHalfSize.x, spawnCenter.y + spawnHalfSize.y,
                    spawnCenter.z + spawnHalfSize.z));
                list.Add(new UnityEngine.Vector3(spawnCenter.x + spawnHalfSize.x, spawnCenter.y + spawnHalfSize.y,
                    spawnCenter.z + spawnHalfSize.z));
                list.Add(new UnityEngine.Vector3(spawnCenter.x + spawnHalfSize.x, spawnCenter.y + spawnHalfSize.y,
                    spawnCenter.z - spawnHalfSize.z));
            }

            float minDist = float.MaxValue;
            UnityEngine.Vector3 ret = list[0];
            foreach (var vec in list)
            {
                float dist = (vec - nearTo).magnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    ret = vec;
                }
            }

            return ret;
        }

        public bool isPositionIn(UnityEngine.Vector3 pos, bool ignoreY = true)
        {
            if (ignoreY)
            {
                return (pos.x >= spawnCenter.x - spawnHalfSize.x) && (pos.x <= spawnCenter.x + spawnHalfSize.x)
                                                                  && (pos.z >= spawnCenter.z - spawnHalfSize.z) &&
                                                                  (pos.z <= spawnCenter.z + spawnHalfSize.z);
            }
            else
            {
                return (pos.x >= spawnCenter.x - spawnHalfSize.x) && (pos.x <= spawnCenter.x + spawnHalfSize.x)
                                                                  && (pos.y >= spawnCenter.y - spawnHalfSize.y) &&
                                                                  (pos.y <= spawnCenter.y + spawnHalfSize.y)
                                                                  && (pos.z >= spawnCenter.z - spawnHalfSize.z) &&
                                                                  (pos.z <= spawnCenter.z + spawnHalfSize.z);
            }
        }
    }
}