using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

        public void ShowAttackRange(bool isCanPut)
        {
            var spriteRenderer = _unityGameObject.GetComponent<SpriteRenderer>();
            string spPath; 
            if (isCanPut)
            {
                spPath = "Battle/HeroAttackRange/Textures/aq_lv";
            }
            else
            {
                spPath = "Battle/HeroAttackRange/Textures/aq_lan";
            }
            var sp = ((UnityResourceManager) UnityGameApp.Inst.Resource).LoadSprite(spPath);
            if (sp)
                spriteRenderer.sprite = sp;
        }
    }
}
