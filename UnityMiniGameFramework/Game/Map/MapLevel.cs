using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    class UnityMapLevelColliderTrigger : UnityEngine.MonoBehaviour
    {
        private void OnTriggerEnter(UnityEngine.Collider other)
        {
            if (UnityGameApp.Inst.currInitStep != MiniGameFramework.GameAppInitStep.EnterMainScene)
            {
                return;
            }

            if(UnityGameApp.Inst.MainScene.map.currentLevel == null)
            {
                return;
            }

            (UnityGameApp.Inst.MainScene.map.currentLevel as MapLevel).OnMapLevelTriggerEnter(this.gameObject.name, other);
        }
        private void OnTriggerExit(UnityEngine.Collider other)
        {
            if (UnityGameApp.Inst.currInitStep != MiniGameFramework.GameAppInitStep.EnterMainScene)
            {
                return;
            }

            if (UnityGameApp.Inst.MainScene.map.currentLevel == null)
            {
                return;
            }

            (UnityGameApp.Inst.MainScene.map.currentLevel as MapLevel).OnMapLevelTriggerEnter(this.gameObject.name, other);
        }
    }

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

        protected Dictionary<string, int> _kmCount;
        public Dictionary<string, int> kmCount => _kmCount;

        public MapLevel()
        {
            _kmCount = new Dictionary<string, int>();
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
                });
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
                sp.mapMonSpawn.Reset();
            }

            _isStarted = true;
            _kmCount.Clear();
        }

        virtual public void Finish()
        {
            _map.OnMapLevelFinish();

            foreach (var sp in _monSpawns)
            {
                sp.mapMonSpawn.StopSpawn();
                sp.mapMonSpawn.ClearAllMonsters();
            }
            _isStarted = false;
        }

        public void Clear()
        {
            foreach (var sp in _monSpawns)
            {
                sp.mapMonSpawn.StopSpawn();
                sp.mapMonSpawn.ClearAllMonsters();
            }
            _kmCount.Clear();
        }

        public virtual void OnMapBuildingTriggerEnter(string tirggerObjName, MapBuildingObject buildingObj, UnityEngine.Collider other)
        {
        }
        public virtual void OnMapBuildingTriggerExit(string tirggerObjName, MapBuildingObject buildingObj, UnityEngine.Collider other)
        {
        }

        public virtual void OnMapLevelTriggerEnter(string triggerObjName, UnityEngine.Collider other)
        {

        }
        public virtual void OnMapLevelTriggerExit(string triggerObjName, UnityEngine.Collider other)
        {

        }

        public virtual void OnMapNPCTriggerEnter(string tirggerObjName, MapNPCObject npcObj, UnityEngine.Collider other)
        {
        }
        public virtual void OnMapNPCTriggerExit(string tirggerObjName, MapNPCObject npcObj, UnityEngine.Collider other)
        {
        }

        protected virtual void _OnTimeUp()
        {

        }

        protected virtual void _OnWin()
        {
            // 显示通关奖励界面
            UIPassRewardPanel _passUI = UnityGameApp.Inst.UI.createUIPanel("PassRewardUI") as UIPassRewardPanel;
            _passUI.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.uiRootObject).unityGameObject.transform);
            _passUI.showUI();
        }

        protected virtual void _OnLose()
        {

        }

        protected virtual bool _checkFinish()
        {
            if(_timeLeft <= 0)
            {
                _OnTimeUp();
                return true;
            }

            if(_conf.kmWinCheck != null && _conf.kmWinCheck.Count > 0)
            {
                bool win = true;
                foreach(var km in _conf.kmWinCheck)
                {
                    if(_kmCount.ContainsKey(km.mapMonsterName))
                    {
                        if(_kmCount[km.mapMonsterName] < km.killCount)
                        {
                            win = false;
                            break;
                        }
                    }
                    else
                    {
                        win = false;
                        break;
                    }
                }

                if(win)
                {
                    _OnWin();
                    return true;
                }
            }

            int i;
            for (i = 0; i < _monSpawns.Count; ++i)
            {
                var sp = _monSpawns[i];
                if(!sp.mapMonSpawn.isFinishSpawn || sp.mapMonSpawn.monsters.Count > 0)
                {
                    // still spawn or spawn monsters alive
                    break;
                }
            }

            if(i == _monSpawns.Count)
            {
                // all monster spawned and cleared
                _OnLose();
                return true;
            }

            // TO DO : check win or other finish condition

            return false;
        }

        virtual public void OnMonsterDie(MapMonsterObject mon)
        {
            if(_kmCount.ContainsKey(mon.name))
            {
                _kmCount[mon.name] += 1;
            }
            else
            {
                _kmCount[mon.name] = 1;
            }
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
