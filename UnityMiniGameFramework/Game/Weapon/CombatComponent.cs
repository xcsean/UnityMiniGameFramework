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
    }

    abstract public class CombatComponent : GameObjectComponent
    {
        protected HealthBar _hpBar;

        protected int _HP;
        protected int _maxHP;

        protected int _Def;

        public override void Init(object config)
        {
            base.Init(config);

            var ccConf = config as CombatConf;

            // TO DO : calculate level
            _HP = ccConf.hpMax;
            _maxHP = ccConf.hpMax;
            _Def = ccConf.def;

            // for Debug ...
            // init hp bar

            _hpBar = new HealthBar();
            _hpBar.Init();

            // TO DO : init hp bar from config

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

        virtual public void OnHitby(WeaponObject weapon)
        {
            // check missing
            var missed = UnityGameApp.Inst.Rand.RandomBetween(0, 10000);
            if(missed < weapon.attackInfo.missingRate * 10000)
            {
                // missed
                _onHitMissed(weapon);
                return;
            }

            // rand dmg
            int dmg = UnityGameApp.Inst.Rand.RandomBetween(weapon.attackInfo.attackMin, weapon.attackInfo.attackMax);

            dmg = dmg - _Def;

            bool critical = false;
            var criticalHit = UnityGameApp.Inst.Rand.RandomBetween(0, 10000);
            if(criticalHit < weapon.attackInfo.criticalHitRate * 10000)
            {
                // critical
                dmg = dmg * weapon.attackInfo.criticalHitPer;
                critical = true;
            }

            _onDamage(weapon, dmg, critical);

            _HP -= dmg;
            if(_HP < 0)
            {
                _HP = 0;
            }

            _hpBar.setHp((float)_HP / _maxHP);

            if (_HP <= 0)
            {
                _onDie(weapon);
            }
        }

        virtual protected void _onHitMissed(WeaponObject weapon)
        {

        }
        virtual protected void _onDamage(WeaponObject weapon, int dmg, bool critical)
        {

        }
        virtual protected void _onDie(WeaponObject weapon)
        {

        }
    }
}
