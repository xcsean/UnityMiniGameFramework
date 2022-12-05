using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityMiniGameFramework
{
    public class Scene : IScene
    {
        protected string _name;
        public string name => _name;

        protected MGGameObject _rootObj;
        public IGameObject sceneRootObj => _rootObj;

        protected MGGameObject _uiRootObj;
        public IGameObject uiRootObject => _uiRootObj;

        // unity cached objects root
        protected UnityEngine.GameObject _cachedRootObj;
        public UnityEngine.GameObject cachedRootObj => _cachedRootObj;

        protected UnityGameCamera _mainCamera;
        public ICamera camera => _mainCamera;
        public Camera unityCamera => _mainCamera.unityCamera;

        protected UnityEngine.GameObject _mapRoot;
        public UnityEngine.GameObject mapRoot => _mapRoot;

        protected AsyncOpStatus _loadStatus;
        public AsyncOpStatus loadStatus => _loadStatus;

        protected AsyncOpStatus _unloadStatus;
        public AsyncOpStatus unloadStatus => _unloadStatus;

        protected Map _map;
        public IMap map => _map;
        public Map implMap => _map;

        protected Dictionary<string, Map> _maps;

        protected SceneConf _conf;
        protected UnityEngine.SceneManagement.Scene _unityScene;
        protected AsyncOperation _unitySceneLoadStatus;
        protected AsyncOperation _unitySceneUnLoadStatus;

        public bool isAdditive => (_conf.isAdditive.HasValue && _conf.isAdditive.Value);

        public Scene(SceneConf conf)
        {
            _conf = conf;
            _name = conf.name;
        }


        public void UnloadAsync()
        {
            if(!_loadStatus.done)
            {
                return;
            }

            if(isAdditive)
            {
                _unitySceneUnLoadStatus = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(_unityScene);
                _unloadStatus.progressing = true;
            }
            else
            {

                _unloadStatus.progressing = false;
                _unloadStatus.done = true;

                _onUnloaded();
            }
        }

        public void LoadAsync()
        {
            if(_unloadStatus.progressing)
            {
                // err : unload is in progress
            }

            _unityScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(_conf.name);
            if(!_unityScene.isLoaded)
            {
                _loadStatus.progressing = true;
                if(isAdditive)
                {
                    _unitySceneLoadStatus = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_conf.name, LoadSceneMode.Additive);
                }
                else
                {
                    _unitySceneLoadStatus = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_conf.name);
                }
                //_unitySceneLoadStatus.allowSceneActivation = true;
            }
        }

        protected void _onLoaded()
        {
            // TO DO : init scene

            string rootName = "Root";
            if(_conf.rootObjectName != null)
            {
                rootName = _conf.rootObjectName;
            }

            string uiRootName = "UIRoot";
            if (_conf.uiRootName != null)
            {
                uiRootName = _conf.uiRootName;
            }

            string mainCameraName = "MainCamera";
            if (_conf.mainCameraName != null)
            {
                mainCameraName = _conf.mainCameraName;
            }

            string mapRootName = "MapRoot";
            if (_conf.mapRootName != null)
            {
                mapRootName = _conf.mapRootName;
            }

            string cachedRootName = "CachedRoot";


            // fetch objects
            UnityEngine.GameObject[] objs = _unityScene.GetRootGameObjects();

            for(uint i=0; i< objs.Length; ++i)
            {
                if(objs[i].name == rootName)
                {
                    var mgObjComp = objs[i].GetComponent<UnityGameObjectBehaviour>();
                    if (mgObjComp == null)
                    {
                        MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Load Scene {_name} rootobj {rootName} without UnityGameObjectBehaviour");
                    }
                    else
                    {
                        _rootObj = mgObjComp.mgGameObject;
                    }
                }
                else if(objs[i].name == uiRootName)
                {
                    var mgObjComp = objs[i].GetComponent<UnityGameObjectBehaviour>();
                    if (mgObjComp == null)
                    {
                        MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Load Scene {_name} ui rootobj {uiRootName} without UnityGameObjectBehaviour");
                    }
                    else
                    {
                        _uiRootObj = mgObjComp.mgGameObject;
                    }
                }
                else if(objs[i].name == mainCameraName)
                {
                    // TO DO : camera

                    var mgObjComp = objs[i].GetComponent<UnityGameObjectBehaviour>();
                    if (mgObjComp == null)
                    {
                        MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Load Scene {_name} ui rootobj {uiRootName} without UnityGameObjectBehaviour");
                    }
                    else
                    {
                        _mainCamera = mgObjComp.mgGameObject as UnityGameCamera;
                    }
                }
                else if(objs[i].name == mapRootName)
                {
                    // TO DO : map
                    _mapRoot = objs[i];
                    _maps = new Dictionary<string, Map>();

                    var tr = _mapRoot.transform.Find(_conf.mapName);
                    if (tr == null)
                    {
                        MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Load Scene {_name} map object {_conf.mapName} not exist");
                    }
                    else
                    {
                        var mgObjComp = tr.gameObject.GetComponent<UnityGameObjectBehaviour>();
                        if (mgObjComp == null)
                        {
                            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Load Scene {_name} map {_conf.mapName} without UnityGameObjectBehaviour");
                        }
                        else
                        {
                            _map = mgObjComp.mgGameObject as Map;
                            _maps[_conf.mapName] = _map;
                        }
                    }

                    if (_conf.mapNames != null)
                    {
                        foreach(var mapName in _conf.mapNames)
                        {
                            tr = _mapRoot.transform.Find(mapName);
                            if (tr == null)
                            {
                                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Load Scene {_name} maplist object {mapName} not exist");
                                continue;
                            }
                            else
                            {
                                var mgObjComp = tr.gameObject.GetComponent<UnityGameObjectBehaviour>();
                                if (mgObjComp == null)
                                {
                                    MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Load Scene {_name} maplist {mapName} without UnityGameObjectBehaviour");
                                    continue;
                                }
                                else
                                {
                                    _maps[mapName] = mgObjComp.mgGameObject as Map;
                                }
                            }
                        }
                    }
                }
                else if(objs[i].name == cachedRootName)
                {
                    _cachedRootObj = objs[i];
                }
            }

            if(_conf.changeOnLoaded.HasValue && _conf.changeOnLoaded.Value)
            {
                UnityGameApp.Inst.SceneManager.changeScene(this);
            }

            // TO DO : manager ui in scene
            // load scene main ui panel
            UIPanel mainUIPanel = (UIPanel)UnityGameApp.Inst.UI.createUIPanel(_conf.mainUIPanelName);
            mainUIPanel.unityGameObject.transform.SetParent(_uiRootObj.unityGameObject.transform);
        }

        protected void _onUnloaded()
        {
            // TO DO : on unloaded

            UnityGameApp.Inst.SceneManager.OnDisposeScene(this);
        }

        public void OnHide()
        {
            if(!isAdditive)
            {
                return;
            }

            UnityEngine.SceneManagement.SceneManager.SetActiveScene(((UnitySceneManager)UnityGameApp.Inst.SceneManager).unityBaseScene);

            _rootObj.Hide();
            _uiRootObj.Hide();
            _mainCamera.Hide();
        }

        public void OnShow()
        {
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(_unityScene);

            _rootObj.Show();
            _uiRootObj.Show();
            _mainCamera.Show();
        }

        public void OnUpdate()
        {
            if(_loadStatus.progressing)
            {
                _loadStatus.percentage = (uint)(_unitySceneLoadStatus.progress * 100);

                if (_unitySceneLoadStatus.isDone)
                {
                    _loadStatus.progressing = false;
                    _loadStatus.done = true;

                    _unityScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(_conf.name);

                    _onLoaded();
                }
            }

            if(_unloadStatus.progressing)
            {
                _unloadStatus.percentage = (uint)(_unitySceneUnLoadStatus.progress * 100);

                if(_unitySceneUnLoadStatus.isDone)
                {
                    _unloadStatus.progressing = false;
                    _unloadStatus.done = true;

                    _onUnloaded();
                }
            }
        }

        public void cacheUnityObject(UnityEngine.GameObject o)
        {
            if(_cachedRootObj != null)
            {
                o.transform.SetParent(_cachedRootObj.transform);
            }
            else
            {
                o.transform.SetParent(null);
            }
        }

        public void changeMap(string newMapName)
        {
            // TO DO : implement change map

            Map map;
            if(!_maps.TryGetValue(newMapName, out map))
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Scene [{_name}] change map [{newMapName}] not exist");

                return;
            }

            _map.OnLeave();

            _map = map;

            _map.OnEnter();
        }
    }
}
