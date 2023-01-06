using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class HealthBar
    {
        UnityEngine.GameObject _barObject;
        public UnityEngine.GameObject barObject => _barObject;

        public void Init()
        {
            // TO DO : init bar object from manager

            // for Debug ...
            _barObject = ((UnityResourceManager)UnityGameApp.Inst.Resource).LoadUnityPrefabObject("actor/HealthBar");
            _barObject = UnityEngine.GameObject.Instantiate(_barObject);

            // TO DO : set transform to hp bar root
            _barObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);


        }

        public void Dispose()
        {
            UnityEngine.GameObject.Destroy(_barObject);
        }

        public void setHp(float per)
        {
            // TO DO : change shader to quad hp bar shader, set mat value, do batch reandering
            _barObject.transform.localScale = new UnityEngine.Vector3(per * 0.5f, _barObject.transform.localScale.y, _barObject.transform.localScale.z);
        }

        public void show()
        {
            _barObject.SetActive(true);
        }
        public void hide()
        {
            _barObject.SetActive(false);
        }
    }

    abstract public class CombatComponent : GameObjectComponent
    {
        protected HealthBar _hpBar;
        public HealthBar hpBar => _hpBar;

        protected int _HP;
        protected int _maxHP;

        protected int _Def;

        public Action<ActorObject> OnDie;

        protected bool _isDie;

        protected HashSet<ActBufAttrConfig> _bufAttrs;
        public HashSet<ActBufAttrConfig> bufAttrs => _bufAttrs;

        protected CombatConf _ccConf;

        public override void Init(object config)
        {
            base.Init(config);

            _ccConf = config as CombatConf;

            _HP = _ccConf.hpMax;
            _maxHP = _ccConf.hpMax;
            _Def = _ccConf.def;

            // for Debug ...
            // init hp bar

            _hpBar = new HealthBar();
            _hpBar.Init();

            _isDie = false;
            // TO DO : init hp bar from config

            _bufAttrs = new HashSet<ActBufAttrConfig>();
        }

        public override void Dispose()
        {
            base.Dispose();

            if(_hpBar != null)
            {
                _hpBar.Dispose();
                _hpBar = null;
            }
        }

        public override void OnUpdate(float timeElasped)
        {
            base.OnUpdate(timeElasped);

            var mgGameObject = _gameObject as MGGameObject;
            _hpBar.barObject.transform.position = new UnityEngine.Vector3(
                mgGameObject.unityGameObject.transform.position.x, 
                mgGameObject.unityGameObject.transform.position.y + 1.5f, 
                mgGameObject.unityGameObject.transform.position.z);
        }

        virtual public void OnHitByWeapon(WeaponObject weapon)
        {
            if (_isDie)
            {
                return;
            }

            // check missing
            var missed = UnityGameApp.Inst.Rand.RandomBetween(0, 10000);
            if (missed < weapon.attackInfo.missingRate * 10000)
            {
                // missed
                _onHitMissed(weapon.holder);
                return;
            }

            // rand dmg
            int dmg = UnityGameApp.Inst.Rand.RandomBetween(weapon.attackInfo.attackMin, weapon.attackInfo.attackMax);

            bool critical = false;
            var criticalHit = UnityGameApp.Inst.Rand.RandomBetween(0, 10000);
            if (criticalHit < weapon.attackInfo.criticalHitRate * 10000)
            {
                // critical
                dmg = (int)(dmg * weapon.attackInfo.criticalHitPer);
                critical = true;
            }

            OnDamageBy(weapon.holder, dmg, critical);
        }

        virtual public void OnDamageBy(ActorObject actor, int dmg, bool critical = false)
        {
            if (_isDie)
            {
                return;
            }

            dmg = dmg - _Def;

            _onDamage(actor, dmg, critical);

            _HP -= dmg;
            if (_HP < 0)
            {
                _HP = 0;
            }

            _hpBar.setHp((float)_HP / _maxHP);

            if (_HP <= 0)
            {
                _isDie = true;

                _onDie(actor);

                if (OnDie != null)
                {
                    OnDie(_gameObject as ActorObject);
                }
            }
        }

        public virtual void AddBufAttr(ActBufAttrConfig attr)
        {
            _bufAttrs.Add(attr);
        }

        public virtual void RemoveBufAttr(ActBufAttrConfig attr)
        {
            _bufAttrs.Remove(attr);
        }

        public virtual void RecalcAttributes()
        {

        }

        virtual protected void _onHitMissed(ActorObject actor)
        {

        }
        virtual protected void _onDamage(ActorObject actor, int dmg, bool critical)
        {

        }
        virtual protected void _onDie(ActorObject actor)
        {

        }

    }
}
