using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class VFXObjectBase : IVFXObject
    {
        virtual public string type => "VFXObjectBase";

        public static VFXObjectBase create()
        {
            return new VFXObjectBase();
        }

        protected VFXConf _conf;
        public string name => _conf.name;

        public uint maxCacheCount => _conf.maxCacheCount.HasValue ? _conf.maxCacheCount.Value : 0;
        public uint maxShowCount => _conf.maxShowCount.HasValue ? _conf.maxShowCount.Value : 10;

        protected UnityEngine.GameObject _unityGameObject;
        public UnityEngine.GameObject unityGameObject => _unityGameObject;

        protected UnityEngine.ParticleSystem _particleSys;

        public UnityEngine.ParticleSystem particleSystem => _particleSys;

        public VFXObjectBase()
        {
        }

        virtual public void Init(VFXConf conf, UnityEngine.GameObject o)
        {
            _conf = conf;
            _unityGameObject = o;
            _particleSys = o.GetComponent<UnityEngine.ParticleSystem>();
            //if(_particleSys == null)
            //{
            //    Debug.DebugOutput(DebugTraceType.DTT_System, $"VFXObject [{name}] init with [{conf.prefabName}] without ParticleSystem");
            //    return;
            //}
        }

        virtual public void OnCacheRecreate()
        {
            // TO DO : re use object, do init
            if (_particleSys != null)
            {
                _particleSys.Play();
            }
            _unityGameObject.SetActive(true);
        }

        virtual public void OnUpdate(float deltaTime)
        {
            if(_particleSys != null)
            {
                if(!_particleSys.main.loop && _particleSys.isStopped)
                {
                    // finish play

                    _unityGameObject.SetActive(false);

                    UnityGameApp.Inst.VFXManager.onVFXDestory(this);
                }
            }
        }

        public void Play()
        {
            if (_particleSys != null)
            {
                _particleSys.Play();
            }
        }
    }
}
