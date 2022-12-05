using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class AITrace : AIState
    {
        protected UnityEngine.GameObject _traceTarget;
        protected RigibodyMoveAct _movAct;

        public AITrace(ActorObject actor) : base(actor)
        {
            _movAct = (actor as MapRoleObject).moveAct;
        }

        public void setTraceTarget(UnityEngine.GameObject unityGameObj)
        {
            _traceTarget = unityGameObj;
        }

        override public void OnUpdate()
        {
            if(_actor.actionComponent.hasState(ActStates.STATE_KEY_DIE))
            {
                return;
            }

            var vec = _traceTarget.transform.position - _actor.unityGameObject.transform.position;
            if (vec.magnitude > 1.0f)
            {
                _movAct.moveToward(vec);
            }
            else
            {
                _movAct.stop();
            }
        }
    }
}
