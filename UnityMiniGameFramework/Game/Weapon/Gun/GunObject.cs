using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class UnityProjectileCollider : UnityEngine.MonoBehaviour
    {
        public GunObject gunObject;
        public VFXObjectBase projVfxObj;

        private void Start()
        {
        }

        private void OnCollisionEnter(UnityEngine.Collision collision)
        {
            gunObject.onProjectileHit(this, collision);
        }

    }
    public class UnityEmmiterCollider : UnityEngine.MonoBehaviour
    {
        public GunObject gunObject;
        public VFXObjectBase emmterVfxObj;

        private void Start()
        {
        }

        private void OnTriggerEnter(UnityEngine.Collider other)
        {
            gunObject.onEmmiterHitEnter(other);
        }

        private void OnTriggerExit(UnityEngine.Collider other)
        {
            gunObject.onEmmiterHitExit(other);
        }
    }

    public class GunObject : MGGameObject
    {
        override public string type => "GunObject";
        new public static GunObject create()
        {
            return new GunObject();
        }

        protected AnimatorComponent _animatorComponent;
        public AnimatorComponent animatorComponent => _animatorComponent;

        protected GunConf _conf;

        protected bool _isOpenFire;
        public bool isOpenFire => _isOpenFire;

        protected UnityEngine.GameObject _gunPos;
        protected float _currCD;

        protected float _projectFlySpeed;
        protected HashSet<VFXObjectBase> _currentProjectiles;

        protected VFXObjectBase _throwEmmiter;
        protected Dictionary<UnityEngine.GameObject, float> _emmiterHitObjectsTime;

        protected float _maxRayLength;
        protected VFXLinerObject _rayVFX;
        protected VFXObjectBase _rayImpactVFX;
        protected UnityEngine.GameObject _currentRayHitObject;
        protected UnityEngine.Ray _currentRay;
        protected UnityEngine.RaycastHit _currentHitPoint;

        public GunObject()
        {
            _currentProjectiles = new HashSet<VFXObjectBase>();
            _currentRay = new UnityEngine.Ray();
            _currentHitPoint = new UnityEngine.RaycastHit();
        }

        virtual protected GunConf _getGunConf(string confname)
        {
            if (UnityGameApp.Inst.CharacterManager.CharacterConfs == null)
            {
                return null;
            }
            return UnityGameApp.Inst.WeaponManager.WeaponConf.getGunConf(confname);
        }

        override public void Init(string confname)
        {
            base.Init(confname);

            _conf = _getGunConf(confname);
            if (_conf == null)
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Init gun config({confname}) not exist.");
                return;
            }

            if (_conf.FireConf == null)
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Init gun config({confname}) FireConf exist.");
                return;
            }

            var tr = this._unityGameObject.transform.Find("GunPos");
            if (tr == null)
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Init gun config({confname}) GunPos exist.");
                return;
            }
            
            _projectFlySpeed = _conf.FireConf.projectileFlySpeed.HasValue ? _conf.FireConf.projectileFlySpeed.Value : 10;
            _maxRayLength = _conf.FireConf.maxRayLength.HasValue ? _conf.FireConf.maxRayLength.Value : 10;
            _gunPos = tr.gameObject;
            _name = _conf.name;

            if (_conf.AnimatorConf != null)
            {
                _animatorComponent = (AnimatorComponent)GameObjectManager.createGameObjectComponent(_conf.AnimatorConf.componentType);
                this.AddComponent(_animatorComponent);
                _animatorComponent.Init(_conf.AnimatorConf);

                _animatorComponent.playAnimation("Idle");
            }

        }

        virtual public void Fire()
        {
            if(_isOpenFire)
            {
                return;
            }

            _isOpenFire = true;

            if(_animatorComponent != null)
            {
                if (_animatorComponent.currBaseAnimation.aniName != "Fire")
                {
                    _animatorComponent.playAnimation("Fire");
                }
            }

            switch (_conf.FireConf.fireType)
            {
                case "projectile":
                    _onOpenfireProjectile();
                    break;
                case "ray":
                    _onOpenfireRay();
                    break;
                case "emmiter":
                    _onOpenfireEmmiter();
                    break;
            }
        }

        virtual public void StopFire()
        {
            if(!_isOpenFire)
            {
                return;
            }

            _isOpenFire = false;

            if (_animatorComponent != null)
            {
                if (_animatorComponent.currBaseAnimation.aniName != "Idle")
                {
                    _animatorComponent.playAnimation("Idle");
                }
            }

            switch (_conf.FireConf.fireType)
            {
                case "projectile":
                    _onStopfireProjectile();
                    break;
                case "ray":
                    _onStopfireRay();
                    break;
                case "emmiter":
                    _onStopfireEmmiter();
                    break;
            }
        }

        override public void Dispose()
        {
            base.Dispose();
        }

        override public void OnUpdate(float timeElasped)
        {
            base.OnUpdate(timeElasped);

            switch (_conf.FireConf.fireType)
            {
                case "projectile":
                    _updateProjectile();
                    break;
                case "ray":
                    _updatefireRay();
                    break;
                case "emmiter":
                    _updateEmmiter();
                    break;
            }

            if (!_isOpenFire)
            {
                return;
            }

            _currCD -= UnityEngine.Time.deltaTime;
            if (_currCD > 0)
            {
                return;
            }

            if(_doFire())
            {
                _currCD = _conf.FireConf.fireCdTime;
            }

        }
        override public void OnPostUpdate(float timeElasped)
        {


            base.OnPostUpdate(timeElasped);
        }

        virtual public void onProjectileHit(UnityProjectileCollider projectileObject, UnityEngine.Collision collision)
        {
            var contact = collision.GetContact(0);

            if(_conf.FireConf.hitVFX != null)
            {
                var hitVfx = UnityGameApp.Inst.VFXManager.createVFXObject(_conf.FireConf.hitVFX);
                if (hitVfx != null)
                {
                    hitVfx.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);
                    hitVfx.unityGameObject.transform.position = contact.point;
                    hitVfx.unityGameObject.transform.forward = contact.normal;
                }
            }

            // TO DO : do hit result
            var rigiBody = collision.gameObject.GetComponent<UnityEngine.Rigidbody>();
            if(rigiBody != null)
            {
                rigiBody.AddForce(collision.impulse.normalized * 100);
            }

            UnityGameApp.Inst.VFXManager.onVFXDestory(projectileObject.projVfxObj);
            _currentProjectiles.Remove(projectileObject.projVfxObj);

            if(_conf.FireConf.collideExplosive != null)
            {
                var explosiveObj = UnityGameApp.Inst.WeaponManager.CreateExplosiveObject(_conf.FireConf.collideExplosive);
                if(explosiveObj.explosiveVFX != null)
                {
                    explosiveObj.explosiveVFX.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);
                    explosiveObj.explosiveVFX.unityGameObject.transform.position = contact.point;
                    explosiveObj.explosiveVFX.unityGameObject.transform.forward = contact.normal;
                }
            }
        }

        virtual public void onEmmiterHitEnter(UnityEngine.Collider other)
        {
            _emmiterHitObjectsTime[other.gameObject] = UnityEngine.Time.time;
            _onEmmiterHit(other.gameObject);
        }
        virtual public void onEmmiterHitExit(UnityEngine.Collider other)
        {
            _emmiterHitObjectsTime.Remove(other.gameObject);
        }

        virtual protected void _onEmmiterHit(UnityEngine.GameObject o)
        {
            if (_conf.FireConf.hitVFX != null)
            {
                var hitVfx = UnityGameApp.Inst.VFXManager.createVFXObject(_conf.FireConf.hitVFX);
                if (hitVfx != null)
                {
                    hitVfx.unityGameObject.transform.SetParent(o.transform);
                    hitVfx.unityGameObject.transform.localPosition = UnityEngine.Vector3.zero;
                }
            }

            // TO DO : do hit result

            var rigiBody = o.GetComponent<UnityEngine.Rigidbody>();
            if (rigiBody != null)
            {
                rigiBody.AddForce((o.transform.position - this.unityGameObject.transform.position).normalized * 100);
            }
        }

        virtual protected void _updateProjectile()
        {
            VFXObjectBase[] curProjs = _currentProjectiles.ToArray();
            foreach (var proj in curProjs)
            {
                if((proj.unityGameObject.transform.position - this.unityGameObject.transform.position).magnitude > 100)
                {
                    _currentProjectiles.Remove(proj);
                    UnityGameApp.Inst.VFXManager.onVFXDestory(proj);
                    continue;
                }
                proj.unityGameObject.transform.position = proj.unityGameObject.transform.position + proj.unityGameObject.transform.forward * _projectFlySpeed * UnityEngine.Time.deltaTime;  
            }
        }

        virtual protected void _updatefireRay()
        {
            if(_rayVFX == null)
            {
                return;
            }

            _currentRay.direction = _gunPos.transform.forward;
            _currentRay.origin = _gunPos.transform.position;

            var rayLength = _maxRayLength;
            if (UnityEngine.Physics.Raycast(_currentRay, out _currentHitPoint, _maxRayLength))
            {
                rayLength = UnityEngine.Vector3.Distance(_gunPos.transform.position, _currentHitPoint.point);

                _currentRayHitObject = _currentHitPoint.collider.gameObject;
            }
            else
            {
                _currentRayHitObject = null;
            }

            _rayVFX.linerRender.SetPosition(1, new UnityEngine.Vector3(0f, 0f, rayLength));

            // Adjust impact effect position
            if (_rayImpactVFX != null)
            {
                _rayImpactVFX.unityGameObject.transform.position = _gunPos.transform.position + _gunPos.transform.forward * rayLength;
            }
        }

        virtual protected void _updateEmmiter()
        {
            if(_emmiterHitObjectsTime == null)
            {
                return;
            }

            UnityEngine.GameObject[] keys = _emmiterHitObjectsTime.Keys.ToArray();
            foreach (var key in keys)
            {
                var value = _emmiterHitObjectsTime[key];
                if (UnityEngine.Time.time - value < _conf.FireConf.fireCdTime)
                {
                    continue;
                }

                _emmiterHitObjectsTime[key] = UnityEngine.Time.time;
                _onEmmiterHit(key);
            }
        }

        virtual protected bool _doFire()
        {
            switch (_conf.FireConf.fireType)
            {
                case "projectile":
                    return _dofireProjectile();
                case "ray":
                    return _dofireRay();
                case "emmiter":
                    return _dofireEmmiter();
            }

            return true;
        }

        virtual protected bool _dofireProjectile()
        {
            if (_conf.FireConf.shootVFX != null)
            {
                var shootVfx = UnityGameApp.Inst.VFXManager.createVFXObject(_conf.FireConf.shootVFX);
                if (shootVfx != null)
                {
                    shootVfx.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);
                    shootVfx.unityGameObject.transform.position = _gunPos.transform.position;
                    shootVfx.unityGameObject.transform.rotation = _gunPos.transform.rotation;
                }
            }

            var proj = UnityGameApp.Inst.VFXManager.createVFXObject(_conf.FireConf.bulletVFX);
            if(proj == null)
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Gun ({_name}) create projectile ({_conf.FireConf.bulletVFX}) failed.");
                return false;
            }

            proj.unityGameObject.layer = UnityEngine.LayerMask.NameToLayer("Self");
            var collider = proj.unityGameObject.AddComponent<UnityProjectileCollider>();
            collider.gunObject = this;
            collider.projVfxObj = proj;

            proj.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);
            proj.unityGameObject.transform.position = _gunPos.transform.position;
            proj.unityGameObject.transform.forward = _gunPos.transform.forward;

            _currentProjectiles.Add(proj);

            return true;
        }
        virtual protected bool _dofireRay()
        {
            if(_currentRayHitObject == null)
            {
                return false;
            }

            if (_conf.FireConf.hitVFX != null)
            {
                var hitVfx = UnityGameApp.Inst.VFXManager.createVFXObject(_conf.FireConf.hitVFX);
                if (hitVfx != null)
                {
                    hitVfx.unityGameObject.transform.SetParent(_currentHitPoint.collider.gameObject.transform);
                    hitVfx.unityGameObject.transform.localPosition = UnityEngine.Vector3.zero;
                }
            }

            // TO DO : do hit result

            var rigiBody = _currentRayHitObject.GetComponent<UnityEngine.Rigidbody>();
            if (rigiBody != null)
            {
                rigiBody.AddForce(_currentRay.direction.normalized * 100);
            }

            return true;
        }
        virtual protected bool _dofireEmmiter()
        {
            return true;
        }


        virtual protected void _onOpenfireProjectile()
        {

        }
        virtual protected void _onOpenfireRay()
        {
            _rayVFX = UnityGameApp.Inst.VFXManager.createVFXObject(_conf.FireConf.bulletVFX) as VFXLinerObject;
            if (_rayVFX == null)
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Gun ({_name}) create ray ({_conf.FireConf.bulletVFX}) failed.");
                return;
            }

            _rayVFX.unityGameObject.transform.SetParent(_gunPos.transform);
            _rayVFX.unityGameObject.transform.localPosition = UnityEngine.Vector3.zero;
            _rayVFX.unityGameObject.transform.forward = _gunPos.transform.forward;

            if (_conf.FireConf.rayImpactVFX != null)
            {
                _rayImpactVFX = UnityGameApp.Inst.VFXManager.createVFXObject(_conf.FireConf.rayImpactVFX);
                if (_rayImpactVFX != null)
                {
                    _rayImpactVFX.unityGameObject.transform.SetParent(_gunPos.transform);
                    _rayImpactVFX.unityGameObject.transform.localPosition = UnityEngine.Vector3.zero;
                    _rayImpactVFX.unityGameObject.transform.forward = _gunPos.transform.forward;
                }
            }

        }
        virtual protected void _onOpenfireEmmiter()
        {
            _throwEmmiter = UnityGameApp.Inst.VFXManager.createVFXObject(_conf.FireConf.bulletVFX);
            if (_throwEmmiter == null)
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Gun ({_name}) create emmiter ({_conf.FireConf.bulletVFX}) failed.");
                return;
            }

            _throwEmmiter.unityGameObject.layer = UnityEngine.LayerMask.NameToLayer("Self");
            var collider = _throwEmmiter.unityGameObject.AddComponent<UnityEmmiterCollider>();
            collider.gunObject = this;
            collider.emmterVfxObj = _throwEmmiter;

            _throwEmmiter.unityGameObject.transform.SetParent(_gunPos.transform);
            _throwEmmiter.unityGameObject.transform.localPosition = UnityEngine.Vector3.zero;
            _throwEmmiter.unityGameObject.transform.forward = _gunPos.transform.forward;

            _emmiterHitObjectsTime = new Dictionary<UnityEngine.GameObject, float>();
        }
        virtual protected void _onStopfireProjectile()
        {

        }
        virtual protected void _onStopfireRay()
        {
            if(_rayVFX != null)
            {
                UnityGameApp.Inst.VFXManager.onVFXDestory(_rayVFX);
                _rayVFX = null;
            }

            if(_rayImpactVFX != null)
            {
                UnityGameApp.Inst.VFXManager.onVFXDestory(_rayImpactVFX);
                _rayImpactVFX = null;
            }

            _currentRayHitObject = null;
        }
        virtual protected void _onStopfireEmmiter()
        {
            if(_throwEmmiter == null)
            {
                return;
            }


            UnityGameApp.Inst.VFXManager.onVFXDestory(_throwEmmiter);
            _throwEmmiter = null;

            _emmiterHitObjectsTime = null;
        }
    }
}
