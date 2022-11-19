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

        protected ICamera _mainCamera;
        public ICamera camera => _mainCamera;

        protected AsyncOpStatus _loadStatus;
        public AsyncOpStatus loadStatus => _loadStatus;

        protected AsyncOpStatus _unloadStatus;
        public AsyncOpStatus unloadStatus => _unloadStatus;

        protected SceneConf _conf;
        protected UnityEngine.SceneManagement.Scene _unityScene;
        protected AsyncOperation _unitySceneLoadStatus;
        protected AsyncOperation _unitySceneUnLoadStatus;

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

            _unitySceneUnLoadStatus = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(_unityScene);
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
                _unitySceneLoadStatus = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_conf.name, LoadSceneMode.Additive);
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
                }
            }

            // load scene main ui panel
            UIPanel mainUIPanel = (UIPanel)UnityGameApp.Inst.UI.createUIPanel(_conf.mainUIPanelName);
            mainUIPanel.unityGameObject.transform.SetParent(_uiRootObj.unityGameObject.transform);
        }

        protected void _onUnloaded()
        {
            // TO DO : on unloaded

            UnityGameApp.Inst.SceneManager.onDisposeScene(this);
        }

        public void OnHide()
        {
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(((UnitySceneManager)UnityGameApp.Inst.SceneManager).unityBaseScene);

            _rootObj.Hide();
            _uiRootObj.Hide();
        }

        public void OnShow()
        {
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(_unityScene);

            _rootObj.Show();
            _uiRootObj.Show();
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
    }
}
