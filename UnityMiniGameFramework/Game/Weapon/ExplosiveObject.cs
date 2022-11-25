using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class UnityExplosiveCollider : UnityEngine.MonoBehaviour
    {
        public ExplosiveObject explosiveObj;

        private void Start()
        {
        }

        private void OnTriggerEnter(UnityEngine.Collider other)
        {
            explosiveObj.onHitEnter(other);
        }

        private void OnTriggerExit(UnityEngine.Collider other)
        {
            explosiveObj.onHitExit(other);
        }
    }

    public class ExplosiveObject
    {
        protected VFXObjectBase _explosiveVFX;
        public VFXObjectBase explosiveVFX => _explosiveVFX;

        protected HashSet<UnityEngine.GameObject> _hitedObjects;

        protected float _startTime;
        protected float _endTime;
        // TO DO : 

        public ExplosiveObject()
        {
            _hitedObjects = new HashSet<UnityEngine.GameObject>();
        }

        public bool Init(ExplosiveConf conf)
        {
            _explosiveVFX = UnityGameApp.Inst.VFXManager.createVFXObject(conf.explosiveVFX);
            if(_explosiveVFX == null)
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Init explosive vfx({conf.explosiveVFX}) not exist.");
                return false;
            }

            _explosiveVFX.onDestoryCallback = onVFXDestory;

            var col = _explosiveVFX.unityGameObject.AddComponent<UnityExplosiveCollider>();
            col.explosiveObj = this;

            _startTime = UnityEngine.Time.time + (conf.startTime.HasValue ? conf.startTime.Value : 0);
            _endTime = UnityEngine.Time.time + (conf.keepTime.HasValue ? conf.keepTime.Value : 1);

            return true;
        }

        private void onVFXDestory()
        {
            UnityGameApp.Inst.WeaponManager.onExplosiveDestory(this);
        }

        public void onHitEnter(UnityEngine.Collider other)
        {
            var rigibody = other.gameObject.GetComponent<UnityEngine.Rigidbody>();
            if(rigibody == null)
            {
                return;
            }

            _hitedObjects.Add(other.gameObject);
        }
        public void onHitExit(UnityEngine.Collider other)
        {
            _hitedObjects.Remove(other.gameObject);
        }

        public void OnUpdate()
        {
            if(UnityEngine.Time.time < _startTime || UnityEngine.Time.time > _endTime)
            {
                return;
            }

            foreach(var obj in _hitedObjects)
            {
                var rigibody = obj.GetComponent<UnityEngine.Rigidbody>();
                if (rigibody == null)
                {
                    continue;
                }

                var vec = (obj.transform.position - _explosiveVFX.unityGameObject.transform.position).normalized;
                vec.y += 0.3f;
                rigibody.AddForce(vec.normalized * 100);
            }
        }
    }
}
