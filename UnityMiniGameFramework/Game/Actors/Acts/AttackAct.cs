using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class AttackAct : Act
    {
        protected bool _isFinished;
        override public bool isFinished => _isFinished;
        override public bool discardWhenFinish => true;
        override public bool queueWhenNotStartable => false;

        public AttackAct(ActorObject actor) : base(actor)
        {

        }
        override public bool checkStartCondition()
        {
            // TO DO : check is ready for attack act
            return true;
        }

        override public void Start()
        {
            base.Start();

            if (!actor.animatorComponent.isCurrBaseAnimation(ActAnis.AttackAni))
            {
                actor.animatorComponent.playAnimation(ActAnis.AttackAni);
                actor.actionComponent.setState(ActStates.STATE_KEY_NO_MOVE, 1);
            }
            _isFinished = false;
        }

        override public void Update(float timeElasped)
        {
            if (!actor.animatorComponent.isCurrBaseAnimation(ActAnis.AttackAni))
            {
                _isFinished = true;
            }
            else if (actor.animatorComponent.unityAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
            {
                _isFinished = true;
            }

            if(_isFinished)
            {
                if (!actor.animatorComponent.isCurrBaseAnimation(ActAnis.IdleAni))
                {
                    actor.animatorComponent.playAnimation(ActAnis.IdleAni);
                    actor.actionComponent.unsetState(ActStates.STATE_KEY_NO_MOVE);
                }
            }
        }

    }
}
