using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class AniVanishAct : Act
    {
        protected bool _isFinished;
        override public bool isFinished => _isFinished;
        override public bool discardWhenFinish => true;
        override public bool queueWhenNotStartable => false;

        protected string _vanishAni;

        public AniVanishAct(ActorObject actor) : base(actor)
        {
        }

        public void setVanishAnimaionName(string aniName)
        {
            _vanishAni = aniName;
        }

        override public bool checkStartCondition()
        {
            // TO DO : check is ready for vanish act
            return true;
        }

        override public void Start()
        {
            base.Start();

            if (!_actor.animatorComponent.isCurrBaseAnimation(_vanishAni))
            {
                actor.animatorComponent.playAnimation(_vanishAni);
                actor.actionComponent.setState(ActStates.STATE_KEY_NO_MOVE, 1);
                actor.actionComponent.setState(ActStates.STATE_KEY_DIE, 1);
            }
            _isFinished = false;
        }

        override public void Update(float timeElasped)
        {
            bool aniFnished = false;
            if (!_actor.animatorComponent.isCurrBaseAnimation(_vanishAni))
            {
                aniFnished = true;
            }
            else if (_actor.animatorComponent.unityAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
            {
                aniFnished = true;
            }

            if (aniFnished)
            {
                _isFinished = true;
                _actor.markNeedDestroy();
            }
        }
    }
}
