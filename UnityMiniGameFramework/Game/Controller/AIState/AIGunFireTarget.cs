using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class AIGunFireTarget : AIState
    {
        protected GunObject _gunObj;
        protected Map _map;
        protected MapMonsterObject _currentTargetMon;

        public AIGunFireTarget(ActorObject actor) : base(actor)
        {
            _map = UnityGameApp.Inst.MainScene.map as Map;
        }

        public void setGun(GunObject g)
        {
            _gunObj = g;
        }

        public void clearTarget()
        {
            _currentTargetMon = null;
        }

        virtual protected bool _checkMonsterAttackable(MapMonsterObject m)
        {
            if (m.actionComponent.hasState(ActStates.STATE_KEY_DIE))
            {
                // monster already die
                return false;
            }

            // TO DO : check other conditions

            return true;
        }
        protected bool _isMonInAttackRange(MapMonsterObject m)
        {
            UnityEngine.Vector3 distVec = m.unityGameObject.transform.position - _actor.unityGameObject.transform.position;
            if (distVec.magnitude <= _gunObj.attackRange)
            {
                return true;
            }

            return false;
        }

        virtual protected void _seekTargetMonster()
        {
            // TO DO : seprate & sort monsters in map when it's too many
            foreach (var spawn in _map.monsterSpawns.Values)
            {
                foreach (var m in spawn.monsters)
                {
                    if (!_checkMonsterAttackable(m))
                    {
                        // not attackalbe
                        continue;
                    }

                    UnityEngine.Vector3 distVec = m.unityGameObject.transform.position - _actor.unityGameObject.transform.position;
                    if(distVec.magnitude <= _gunObj.attackRange)
                    {
                        _currentTargetMon = m;
                        return;
                    }
                }
            }
        }


        public override void OnUpdate()
        {
            if (_actor.actionComponent.hasState(ActStates.STATE_KEY_DIE))
            {
                return;
            }
            if (_actor.actionComponent.hasState(ActStates.STATE_KEY_NO_ATK))
            {
                return;
            }

            if (_currentTargetMon == null)
            {
                // try seek monster
                _seekTargetMonster();
            }
            else if (_currentTargetMon.unityGameObject == null)
            {
                _currentTargetMon = null;
            }
            else if(_currentTargetMon.actionComponent.hasState(ActStates.STATE_KEY_DIE))
            {
                _currentTargetMon = null;

                // try seek monster
                _seekTargetMonster();
            }
            else if(!_isMonInAttackRange(_currentTargetMon))
            {
                _currentTargetMon = null;

                // try seek monster
                _seekTargetMonster();
            }


            if (_currentTargetMon != null)
            {

                if(!_actor.animatorComponent.isCurrBaseAnimation(ActAnis.FireAni))
                {
                    _actor.animatorComponent.playAnimation(ActAnis.FireAni);
                }

                _gunObj.Fire(_currentTargetMon);
                _actor.unityGameObject.transform.forward = (_currentTargetMon.unityGameObject.transform.position - _actor.unityGameObject.transform.position).normalized;
            }
            else
            {
                if (!_actor.animatorComponent.isCurrBaseAnimation(ActAnis.IdleAni))
                {
                    _actor.animatorComponent.playAnimation(ActAnis.IdleAni);
                }

                _gunObj.StopFire();
            }
        }
    }
}
