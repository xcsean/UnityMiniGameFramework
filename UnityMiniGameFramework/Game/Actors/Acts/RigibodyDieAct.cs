using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class RigibodyDieAct : Act
    {
        protected bool _isFinished;
        override public bool isFinished => _isFinished;
        override public bool discardWhenFinish => true;
        override public bool queueWhenNotStartable => false;

        protected float _dieTime;
        protected bool _aniFinished;

        public RigibodyDieAct(ActorObject actor) : base(actor)
        {
            _dieTime = 0.5f; // for Debug ...
        }
        override public bool checkStartCondition()
        {
            // TO DO : check is ready for attack act
            return true;
        }

        override public void Start()
        {
            base.Start();

            if (!_actor.animatorComponent.isCurrBaseAnimation(ActAnis.DieAni))
            {
                actor.animatorComponent.playAnimation(ActAnis.DieAni);
                actor.actionComponent.setState(ActStates.STATE_KEY_NO_MOVE, 1);
                actor.actionComponent.setState(ActStates.STATE_KEY_DIE, 1);
            }
            _isFinished = false;
        }

        override public void Update(float timeElasped)
        {
            if (!_actor.animatorComponent.isCurrBaseAnimation(ActAnis.DieAni))
            {
                _aniFinished = true;
            }
            else if (_actor.animatorComponent.getAnimatorStateInfoNormalizedTime() >= 0.99f)
            {
                _aniFinished = true;
            }

            if (_aniFinished)
            {
                _dieTime -= UnityEngine.Time.deltaTime;
                if(_dieTime <= 0)
                {
                    // remove colliders
                    var collider = _actor.unityGameObject.GetComponent<UnityEngine.Collider>();
                    if (collider != null)
                    {
                        UnityEngine.GameObject.Destroy(collider);
                    }

                    // remove rigibody
                    var rigiBody = _actor.unityGameObject.GetComponent<UnityEngine.Rigidbody>();
                    if (rigiBody != null)
                    {
                        UnityEngine.GameObject.Destroy(rigiBody);
                    }

                    _isFinished = true;
                    _actor.markNeedDestroy();
                }
            }
        }
    }
}
