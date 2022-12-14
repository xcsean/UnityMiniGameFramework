using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class RigibodyMoveAct : Act
    {
        public float TurnSpeed = 180.0f;
        public float TurnDecSpeed = 3.0f;
        public float AccSpeed = 1.0f;
        public float DecSpeed = 8.0f;
        public float MaxSpeed = 3.0f;
        public float MinSpeed = 1.0f;

        public float curSpeed => _curSpeed;

        protected float _curSpeed;
        protected UnityEngine.Vector3? _movVec;
        protected UnityEngine.Rigidbody _rigiBody;

        protected string _defaultAniName;
        
        public RigibodyMoveAct(ActorObject actor) : base(actor)
        {
            _rigiBody = actor.unityGameObject.GetComponent<UnityEngine.Rigidbody>();

            _defaultAniName = ActAnis.IdleAni;
            actor.animatorComponent.playAnimation(_defaultAniName);
        }

        bool _isTurning;
        public bool isTurning => _isTurning;

        bool _isStopping;
        public bool isStopping => _isStopping;
        public bool isMoving => (_movVec != null);
        override public bool isFinished => false;
        override public bool discardWhenFinish => false;
        override public bool queueWhenNotStartable => true;

        protected List<UnityEngine.Vector3> _movePath;
        protected float _movePathNodeRadius;
        protected int _curPathTargetIndex;
        protected UnityEngine.Vector3? _curTargetPos;

        public void moveToward(UnityEngine.Vector3 to)
        {
            _movVec = to.normalized;
        }
        public void moveOn(List<UnityEngine.Vector3> path, float nodeRadius)
        {
            if(path.Count <= 0)
            {
                return;
            }

            _movePath = path;
            _movePathNodeRadius = nodeRadius;
            _curPathTargetIndex = 0;
            _curTargetPos = _movePath[_curPathTargetIndex];

            _curSpeed = MinSpeed;
        }
        public void stop()
        {
            _movVec = null;
            _movePath = null;
            _curTargetPos = null;
            _isStopping = this._curSpeed > 0;
        }

        public void setDefaultAni(string _defAni)
        {
            _defaultAniName = _defAni;
            actor.animatorComponent.playAnimation(_defaultAniName);
        }

        override public void Start()
        {
            base.Start();
        }

        override public void Update(float timeElasped)
        {
            if (_actor.actionComponent.hasState(ActStates.STATE_KEY_NO_MOVE))
            {
                // can't move
                _onStop();
                return;
            }

            _updatePath();

            _updateMove();

            _updateStoping();

        }

        bool _pickNextPathNode()
        {
            ++_curPathTargetIndex;
            if (_curPathTargetIndex >= _movePath.Count)
            {
                return false;
            }

            _curTargetPos = _movePath[_curPathTargetIndex];
            return true;
        }

        void _updatePath()
        {
            if(_movePath == null)
            {
                return;
            }

            if(_curTargetPos == null)
            {
                return;
            }

            var vec = _curTargetPos - _rigiBody.transform.position;
            if (vec.Value.magnitude <= _movePathNodeRadius)
            {
                // reach current path node
                if(_pickNextPathNode())
                {
                    // recalc vec
                    vec = _curTargetPos - _rigiBody.transform.position;
                }
                else
                {
                    // reach path end
                    stop();
                    return;
                }
            }

            _movVec = vec.Value.normalized;
        }

        void _onStop()
        {
            if (actor.animatorComponent.isCurrBaseAnimation(ActAnis.RunAni))
            {
                actor.animatorComponent.playAnimation(_defaultAniName);
            }
        }

        void _updateStoping()
        {
            if(!_isStopping || _curSpeed <= 0)
            {
                return;
            }

            _decSpeed();

            _rigiBody.MovePosition(_rigiBody.position + _rigiBody.transform.forward * _curSpeed * UnityEngine.Time.deltaTime);
        }

        void _updateMove()
        {
            if (_movVec == null)
            {
                _onStop();
                return;
            }

            if(!actor.animatorComponent.isCurrBaseAnimation(ActAnis.RunAni))
            {
                actor.animatorComponent.playAnimation(ActAnis.RunAni);
            }

            var rot = UnityEngine.Quaternion.FromToRotation(_rigiBody.transform.forward, _movVec.Value);
            if(rot.eulerAngles.y < 1 && rot.eulerAngles.y > -1)
            {
                _isTurning = false;
                _updateSpeed();
            }
            else
            {
                // update turning
                _isTurning = true;
                _updateTurning(rot.eulerAngles.y);
            }

            _rigiBody.MovePosition(_rigiBody.position + _movVec.Value * _curSpeed * UnityEngine.Time.deltaTime); 
        }

        void _updateTurning(float yRot)
        {
            if(yRot > 180)
            {
                yRot -= 360;
            }

            float deg = TurnSpeed * UnityEngine.Time.deltaTime;

            if(yRot > 0)
            {
                if(deg < yRot)
                {
                    yRot = deg;
                }
            }
            else
            {
                if(deg < -yRot)
                {
                    yRot = -deg;
                }
            }

            // rotate
            _rigiBody.transform.Rotate(new UnityEngine.Vector3(0,yRot,0));

            // dec moving speed
            if (TurnDecSpeed > 0)
            {
                _curSpeed -= TurnDecSpeed * UnityEngine.Time.deltaTime;
                if (_curSpeed < MinSpeed)
                {
                    _curSpeed = MinSpeed;
                }
            }
        }

        void _decSpeed()
        {
            if (DecSpeed <= 0)
            {
                _curSpeed = 0;
                _isStopping = false;
            }
            else if (_curSpeed > 0)
            {
                _curSpeed -= DecSpeed * UnityEngine.Time.deltaTime;
                if (_curSpeed < 0)
                {
                    _curSpeed = 0;
                    _isStopping = false;
                }
            }
        }

        void _updateSpeed()
        {
            if (AccSpeed <= 0)
            {
                _curSpeed = MaxSpeed;
            }
            else if (_curSpeed < MaxSpeed)
            {
                _curSpeed += AccSpeed * UnityEngine.Time.deltaTime;
                if (_curSpeed > MaxSpeed)
                {
                    _curSpeed = MaxSpeed;
                }
            }
        }
    }
}
