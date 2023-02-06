using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class CMCombatComponent : CombatComponent
    {
        override public string type => "CombatComponent";

        protected ActorObject _actor;
        protected RigibodyDieAct _dieAct;
        protected RigibodyOnHitAct _onHitAct;

        public event Action OnRecalcAttributes;

        public override void Init(object config)
        {
            base.Init(config);

            _actor = this.gameObject as ActorObject;

            _dieAct = new RigibodyDieAct(_actor);
        }


        public override void RecalcAttributes()
        {
            float hpAdd = 0;
            float hpMul = 0;
            float defAdd = 0;
            float defMul = 0;

            foreach (var bufAttr in _bufAttrs)
            {
                if (bufAttr.name == BuffAttrNameDefine.HP)
                {
                    hpAdd += bufAttr.addValue;
                    hpMul += bufAttr.mulValue;
                }
                else if (bufAttr.name == BuffAttrNameDefine.DEF)
                {
                    defAdd += bufAttr.addValue;
                    defMul += bufAttr.mulValue;
                }
            }

            int prevHP = _HP;
            int prevMaxHP = _maxHP;

            _maxHP = (int) (_ccConf.hpMax * (1 + hpMul) + hpAdd);
            _Def = (int) (_ccConf.def * (1 + defMul) + defAdd);

            _HP = prevHP * (_maxHP / prevMaxHP);

            OnRecalcAttributes();
        }

        protected override void OnDamageCalculation(WeaponObject weapon)
        {
            if (weapon.ActBuffs != null)
            {
                foreach (var actBuff in weapon.ActBuffs)
                {
                    OnBuffAddByActBuffConfig(actBuff, _actor, weapon.holder);
                }
            }

            base.OnDamageCalculation(weapon);
        }

        protected override int _onDamageCalculation(WeaponObject weapon)
        {
            int dmg = base._onDamageCalculation(weapon);
            if (weapon.ActBuffs == null)
                return dmg;

            foreach (var config in weapon.ActBuffs)
            {
                if (config.bufAttrs != null)
                    dmg += OnDamageCalByConf(config.bufAttrs);
            }

            return dmg;
        }

        public override int OnDamageCalByConf(List<ActBufAttrConfig> buffAttrs, int dmg = 0)
        {
            if (buffAttrs == null)
                return dmg;
            // 固定伤害
            int fixedDamage = 0;
            // 百分比伤害
            float perHP = 0;
            // 普通伤害
            int commonDamage = 0;
            float commonPerHp = 0;
            foreach (var attr in buffAttrs)
            {
                bool _isIgnoreArmor = false;
                {
                    if (attr.name == BuffAttrNameDefine.FIXED_DAMAGE)
                    {
                        _isIgnoreArmor = true;
                        fixedDamage += (int) attr.addValue;
                        perHP += attr.mulValue;
                    }
                    else if (attr.name == BuffAttrNameDefine.ATTACK_DAMAGE)
                    {
                        commonDamage += (int) attr.addValue;
                        commonPerHp += attr.mulValue;
                    }
                }

                if (_isIgnoreArmor)
                    dmg = (int) (_maxHP * perHP) + fixedDamage + dmg + _Def;
                dmg = dmg + (int) (_maxHP * commonPerHp) + commonDamage;
            }

            return dmg;
        }

        protected virtual void OnBuffAddByActBuffConfig(ActBufConfig config, ActorObject actor, ActorObject fromActor)
        {
            if (!config.isVaild())
                return;
            if(actor.actionComponent.HasBuf(config.buffUID))
                return;
            if (config.CheckAddBuff())
            {
                ActBuf buff = new ActBuf(actor, fromActor);
                buff.Init(config, config.endTime);
                actor.actionComponent.AddBuf(buff);
            }
        }

        override protected void _onHitMissed(ActorObject actor)
        {
            // TO DO : show missing text
        }

        private Dictionary<int, string> damageDesDic = new Dictionary<int, string>() {{1, "普通"}, {2, "暴击"}, {3, "dot"}};

        override protected void _onDamage(ActorObject actor, int dmg, DamageTypeEnum damageType)
        {
            // TO DO : show damage text

            // perform onhit act
            if (_gameObject is MGGameObject mgGameObject)
            {
                string damageDes = string.Empty;
                damageDesDic.TryGetValue((int) damageType, out damageDes);
                Debug.DebugOutput(DebugTraceType.DTT_System,
                    $" ({_actor.name}) 受到来自({(actor != null ? actor.name : string.Empty)})的({damageDes}{dmg}点伤害)");
                BattleNumberEmitter.CreateNumAction.Invoke(mgGameObject.unityGameObject, dmg,
                    damageType);
            }
        }

        override protected void _onDie(ActorObject actor)
        {
            _actor.actionComponent.AddAction(_dieAct);
        }

        public override void OnHitByWeapon(WeaponObject weapon)
        {
            if (_actor.type != "MapMonsterObject")
                return;
            base.OnHitByWeapon(weapon);
        }
    }
}