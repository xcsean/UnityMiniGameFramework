using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace UnityMiniGameFramework
{
    public class MoveAct : Act
    {
        public float MaxSpeed = 1.0f;
        public float AccSpeed = 0.0f;

        protected float _curSpeed;

        protected Vector3? _curTargetPos;
        protected Queue<Vector3> _movePath;

        protected IHeightMap _heightMap;

        public bool isMoving => (_curTargetPos != null);
        public float curSpeed => _curSpeed;

        public MoveAct(ActorObject actor) : base(actor)
        {

        }

        public void moveTo(Vector3 pos)
        {
            _movePath?.Clear();
            _curTargetPos = pos;
        }
        public void moveOn(Queue<Vector3> path)
        {
            _movePath = path;
        }
        public void setHeightMap(IHeightMap hm)
        {
            _heightMap = hm;
        }

        override public bool isFinished => false;
        override public bool discardWhenFinish => false;
        override public bool queueWhenNotStartable => true;

        override public bool checkStartCondition()
        {
            return true;
        }

        override public void Start()
        {
            base.Start();
        }

        override public void Update(uint timeElasped)
        {
            if(_actor.actionComponent.hasState(ActStates.STATE_KEY_NO_MOVE))
            {
                // can't move
                return;
            }

            _pickNextPos();

            _updateMove();

        }

        void _pickNextPos()
        {
            if (_curTargetPos == null)
            {
                if (_movePath != null && _movePath.Count > 0)
                {
                    _curTargetPos = _movePath.Dequeue();
                }
            }
        }

        void _updateMove()
        {
            if (_curTargetPos == null)
            {
                return;
            }

            _updateSpeed();

            Vector3 deltaPos = _curTargetPos.Value - _actor.unityGameObject.transform.position;
            if (_heightMap != null)
            {
                deltaPos.y = 0;
            }

            Vector3 movVec;
            float movLen = _curSpeed * Time.deltaTime;
            if (movLen < deltaPos.magnitude)
            {
                movVec = deltaPos.normalized * movLen;
            }
            else
            {
                movVec = deltaPos;

                _curTargetPos = null; // reach target pos, stop
            }
            _actor.unityGameObject.transform.position = _actor.unityGameObject.transform.position + movVec;
            //this.transform.Translate(movVec);

            //MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Debug, $"{Time.deltaTime} {movVec.magnitude}");

            if (_heightMap != null)
            {
                float y = _heightMap.getHeightByVector(_actor.unityGameObject.transform.position);
                _actor.unityGameObject.transform.position = new Vector3(_actor.unityGameObject.transform.position.x, y, _actor.unityGameObject.transform.position.z);
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
                _curSpeed += AccSpeed * Time.deltaTime;
                if (_curSpeed > MaxSpeed)
                {
                    _curSpeed = MaxSpeed;
                }
            }
        }
    }
}
