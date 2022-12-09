using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class AITryAttack : AIState
    {
        protected UnityEngine.GameObject _traceTarget;
        protected AttackAct _attackAct;

        protected float _attackRange; 

        public AITryAttack(ActorObject actor) : base(actor)
        {
            _attackAct = new AttackAct(actor);
            _attackRange = 1.0f; // TO DO : read from config
        }

        public void setTraceTarget(UnityEngine.GameObject unityGameObj)
        {
            _traceTarget = unityGameObj;
        }

        override public void OnUpdate()
        {
            if (_actor.actionComponent.hasState(ActStates.STATE_KEY_DIE))
            {
                return;
            }
            if (_actor.actionComponent.hasState(ActStates.STATE_KEY_NO_ATK))
            {
                return;
            }

            var dist = _traceTarget.transform.position - _actor.unityGameObject.transform.position;
            if(dist.magnitude < _attackRange)
            {
                _actor.actionComponent.AddAction(_attackAct);
            }
        }
    }
}
