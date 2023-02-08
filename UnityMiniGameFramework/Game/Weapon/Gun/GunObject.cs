using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = MiniGameFramework.Debug;
using GameObject = MiniGameFramework.GameObject;
using Matrix4x4 = UnityEngine.Matrix4x4;

namespace UnityMiniGameFramework
{
    public class UnityProjectileCollider : UnityEngine.MonoBehaviour
    {
        public GunObject gunObject;
        public VFXObjectBase projVfxObj;
        public int pierceCount;

        // private void OnCollisionEnter(UnityEngine.Collision collision)
        // {
        //     var contact = collision.GetContact(0);
        //     gunObject.onProjectileHit(this, contact.point, collision.gameObject);
        // }
        

        private void OnTriggerEnter(UnityEngine.Collider other)
        {
            gunObject.onProjectileHit(this, other.ClosestPoint(this.gameObject.transform.position), other.gameObject);
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

    public class GunObject : WeaponObject
    {

        override public string type => "GunObject";
        new public static GunObject create()
        {
            return new GunObject();
        }

        protected AnimatorComponent _animatorComponent;
        public AnimatorComponent animatorComponent => _animatorComponent;

        protected GunConf _conf;
        public GunConf conf => _conf;

        protected bool _isOpenFire;
        public bool isOpenFire => _isOpenFire;

        protected UnityEngine.GameObject _gunPos;

        public UnityEngine.GameObject GunPos => _gunPos;

        protected float _currCD;

        protected float _projectFlySpeed;
        protected Dictionary<VFXObjectBase, UnityEngine.GameObject> _currentProjectiles;
        
        protected string _fireAudio;
        protected string _hitAudio;

        protected VFXObjectBase _throwEmmiter;
        protected Dictionary<UnityEngine.GameObject, float> _emmiterHitObjectsTime;

        protected VFXLinerObject _rayVFX;
        protected VFXObjectBase _rayImpactVFX;
        protected UnityEngine.GameObject _currentRayHitObject;
        protected UnityEngine.Ray _currentRay;
        protected UnityEngine.RaycastHit _currentHitPoint;
        

        protected ActorObject _currentTarget;

        protected float _hitForce;
        protected int _Multiple;
        protected int _BulletCount;
        protected int _PierceCount;
        protected float _BlastRange; //投掷物的爆炸范围
        protected float _shootOffsetAngleBegin;
        protected float _shootOffsetAngleEnd;
        
        protected float _attackRange;
        public float attackRange => _attackRange;

        protected float _fireCd;
        protected float _baseAttackSpeedRate;
        protected float _projectilesRotationAngle = 0.0f;
        protected int _gunPosIndex = 0;

        protected static string[] _layers = new string[] { "Hitable", "Default", "Ground" };

        public GunObject()
        {
            _currentProjectiles = new Dictionary<VFXObjectBase, UnityEngine.GameObject>();
            _currentRay = new UnityEngine.Ray();
            _currentHitPoint = new UnityEngine.RaycastHit();
        }

        public void AddAttackRange(float rangeAdd)
        {
            _attackRange = GetBaseAttackRange();
            _attackRange += rangeAdd;
            _attackRange = 30;
        }
        
        public void SetAttackRange(float range)
        {
            _attackRange = range;
        }

        private float GetBaseAttackRange()
        {
            return _conf.FireConf.attackRange.HasValue ? _conf.FireConf.attackRange.Value : 5;
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
            _fireAudio = string.IsNullOrEmpty(_conf.FireConf.fireAudio) ? string.Empty : _conf.FireConf.fireAudio;
            _hitAudio = string.IsNullOrEmpty(_conf.FireConf.hitAudio) ? string.Empty : _conf.FireConf.hitAudio;
            
            
            _gunPos = tr.gameObject;
            _name = _conf.name;

            _hitForce = _conf.FireConf.hitForce ?? 0;
            _Multiple = _conf.FireConf.Multiple ?? 1;
            _BulletCount = _conf.FireConf.bulletCount ?? 1;
            _PierceCount = _conf.FireConf.pierceCount ?? 1;
            _BlastRange =
                (_conf.FireConf.collideExplosive != null && _conf.FireConf.collideExplosive.blastRange != null)
                    ? _conf.FireConf.collideExplosive.blastRange.Value
                    : 0.45f;
            _shootOffsetAngleBegin = _conf.FireConf.shootOffsetAngleBegin.HasValue
                ? _conf.FireConf.shootOffsetAngleBegin.Value
                : 0;
            
            _shootOffsetAngleEnd = _conf.FireConf.shootOffsetAngleEnd.HasValue
                ? _conf.FireConf.shootOffsetAngleEnd.Value
                : 0;
            
            _attackRange = GetBaseAttackRange();
            _baseAttackSpeedRate = _conf.FireConf.baseattackspeedrate.HasValue
                ? _conf.FireConf.baseattackspeedrate.Value
                : 100.0f;
            //默认初始值，最后由UpdateFireCd负责更新
            _fireCd = _conf.FireConf.fireCdTime;
            if (_conf.AnimatorConf != null)
            {
                _animatorComponent = (AnimatorComponent)GameObjectManager.createGameObjectComponent(_conf.AnimatorConf.componentType);
                this.AddComponent(_animatorComponent);
                _animatorComponent.Init(_conf.AnimatorConf);

                _animatorComponent.playAnimation("Idle");
            }

        }

        public void UpdateFireCd(int weaponLevelAttackSpeed)
        {
            _fireCd = _baseAttackSpeedRate * 1.0f / weaponLevelAttackSpeed;
            _fireCd /= _BulletCount;
            Debug.DebugOutput(DebugTraceType.DTT_Debug, $"{holder.name}的开火CD为:{_fireCd}s");
        }

        public void UpdateBulletCount(int? count)
        {
            if (count != null)
                _BulletCount = count.Value;
        }

        public void UpdatePierceCount(int? count)
        {
            if (count != null)
                _PierceCount = count.Value;
        }

        public void UpdateBlastRange(float?range)
        {
            if (range != null)
                _BlastRange = range.Value;
        }
        
        /// <summary>
        /// GM工具
        /// </summary>
        /// <param name="weaponLevelAttackSpeed"></param>
        public void GM_UpdateFireCd(int weaponLevelAttackSpeed)
        {
            UpdateFireCd(weaponLevelAttackSpeed);
            _currCD = _fireCd;
        }
        
        public virtual void Fire(ActorObject target)
        {
            _currentTarget = target;

            if (_isOpenFire)
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
                case "multiprojectile":
                    _onOpenfireMultiProjectile();
                    break;
            }
        }

        virtual public void StopFire()
        {
            _currentTarget = null;

            if (!_isOpenFire)
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
                case "multiprojectile":
                    _onStopfireMultiProjectile();
                    break;
            }
        }

