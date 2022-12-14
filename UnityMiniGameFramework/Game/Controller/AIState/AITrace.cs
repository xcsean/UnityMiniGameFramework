using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class AITrace : AIState
    {
        public static AITrace create(ActorObject actor)
        {
            return new AITrace(actor);
        }

        protected UnityEngine.GameObject _traceTarget;
        protected RigibodyMoveAct _movAct;

        public AITrace(ActorObject actor) : base(actor)
        {
            _movAct = (actor as MapRoleObject).moveAct;
        }

        public override void Init(MapConfAIState conf)
        {
            base.Init(conf);

            if(conf.targetName == null)
            {
                // trace self
                // TO DO : remove cmGame, use UnityGameApp.Inst.Game
                var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                setTraceTarget(cmGame.Self.selfMapHero.unityGameObject);
            }
            else
            {
                // TO DO: trace target name object
            }
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
