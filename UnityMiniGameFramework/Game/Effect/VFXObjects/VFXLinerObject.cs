using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class VFXLinerObject : VFXObjectBase
    {
        override public string type => "VFXLinerObject";

        new public static VFXLinerObject create()
        {
            return new VFXLinerObject();
        }

        protected UnityEngine.LineRenderer _linerRender;
        public UnityEngine.LineRenderer linerRender => _linerRender;

        protected float _uvSpeed;
        protected float _initUV;

        override public void Init(VFXConf conf, UnityEngine.GameObject o)
        {
            base.Init(conf, o);

            _linerRender = o.GetComponent<UnityEngine.LineRenderer>();

            if(_linerRender == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_System, $"VFXObject [{name}] init with [{conf.prefabName}] without LineRenderer");
                return;
            }

            _initUV = UnityGameApp.Inst.Rand.RandomBetween(0, 500) / 100.0f;
            _uvSpeed = conf.uvSpeed.HasValue ? conf.uvSpeed.Value : 0;

            _linerRender.positionCount = 2;
            _linerRender.SetPosition(0, UnityEngine.Vector3.zero);
            _linerRender.SetPosition(1, new UnityEngine.Vector3(0, 0, 1));
        }

        override public void OnCacheRecreate()
        {
            _initUV = UnityGameApp.Inst.Rand.RandomBetween(0, 500) / 100.0f;
            _linerRender.SetPosition(0, UnityEngine.Vector3.zero);
            _linerRender.SetPosition(1, new UnityEngine.Vector3(0, 0, 1));
        }

        override public void OnUpdate(float deltaTime)
        {
            //base.OnUpdate(deltaTime);

            if(_linerRender == null)
            {
                return;
            }

            if(_uvSpeed != 0)
            {
                _linerRender.material.SetTextureOffset("_MainTex", new UnityEngine.Vector2(UnityEngine.Time.time * _uvSpeed + _initUV, 0f));
            }
        }
    }
}