        override public void Dispose()
        {
            base.Dispose();

            // clear projectiles
            if(_currentProjectiles != null)
            {
                foreach(var pair in _currentProjectiles)
                {
                    UnityGameApp.Inst.VFXManager.onVFXDestory(pair.Key);
                }
                _currentProjectiles = null;
            }

            // clear ray
            if (_rayVFX != null)
            {
                UnityGameApp.Inst.VFXManager.onVFXDestory(_rayVFX);
                _rayVFX = null;
            }
            if (_rayImpactVFX != null)
            {
                UnityGameApp.Inst.VFXManager.onVFXDestory(_rayImpactVFX);
                _rayImpactVFX = null;
            }

            // clear emmiter
            if (_throwEmmiter != null)
            {
                UnityGameApp.Inst.VFXManager.onVFXDestory(_throwEmmiter);
                _throwEmmiter = null;

                foreach (var pair in _emmiterHitObjectsTime)
                {
                    var ugo = pair.Key.GetComponent<UnityGameObjectBehaviour>();
                    if (ugo != null)
                    {
                        var actor = ugo.mgGameObject as ActorObject;
                        if (actor != null)
                        {
                            actor.OnDispose -= Actor_OnDispose;
                        }
                    }
                }
                _emmiterHitObjectsTime = null;
            }
        }

