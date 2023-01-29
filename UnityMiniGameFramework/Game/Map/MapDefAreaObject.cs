using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace UnityMiniGameFramework
{
    public class MapDefAreaObject : MGGameObject
    {
        new public static MapDefAreaObject create()
        {
            return new MapDefAreaObject();
        }

        protected GameObject _rectObj;

        public override void PostInit()
        {
            var box = _unityGameObject.GetComponent<BoxCollider>();
            if (_rectObj == null && box != null)
            {
                var pb = ((UnityResourceManager)UnityGameApp.Inst.Resource).LoadUnityPrefabObject("VFX/DefAreaRect");
                if (pb)
                {
                    _rectObj = GameObject.Instantiate(pb);
                    _rectObj.transform.parent = _unityGameObject.transform;
                    _rectObj.transform.localPosition = Vector3.zero;
                }
                SetAreaRange(box.size.x, box.size.z);
            }

            HideArea();
        }

        private void SetAreaRange(float x, float y)
        {
            if (_rectObj == null)
            {
                return;
            }
            _rectObj.transform.localScale = new Vector3(x, y, 1);
        }

        public void HideArea()
        {
            if (_rectObj == null)
            {
                return;
            }
            _rectObj.SetActive(false);
        }

        /// <summary>
        /// 显示npc站位区域，能摆放显示绿色，否则显示蓝色
        /// </summary>
        public void ShowAreaStyle(bool isCanPut)
        {
            if (_rectObj == null)
            {
                return;
            }
            _rectObj.SetActive(true);
            var spriteRenderer = _rectObj.GetComponent<SpriteRenderer>();
            string spPath;
            if (isCanPut)
            {
                spPath = "Battle/DefArea/area_lv";
            }
            else
            {
                spPath = "Battle/DefArea/area_lan";
            }
            var sp = ((UnityResourceManager)UnityGameApp.Inst.Resource).LoadSprite(spPath);
            if (sp)
            {
                spriteRenderer.sprite = sp;
            }
        }
    }
}
