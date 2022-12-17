using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class VFXManager
    {
        protected VFXConfig _vfxConf;
        public VFXConfig VFXConfs => _vfxConf;


        protected Dictionary<string, UnityEngine.GameObject> _vfxCacheUnityPrefabObjects;

        protected Dictionary<string, Func<VFXObjectBase>> _vfxObjectCreators;

        protected Dictionary<string, Queue<VFXObjectBase>> _cachedVfxs; // vfx name => cached vfx list
        protected Dictionary<string, HashSet<VFXObjectBase>> _currentShowVfxs; // vfx name => current show vfx set

        protected Dictionary<MGGameObject, List<VFXObjectBase>> _attachedVfxs;
        protected Dictionary<VFXObjectBase, MGGameObject> _vfxAttachToObj;

        public VFXManager()
        {
            _vfxCacheUnityPrefabObjects = new Dictionary<string, UnityEngine.GameObject>();
            _vfxObjectCreators = new Dictionary<string, Func<VFXObjectBase>>();

            _cachedVfxs = new Dictionary<string, Queue<VFXObjectBase>>();
            _currentShowVfxs = new Dictionary<string, HashSet<VFXObjectBase>>();

            _attachedVfxs = new Dictionary<MGGameObject, List<VFXObjectBase>>();
            _vfxAttachToObj = new Dictionary<VFXObjectBase, MGGameObject>();
        }

        public void registerVfxObjectCreator(string type, Func<VFXObjectBase> creator)
        {
            if (_vfxObjectCreators.ContainsKey(type))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"registerVfxObjectCreator ({type}) already exist");
                return;
            }
            _vfxObjectCreators[type] = creator;
        }

        protected VFXObjectBase _createVfxObject(string type)
        {
            if (_vfxObjectCreators.ContainsKey(type))
            {
                return _vfxObjectCreators[type]();
            }

            Debug.DebugOutput(DebugTraceType.DTT_Error, $"createVfxObject ({type}) not exist");

            return null;
        }

        public void Init()
        {
            _vfxConf = (VFXConfig)UnityGameApp.Inst.Conf.getConfig("vfxs");
            if (_vfxConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Create vfx manager [vfxs] config not exist");
                return;
            }

        }
        public VFXObjectBase createVFXObject(string vfxName)
        {
            var conf = _vfxConf.getVFXConfig(vfxName);
            if (conf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Create VFXObject [{vfxName}] config not exist");
                return null;
            }

            if(conf.name != vfxName)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Create VFXObject [{vfxName}] config name [{conf.name}] not same");
                return null;
            }

            // check max show count
            HashSet<VFXObjectBase> currShowSet;
            if(_currentShowVfxs.TryGetValue(vfxName, out currShowSet))
            {
                uint maxShowCount = conf.maxShowCount.HasValue? conf.maxShowCount.Value : 10;
                if (maxShowCount <= currShowSet.Count)
                {
                    Debug.DebugOutput(DebugTraceType.DTT_Detail, $"Create VFXObject [{vfxName}] show count[{currShowSet.Count}] >= max show count [{maxShowCount}]");
                    return null;
                }
            }

            if(!_currentShowVfxs.ContainsKey(vfxName))
            {
                currShowSet = new HashSet<VFXObjectBase>();
                _currentShowVfxs[vfxName] = currShowSet;
            }
            else
            {
                currShowSet = _currentShowVfxs[vfxName];
            }

            // try fetch from cache
            var vfxObject = _tryPopCache(vfxName);
            if (vfxObject != null)
            {
                currShowSet.Add(vfxObject);
                return vfxObject;
            }

            // create new 
            vfxObject = _createVfxObject(conf.type);
            if(vfxObject == null)
            {
                return null;
            }

            var unityObject = _createVFXUnityObject(vfxName, conf);
            if(unityObject == null)
            {
                return null;
            }

            vfxObject.Init(conf, unityObject);

            currShowSet.Add(vfxObject);

            return vfxObject;
        }

        protected VFXObjectBase _tryPopCache(string vfxName)
        {
            if (!_cachedVfxs.ContainsKey(vfxName))
            {
                return null;
            }

            var vfxObj = _cachedVfxs[vfxName].Dequeue();
            vfxObj.OnCacheRecreate();

            if (_cachedVfxs[vfxName].Count <= 0)
            {
                _cachedVfxs.Remove(vfxName);
            }

            return vfxObj;
        }

        protected UnityEngine.GameObject _createVFXUnityObject(string vfxName, VFXConf conf)
        {
            if(_vfxCacheUnityPrefabObjects.ContainsKey(vfxName))
            {
                return UnityEngine.GameObject.Instantiate(_vfxCacheUnityPrefabObjects[vfxName]);
            }

            var vfxObject = UnityGameApp.Inst.UnityResource.LoadUnityPrefabObject(conf.prefabName);
            _vfxCacheUnityPrefabObjects[vfxName] = vfxObject;

            return UnityEngine.GameObject.Instantiate(vfxObject);
        }

        public void releaseVFXCacheUnityPrefabObject(string vfxName)
        {
            if (!_vfxCacheUnityPrefabObjects.ContainsKey(vfxName))
            {
                return;
            }

            UnityGameApp.Inst.UnityResource.ReleaseUnityPrefabObject(_vfxCacheUnityPrefabObjects[vfxName]);
            _vfxCacheUnityPrefabObjects.Remove(vfxName);
        }

        public void onVFXAttachToGameObj(VFXObjectBase vfx, MGGameObject obj)
        {
            List<VFXObjectBase> list = null;
            if(!_attachedVfxs.ContainsKey(obj))
            {
                list = new List<VFXObjectBase>();
                _attachedVfxs[obj] = list;
            }
            else
            {
                list = _attachedVfxs[obj];
            }

            list.Add(vfx);

            _vfxAttachToObj[vfx] = obj;
        }

        public void onGameObjDestroy(MGGameObject obj)
        {
            if (!_attachedVfxs.ContainsKey(obj))
            {
                return;
            }

            var list = _attachedVfxs[obj];
            foreach(var vfx in list)
            {
                if(vfx.unityGameObject != null)
                {
                    vfx.unityGameObject.transform.SetParent(null); // detach from game object
                    vfx.particleSystem.Stop();
                }
                _vfxAttachToObj.Remove(vfx);
                _onVFXDestory(vfx);
            }

            _attachedVfxs.Remove(obj);
        }

        protected void _onVFXDestory(VFXObjectBase o)
        {
            if (_currentShowVfxs.ContainsKey(o.name))
            {
                var currShowSet = _currentShowVfxs[o.name];
                if (currShowSet.Contains(o))
                {
                    currShowSet.Remove(o);
                }
            }

            //UnityEngine.GameObject.Destroy(o.unityGameObject);

            if (o.unityGameObject == null)
            {
                // already destroy, don't cache
                return;
            }

            if (o.maxCacheCount > 0)
            {
                Queue<VFXObjectBase> cacheQueue;
                if (!_cachedVfxs.TryGetValue(o.name, out cacheQueue))
                {
                    cacheQueue = new Queue<VFXObjectBase>();
                    _cachedVfxs[o.name] = cacheQueue;
                }

                if (o.maxCacheCount > cacheQueue.Count)
                {
                    UnityGameApp.Inst.MainScene.cacheUnityObject(o.unityGameObject);
                    cacheQueue.Enqueue(o);
                }
                else
                {
                    // TO DO : clear vfx object
                    UnityEngine.GameObject.Destroy(o.unityGameObject);
                }
            }
            else
            {
                // TO DO : clear vfx object
                UnityEngine.GameObject.Destroy(o.unityGameObject);
            }
        }

        public void onVFXDestory(VFXObjectBase o)
        {
            if(_vfxAttachToObj.ContainsKey(o))
            {
                // clear attached vfx
                List<VFXObjectBase> list = null;
                _attachedVfxs.TryGetValue(_vfxAttachToObj[o], out list);
                if(list != null)
                {
                    list.Remove(o);
                }

                _vfxAttachToObj.Remove(o);
            }

            _onVFXDestory(o);
        }

        public void OnUpdate(float deltaTime)
        {
            Queue<VFXObjectBase> currVFXs = new Queue<VFXObjectBase>();
            foreach(var pair in _currentShowVfxs)
            {
                foreach(var vfx in pair.Value)
                {
                    currVFXs.Enqueue(vfx);
                }
            }

            while(currVFXs.Count > 0)
            {
                var vfx = currVFXs.Dequeue();

                vfx.OnUpdate(deltaTime);
            }
        }
    }
}
