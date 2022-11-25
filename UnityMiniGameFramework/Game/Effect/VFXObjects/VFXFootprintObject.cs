using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class VFXFootprintObject : VFXObjectBase
    {
        override public string type => "VFXFootprintObject";

        new public static VFXFootprintObject create()
        {
            return new VFXFootprintObject();
        }

        public float lifeTime => _conf.lifeTime.HasValue ? _conf.lifeTime.Value : 1;

        protected float _timeLeft;

        override public void Init(VFXConf conf, UnityEngine.GameObject o)
        {
            base.Init(conf, o);

            _timeLeft = lifeTime;
        }

        override public void OnCacheRecreate()
        {
            _timeLeft = lifeTime;
            _unityGameObject.SetActive(true);
        }

        override public void OnUpdate(float deltaTime)
        {
            //base.OnUpdate(deltaTime);

            _timeLeft -= deltaTime;
            if(_timeLeft <= 0)
            {
                _timeLeft = 0;

                _unityGameObject.SetActive(false);
                UnityGameApp.Inst.VFXManager.onVFXDestory(this);
            }
        }
    }
}
