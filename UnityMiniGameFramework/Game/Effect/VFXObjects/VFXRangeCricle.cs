using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class VFXRangeCricle : VFXObjectBase
    {
        override public string type => "VFXRangeCricle";

        new public static VFXRangeCricle create()
        {
            return new VFXRangeCricle();
        }

        override public void Init(VFXConf conf, UnityEngine.GameObject o)
        {
            base.Init(conf, o);

        }

        public void SetCircleRange(float range)
        {
            _unityGameObject.transform.localScale = new UnityEngine.Vector3(range, range, range);
        }

        public void SetColor(float r, float g, float b, float a)
        {
            var mr = _unityGameObject.GetComponent<UnityEngine.MeshRenderer>();
            mr.material.color = new UnityEngine.Color(r, g, b, a);
        }
    }
}
