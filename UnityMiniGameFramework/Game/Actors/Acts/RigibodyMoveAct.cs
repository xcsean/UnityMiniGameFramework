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
        public float AccSpeed = 1.0f;
        public float MaxSpeed = 2.0f;
        public float curSpeed => _curSpeed;

        protected float _curSpeed;
        protected UnityEngine.Vector3? _movVec;
        protected UnityEngine.Rigidbody _rigiBody;

        public RigibodyMoveAct(ActorObject actor) : base(actor)
        {
            _rigiBody = actor.unityGameObject.GetComponent<UnityEngine.Rigidbody>();

            actor.animatorComponent.playAnimation("Idle");
        }

        public bool isMoving => (_movVec != null);
        override public bool isFinished => false;
        override public bool discardWhenFinish => false;
        override public bool queueWhenNotStartable => true;

        public void moveToward(UnityEngine.Vector3 to)
        {
            _movVec = to;
        }
        public void stop()
        {
            _movVec = null;
        }

        override public void Start()
        {
            base.Start();
        }

        override public void Update(uint timeElasped)
        {
            if (_actor.actionComponent.hasState(ActStates.STATE_KEY_NO_MOVE))
            {
                // can't move
                _onStop();
                return;
            }

            _updateMove();

        }

        void _onStop()
        {
            if (actor.animatorComponent.currBaseAnimation.aniName == "Run")
            {
                actor.animatorComponent.playAnimation("Idle");
            }
        }

        void _updateMove()
        {
            if (_movVec == null)
            {
                _onStop();
                return;
            }

            if(actor.animatorComponent.currBaseAnimation.aniName != "Run")
            {
                actor.animatorComponent.playAnimation("Run");
            }

            _updateSpeed();

            //_rigiBody.velocity = _movVec.Value.normalized * _curSpeed;

            _rigiBody.MovePosition(_rigiBody.position + _movVec.Value.normalized * _curSpeed * UnityEngine.Time.deltaTime);
            //_rigiBody.transform.forward = _rigiBody.transform.forward + (_movVec.Value.normalized - _rigiBody.transform.forward).normalized * TurnSpeed * UnityEngine.Time.deltaTime;

            _rigiBody.transform.forward = _movVec.Value.normalized;

            //var yAxis = new UnityEngine.Vector3(0, 1, 0);
            //var forward = new UnityEngine.Vector3(_rigiBody.transform.forward.x, 0, _rigiBody.transform.forward.z);
            //var vec = new UnityEngine.Vector3(_movVec.Value.normalized.x, 0, _movVec.Value.normalized.z);
            //float deg = UnityEngine.Vector3.Dot(forward, vec);

            //float dist = (_movVec.Value.normalized - _rigiBody.transform.forward).magnitude;

            //if (dist > 0.05f)
            //{
            //    //_rigiBody.MoveRotation(UnityEngine.Quaternion.AngleAxis(TurnSpeed * UnityEngine.Time.deltaTime, yAxis));
            //    //_rigiBody.MoveRotation(UnityEngine.Quaternion.Euler(0, _rigiBody.transform.rotation.y + TurnSpeed * UnityEngine.Time.deltaTime, 0));
            //    _rigiBody.gameObject.transform.rotation = UnityEngine.Quaternion.Euler(_rigiBody.transform.rotation.x, _rigiBody.transform.rotation.y + TurnSpeed * UnityEngine.Time.deltaTime, _rigiBody.transform.rotation.y);
            //}
            //else if(dist < -0.05f)
            //{
            //    //_rigiBody.MoveRotation(UnityEngine.Quaternion.AngleAxis(-TurnSpeed * UnityEngine.Time.deltaTime, yAxis));
            //    //_rigiBody.MoveRotation(UnityEngine.Quaternion.Euler(0, _rigiBody.transform.rotation.y - TurnSpeed * UnityEngine.Time.deltaTime, 0));
            //    _rigiBody.gameObject.transform.rotation = UnityEngine.Quaternion.Euler(_rigiBody.transform.rotation.x, _rigiBody.transform.rotation.y - TurnSpeed * UnityEngine.Time.deltaTime, _rigiBody.transform.rotation.y);
            //}    
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
