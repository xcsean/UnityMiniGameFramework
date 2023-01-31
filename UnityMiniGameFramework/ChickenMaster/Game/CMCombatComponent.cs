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

            _maxHP = (int)(_ccConf.hpMax * (1 + hpMul) + hpAdd);
            _Def = (int)(_ccConf.def * (1 + defMul) + defAdd);

            _HP = prevHP * _maxHP / prevMaxHP;

            OnRecalcAttributes();
        }
        override public void OnHitByWeapon(WeaponObject weapon)
        {
            base.OnHitByWeapon(weapon);

            // TO DO : add ActBuf to actor from weapon config
            if(_isDie)
                return;
            if(weapon.actBuf.isVaild())
            {
                OnBuffAddByActBuffConfig(weapon.actBuf, _actor, weapon.holder);
            }
        }

        protected virtual void OnBuffAddByActBuffConfig(ActBufConfig config, ActorObject actor, ActorObject fromActor)
        {
            if (!config.isVaild())
                return;
            ActBuf buff = new ActBuf(actor, fromActor);
            buff.Init(config, config.endTime);
            actor.actionComponent.AddBuf(buff);
        }
        
        override protected void _onHitMissed(ActorObject actor)
        {
            // TO DO : show missing text
        }
        override protected void _onDamage(ActorObject actor, int dmg, bool critical)
        {
            // TO DO : show damage text
            
            // perform onhit act
            if (_gameObject is MGGameObject mgGameObject)
            {
                Debug.DebugOutput(DebugTraceType.DTT_System,
                    $" ({_actor.name}) 受到来自({(actor != null ? actor.name : string.Empty)})的({dmg}点伤害)");
                BattleNumberEmitter.CreateNumAction.Invoke(mgGameObject.unityGameObject, dmg,
                    critical);
            }
                
        }
        override protected void _onDie(ActorObject actor)
        {
            _actor.actionComponent.AddAction(_dieAct);
        }
    }
}