        override public void OnUpdate(float timeElasped)
        {
            base.OnUpdate(timeElasped);

            switch (_conf.FireConf.fireType)
            {
                case "projectile":
                case "multiprojectile":
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

            Debug.DebugOutput(DebugTraceType.DTT_Debug, $"doFireTime:{Time.time}");
            if(_doFire())
            {
                _doFireAudio();
                _currCD = _fireCd;
            }

        }
        override public void OnPostUpdate(float timeElasped)
        {


            base.OnPostUpdate(timeElasped);
        }
        
        
        virtual public void onProjectileHit(UnityProjectileCollider projectileObject, UnityEngine.Vector3 hitPoint, UnityEngine.GameObject other)
        {

            if (_conf.FireConf.hitVFX != null)
            {
                var hitVfx = UnityGameApp.Inst.VFXManager.createVFXObject(_conf.FireConf.hitVFX);
                if (hitVfx != null)
                {
                    hitVfx.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);
                    hitVfx.unityGameObject.transform.position = hitPoint;
                    UnityGameApp.Inst.VFXManager.setVFXColliderIsTrigger(hitVfx.unityGameObject);
                    //hitVfx.unityGameObject.transform.forward = contact.normal;
                }
            }

            if (ActBuffs != null)
            {
                foreach (var actBuffConfig in ActBuffs)
                {
                    if (actBuffConfig.isVaild() && !string.IsNullOrEmpty(actBuffConfig.bufVFXName))
                    {
                        var buffHitVfx = UnityGameApp.Inst.VFXManager.createVFXObject(actBuffConfig.bufVFXName);
                        if (buffHitVfx != null)
                        {
                            buffHitVfx.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);
                            buffHitVfx.unityGameObject.transform.position = hitPoint;
                            UnityGameApp.Inst.VFXManager.setVFXColliderIsTrigger(buffHitVfx.unityGameObject);
                        }
                    }
                }
                
            }
            
            // do hit result
            var ugbGameObj = other.GetComponent<UnityGameObjectBehaviour>();
            if (ugbGameObj != null)
            {
                var combComp = ugbGameObj.mgGameObject.getComponent("CombatComponent") as CombatComponent;
                if (combComp != null)
                {
                    _onHitAudio();
                    combComp.OnHitByWeapon(this);
                }
            }
            
            projectileObject.pierceCount--;
            if (projectileObject.pierceCount <= 0)
            {
                UnityGameApp.Inst.VFXManager.onVFXDestory(projectileObject.projVfxObj);
                _currentProjectiles.Remove(projectileObject.projVfxObj);    
            }
            
            if (_conf.FireConf.collideExplosive != null && _currentTarget != null)
            {
                var explosiveObj =
                    UnityGameApp.Inst.WeaponManager.CreateExplosiveObject(_conf.FireConf.collideExplosive);
                if (explosiveObj != null && explosiveObj.explosiveVFX != null)
                {
                    explosiveObj.setGunObject(this);
                    UnityGameApp.Inst.VFXManager.setVFXColliderIsTrigger(explosiveObj.explosiveVFX.unityGameObject);
                    explosiveObj.explosiveVFX.unityGameObject.transform.position = other.transform.position;
                        explosiveObj.explosiveVFX.unityGameObject.transform.SetParent(
                        ((MGGameObject) UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);
                        explosiveObj.explosiveVFX.unityGameObject.transform.localScale =
                            new Vector3(_BlastRange, _BlastRange, _BlastRange);

                }
            }
        }


        private void Actor_OnDispose(GameObject obj)
        {
            _emmiterHitObjectsTime.Remove((obj as ActorObject).unityGameObject);
        }

        virtual public void onEmmiterHitEnter(UnityEngine.Collider other)
        {
            _emmiterHitObjectsTime[other.gameObject] = UnityEngine.Time.time;
            _onEmmiterHit(other.gameObject);

            var ugo = other.gameObject.GetComponent<UnityGameObjectBehaviour>();
            if(ugo != null)
            {
                var actor = ugo.mgGameObject as ActorObject;
                if(actor != null)
                {
                    actor.OnDispose += Actor_OnDispose;
                }
            }
        }

        virtual public void onEmmiterHitExit(UnityEngine.Collider other)
        {
            _emmiterHitObjectsTime.Remove(other.gameObject);

            var ugo = other.gameObject.GetComponent<UnityGameObjectBehaviour>();
            if (ugo != null)
            {
                var actor = ugo.mgGameObject as ActorObject;
                if (actor != null)
                {
                    actor.OnDispose -= Actor_OnDispose;
                }
            }
        }

