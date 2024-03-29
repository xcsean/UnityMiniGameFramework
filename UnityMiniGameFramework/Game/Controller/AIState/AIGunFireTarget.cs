﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector3 = UnityEngine.Vector3;

namespace UnityMiniGameFramework
{
    public class AIGunFireTarget : AIState
    {
        protected GunObject _gunObj;
        protected Map _map;
        protected MapMonsterObject _currentTargetMon;
        protected bool _levelFinish = false;

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
            _levelFinish = true;
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
                _gunObj?.StopFire();
                _actor.animatorComponent.playAnimation(ActAnis.IdleAni);
                return;
            }

            if ((UnityGameApp.Inst.Game as ChickenMasterGame).uiGMPanel() != null &&
                (UnityGameApp.Inst.Game as ChickenMasterGame).uiGMPanel().IsDisableHero(_actor.name)
            )
            {
                // GM: No shooting
                _gunObj?.StopFire();
                _actor.animatorComponent.playAnimation(ActAnis.IdleAni);
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
                    if (_gunObj.conf.FireConf.fireType == "ray" || _gunObj.conf.FireConf.fireType == "emmiter")
                        _actor.animatorComponent.playAnimation(ActAnis.HoldGunAni);
                    else
                        _actor.animatorComponent.playAnimation(ActAnis.FireAni);
                }
                
                // 枪口与人物的夹角
                Vector3 actorForward = _actor.unityGameObject.transform.forward;
                actorForward.y = 0;
                Vector3 gunForward = _gunObj.GunPos.transform.forward;
                gunForward.y = 0;

                float angle = UnityEngine.Vector3.Angle(actorForward, gunForward);

                var forward = (_currentTargetMon.unityGameObject.transform.position -
                               _actor.unityGameObject.transform.position).normalized;
                //旋转
                forward = UnityEngine.Quaternion.AngleAxis(angle, UnityEngine.Vector3.up) * forward;
                _actor.unityGameObject.transform.forward = forward;
                _gunObj.Fire(_currentTargetMon);
            }
            else if (_levelFinish)
            {
                _gunObj.ResetFire();
                _levelFinish = false;
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
