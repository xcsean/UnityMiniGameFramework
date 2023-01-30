using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    class BuffAttrNameDefine
    {
        public const string MOVE_SPEED = "MOVE_SPEED";
        public const string HP = "HP";
        public const string DEF = "DEF";
        public const string ATK = "ATK";
        public const string MISS = "MISS";
        public const string CRIT_RATE = "CRIT_RATE";
        public const string CRIT_PER = "CRIT_PER";
    }

    public class ActBuf
    {
        protected ActorObject _actor;
        public ActorObject actor => _actor;

        protected ActorObject _fromActor;
        public ActorObject fromActor => _fromActor;

        protected float _timeLeft;
        public float timeLeft => _timeLeft;

        protected ActBufConfig _conf;

        protected float _dotTimeLeft;

        public ActBuf(ActorObject actor, ActorObject fromActor)
        {
            _actor = actor;
            _fromActor = fromActor;
        }

        public void Init(ActBufConfig conf, float time = 30)
        {
            _conf = conf;
            _timeLeft = time;

            if (_conf.dot != null)
            {
                _dotTimeLeft = _conf.dot.time;
            }
        }

        protected virtual bool _checkTimeFinish()
        {
            _timeLeft -= UnityEngine.Time.deltaTime;
            if (_timeLeft > 0)
            {
                return false;
            }

            _timeLeft = 0;

            _actor.actionComponent.OnBufRemove(this);

            return true;
        }

        protected void _onAddState()
        {
            if (_conf.bufAddStates == null)
            {
                return;
            }

            foreach (var stKey in _conf.bufAddStates)
            {
                _actor.actionComponent.addState(stKey);
            }
        }

        protected void _onAddAttr()
        {
            if (_conf.bufAttrs == null)
            {
                return;
            }

            var combatComp = _actor.getComponent("CombatComponent") as CombatComponent;

            if (combatComp == null)
            {
                return;
            }

            foreach (var bufAttr in _conf.bufAttrs)
            {
                combatComp.AddBufAttr(bufAttr);
            }

            combatComp.RecalcAttributes();
        }

        public virtual void OnAdd()
        {
            _onAddState();

            _onAddAttr();
        }


        protected void _onRemoveState()
        {
            if (_conf.bufAddStates == null)
            {
                return;
            }

            foreach (var stKey in _conf.bufAddStates)
            {
                _actor.actionComponent.subState(stKey);
            }
        }

        protected void _onRemoveAttr()
        {
            if (_conf.bufAttrs == null)
            {
                return;
            }

            var combatComp = _actor.getComponent("CombatComponent") as CombatComponent;

            if (combatComp == null)
            {
                return;
            }

            foreach (var bufAttr in _conf.bufAttrs)
            {
                combatComp.RemoveBufAttr(bufAttr);
            }

            combatComp.RecalcAttributes();
        }

        public virtual void OnRemove()
        {
            _onRemoveState();

            _onRemoveAttr();
        }

        protected void _onDotUpdate()
        {
            if (_conf.dot == null)
            {
                return;
            }

            var combatComp = _actor.getComponent("CombatComponent") as CombatComponent;

            if (combatComp == null)
            {
                return;
            }

            _dotTimeLeft -= UnityEngine.Time.deltaTime;
            if (_dotTimeLeft <= 0)
            {
                _dotTimeLeft += _conf.dot.time;

                combatComp.OnDamageBy(_fromActor, _conf.dot.damage);
            }
        }

        public virtual void OnUpdate()
        {
            _onDotUpdate();
        }

        public virtual void OnPostUpdate()
        {
            _checkTimeFinish();
        }
    }
}