        virtual protected void _onEmmiterHit(UnityEngine.GameObject o)
        {
            VFXObjectBase hitVfx = null;
            if (_conf.FireConf.hitVFX != null)
            {
                hitVfx = UnityGameApp.Inst.VFXManager.createVFXObject(_conf.FireConf.hitVFX);
                if (hitVfx != null)
                {
                    hitVfx.unityGameObject.transform.SetParent(o.transform);
                    hitVfx.unityGameObject.transform.localPosition = UnityEngine.Vector3.zero;
                    UnityGameApp.Inst.VFXManager.setVFXColliderIsTrigger(hitVfx.unityGameObject);
                }
            }
            
            var ugbGameObj = o.GetComponent<UnityGameObjectBehaviour>();
            if (ActBuffs != null)
            {
                foreach (var buffConfig in ActBuffs)
                {
                    if (buffConfig.isVaild() && !string.IsNullOrEmpty(buffConfig.bufVFXName))
                    {
                        var buffHitVfx = UnityGameApp.Inst.VFXManager.createVFXObject(buffConfig.bufVFXName);
                        if (buffHitVfx != null)
                        {
                            buffHitVfx.unityGameObject.transform.SetParent(
                                ((MGGameObject) UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);
                            buffHitVfx.unityGameObject.transform.position = Vector3.zero;
                            UnityGameApp.Inst.VFXManager.onVFXAttachToGameObj(buffHitVfx, ugbGameObj.mgGameObject);
                            UnityGameApp.Inst.VFXManager.setVFXColliderIsTrigger(buffHitVfx.unityGameObject);
                        }
                    }
                }
            }

            // do hit result
            if (ugbGameObj != null)
            {
                var combComp = ugbGameObj.mgGameObject.getComponent("CombatComponent") as CombatComponent;
                if (combComp != null)
                {
                    _onHitAudio();
                    combComp.OnHitByWeapon(this);
                }

                if(hitVfx != null)
                {
                    UnityGameApp.Inst.VFXManager.onVFXAttachToGameObj(hitVfx, ugbGameObj.mgGameObject);
                    UnityGameApp.Inst.VFXManager.setVFXColliderIsTrigger(hitVfx.unityGameObject);
                }
            }

            //var rigiBody = o.GetComponent<UnityEngine.Rigidbody>();
            //if (rigiBody != null)
            //{
            //    rigiBody.AddForce((o.transform.position - this.unityGameObject.transform.position).normalized * _hitForce);
            //}
        }

        virtual protected void _updateProjectile()
        {
            VFXObjectBase[] curProjs = _currentProjectiles.Keys.ToArray();
            foreach (var proj in curProjs)
            {
                if((proj.unityGameObject.transform.position - this.unityGameObject.transform.position).magnitude > _attackRange)
                {
                    _currentProjectiles.Remove(proj);
                    UnityGameApp.Inst.VFXManager.onVFXDestory(proj);

                    // TO DO : try explosive

                    continue;
                }

                var targetObject = _currentProjectiles[proj];
                //if(targetObject == null)
                //{
                //    _currentProjectiles.Remove(proj);
                //    UnityGameApp.Inst.VFXManager.onVFXDestory(proj);

                //    // TO DO : try explosive

                //    continue;
                //}

                // 判断projectile是否追踪
                UnityEngine.Vector3 vec;
                if (targetObject != null)
                {
                    var tarPos = new UnityEngine.Vector3(targetObject.transform.position.x, targetObject.transform.position.y + 0.5f, targetObject.transform.position.z);
                    vec = (tarPos - proj.unityGameObject.transform.position).normalized;
                    proj.unityGameObject.transform.forward = vec;
                }
                else
                {
                    vec = proj.unityGameObject.transform.forward;
                }

                //var tarPos = new UnityEngine.Vector3(targetObject.transform.position.x, targetObject.transform.position.y+0.5f, targetObject.transform.position.z);
                //var vec = (tarPos - proj.unityGameObject.transform.position).normalized;
                //proj.unityGameObject.transform.forward = vec;
                proj.unityGameObject.transform.position = proj.unityGameObject.transform.position + vec * _projectFlySpeed * UnityEngine.Time.deltaTime;  
            }
        }

        virtual protected void _updatefireRay()
        {
            if (_rayVFX == null)
            {
                return;
            }
            
            
            var forward = _gunPos.transform.forward;
            forward.y = 0;
            _currentRay.direction = forward;
            _currentRay.origin = _gunPos.transform.position;

            var rayLength = _attackRange;
            if (UnityEngine.Physics.Raycast(_currentRay, out _currentHitPoint, _attackRange, UnityEngine.LayerMask.GetMask(_layers)))
            {
                rayLength = UnityEngine.Vector3.Distance(_gunPos.transform.position, _currentHitPoint.point);

                _currentRayHitObject = _currentHitPoint.collider.gameObject;
            }
            else
            {
                _currentRayHitObject = null;
            }

            _rayVFX.linerRender.SetPosition(1, new UnityEngine.Vector3(0f, 0f, rayLength));
            
            _rayVFX.unityGameObject.transform.forward = forward; 
            // Adjust impact effect position
            if (_rayImpactVFX != null)
            {
                _rayImpactVFX.unityGameObject.transform.position = _gunPos.transform.position + _rayVFX.unityGameObject.transform.forward * rayLength;
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
                if(key == null)
                {
                    continue;
                }

                var value = _emmiterHitObjectsTime[key];
                if (UnityEngine.Time.time - value < _fireCd)
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
                case "multiprojectile":
                    return _dofireMultiProjectile();
            }
            return true;
        }

        protected virtual void _doFireAudio()
        {
            if (!string.IsNullOrEmpty(_fireAudio))
            {
                UnityGameApp.Inst.AudioManager.PlaySFXByAudioName(_fireAudio);
            }
        }

        protected virtual void _onHitAudio()
        {
            if (!string.IsNullOrEmpty(_hitAudio))
            {
                UnityGameApp.Inst.AudioManager.PlaySFXByAudioName(_hitAudio);
            }
        }

        virtual protected bool _dofireProjectile()
        {
            if (_animatorComponent != null)
            {
                if (_animatorComponent.currBaseAnimation.aniName != "Fire")
                {
                    _animatorComponent.playAnimation("Fire");
                }
            }

            bool isRotationFire = _BulletCount > 1 && _gunPos.transform.childCount > 0;

            Transform gunPosTransform = isRotationFire ? _gunPos.transform.GetChild(_gunPosIndex) : _gunPos.transform;

            Vector3 gunPosition = gunPosTransform.position;
            if (isRotationFire)
            {
                gunPosition.x = (float) (gunPosition.x * Math.Cos(_projectilesRotationAngle) +
                                         gunPosition.y * Math.Sin(_projectilesRotationAngle));
                gunPosition.y = (float) (gunPosition.y * Math.Cos(_projectilesRotationAngle) - gunPosition.x *
                                         Math.Sin(_projectilesRotationAngle));
                _gunPosIndex++;
                if (_gunPosIndex >= _gunPos.transform.childCount)
                    _gunPosIndex = 0;
            }
                 
            
            if (_conf.FireConf.shootVFX != null)
            {
                var shootVfx = UnityGameApp.Inst.VFXManager.createVFXObject(_conf.FireConf.shootVFX);
                if (shootVfx != null)
                {
                    shootVfx.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);
                    shootVfx.unityGameObject.transform.position = gunPosition;
                    shootVfx.unityGameObject.transform.rotation = gunPosTransform.rotation;
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
            collider.pierceCount = _PierceCount;
            UnityGameApp.Inst.VFXManager.setVFXColliderIsTrigger(proj.unityGameObject);
            
            proj.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);
            proj.unityGameObject.transform.position = gunPosition;
            Vector3 gunForward = _gunPos.transform.forward;
            gunForward = (new Vector3(gunForward.x, 0, gunForward.z)).normalized;

            proj.unityGameObject.transform.forward = gunForward;

            _currentProjectiles[proj] = _currentTarget.unityGameObject;
            if (_PierceCount > 1)
                _currentProjectiles[proj] = null;    
            return true;
        }

        virtual protected bool _dofireMultiProjectile()
        {
            if (_animatorComponent != null)
            {
                if (_animatorComponent.currBaseAnimation.aniName != "Fire")
                {
                    _animatorComponent.playAnimation("Fire");
                }
            }

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
            
            Vector3 gunForward = _gunPos.transform.forward;

            gunForward = (new Vector3(gunForward.x, 0, gunForward.z)).normalized;

            float _base = _shootOffsetAngleBegin;
            float _add = Math.Abs(_shootOffsetAngleEnd - _shootOffsetAngleBegin) * 1.0f / _Multiple;
            Vector3 rotationAddVec = Vector3.zero;
            for (uint i = 0; i < _Multiple; ++i)
            {
                var proj = UnityGameApp.Inst.VFXManager.createVFXObject(_conf.FireConf.bulletVFX);
                if (proj == null)
                {
                    MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error,
                        $"Gun ({_name}) create projectile ({_conf.FireConf.bulletVFX}) failed.");
                    return false;
                }

                UnityGameApp.Inst.VFXManager.setVFXColliderIsTrigger(proj.unityGameObject);
                proj.unityGameObject.layer = UnityEngine.LayerMask.NameToLayer("Self");
                var collider = proj.unityGameObject.AddComponent<UnityProjectileCollider>();
                collider.gunObject = this;
                collider.projVfxObj = proj;
                collider.pierceCount =  _PierceCount;

                proj.unityGameObject.transform.SetParent(((MGGameObject) UnityGameApp.Inst.MainScene.sceneRootObj)
                    .unityGameObject.transform);
                proj.unityGameObject.transform.position = _gunPos.transform.position;
                rotationAddVec.y = _base + _add;
                proj.unityGameObject.transform.forward = Matrix4x4.Rotate(UnityEngine.Quaternion.Euler(rotationAddVec)).MultiplyVector(gunForward);
                _base += _add;
                _currentProjectiles[proj] = null;
            }

            return true;
        }
        virtual protected bool _dofireRay()
        {
            if(_currentRayHitObject == null)
            {
                return false;
            }

            VFXObjectBase hitVfx = null;
            if (_conf.FireConf.hitVFX != null)
            {
                hitVfx = UnityGameApp.Inst.VFXManager.createVFXObject(_conf.FireConf.hitVFX);
                if (hitVfx != null)
                {
                    hitVfx.unityGameObject.transform.SetParent(_currentRayHitObject.transform);
                    hitVfx.unityGameObject.transform.localPosition = UnityEngine.Vector3.zero;
                    UnityGameApp.Inst.VFXManager.setVFXColliderIsTrigger(hitVfx.unityGameObject);
                }
            }
            
            var ugbGameObj = _currentRayHitObject.GetComponent<UnityGameObjectBehaviour>();
            if (ActBuffs != null)
            {
                foreach (var buffConfig in ActBuffs)
                {
                    if (buffConfig.isVaild() && !string.IsNullOrEmpty(buffConfig.bufVFXName))
                    {
                        var buffHitVfx = UnityGameApp.Inst.VFXManager.createVFXObject(buffConfig.bufVFXName);
                        if (buffHitVfx != null)
                        {
                            buffHitVfx.unityGameObject.transform.SetParent(
                                ((MGGameObject) UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);
                            buffHitVfx.unityGameObject.transform.position = Vector3.zero;
                            UnityGameApp.Inst.VFXManager.onVFXAttachToGameObj(buffHitVfx, ugbGameObj.mgGameObject);
                            UnityGameApp.Inst.VFXManager.setVFXColliderIsTrigger(buffHitVfx.unityGameObject);
                        }
                    }
                }
            }

            // do hit result
            
            if (ugbGameObj != null)
            {
                var combComp = ugbGameObj.mgGameObject.getComponent("CombatComponent") as CombatComponent;
                if (combComp != null)
                {
                    _onHitAudio();
                    combComp.OnHitByWeapon(this);
                }

                if (hitVfx != null)
                {
                    UnityGameApp.Inst.VFXManager.onVFXAttachToGameObj(hitVfx, ugbGameObj.mgGameObject);
                }
            }

            //var rigiBody = _currentRayHitObject.GetComponent<UnityEngine.Rigidbody>();
            //if (rigiBody != null)
            //{
            //    rigiBody.AddForce(_currentRay.direction.normalized * _hitForce);
            //}

            return true;
        }
        virtual protected bool _dofireEmmiter()
        {
            return true;
        }


        virtual protected void _onOpenfireProjectile()
        {
            if (_BulletCount > 0)
            {
                _projectilesRotationAngle = 0.0f;
                _gunPosIndex = 0;
            }
                
        }
        virtual protected void _onOpenfireMultiProjectile()
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
            _projectilesRotationAngle = 0.0f;
            _gunPosIndex = 0;
        }
        virtual protected void _onStopfireMultiProjectile()
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

            foreach (var pair in _emmiterHitObjectsTime)
            {
                var ugo = pair.Key.GetComponent<UnityGameObjectBehaviour>();
                if (ugo != null)
                {
                    var actor = ugo.mgGameObject as ActorObject;
                    if (actor != null)
                    {
                        actor.OnDispose -= Actor_OnDispose;
                    }
                }
            }
            _emmiterHitObjectsTime = null;
        }
    }
}
