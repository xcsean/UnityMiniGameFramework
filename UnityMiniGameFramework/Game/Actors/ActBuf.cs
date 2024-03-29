﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    class BuffAttrNameDefine
    {
        public const string MOVE_SPEED = "MOVE_SPEED";
        public const string HP = "HP";
        public const string DEF = "DEF";
        public const string ATK = "ATK";
        public const string MISS = "MISS";
        public const string CRIT_RATE = "CRIT_RATE";            // 暴击率
        public const string CRIT_PER = "CRIT_PER";              // x倍暴击
        public const string FIXED_DAMAGE = "FIXED_DAMAGE";      // 无视护甲-固定伤害
        public const string PER_DAMAGE = "PER_DAMAGE";          // 无视护甲-百分比伤害
        public const string TRIGGER_ADD_BUFF = "TRIGGER_ADD_BUFF";// 触发添加此buff
        public const string DOT_DAMAGE = "DOT_DAMAGE";   // dot 伤害
        public const string ATTACK_DAMAGE = "ATTACK_DAMAGE";      // 普通伤害 计算护甲

    }

    public class ActBuf
    {
        protected ActorObject _actor;
        public ActorObject actor => _actor;

        protected ActorObject _fromActor;
        public ActorObject fromActor => _fromActor;

        protected float _timeLeft;
        public float timeLeft => _timeLeft;

        protected ActBufConfig _conf;

        protected float _dotTimeLeft;

        public ActBuf(ActorObject actor, ActorObject fromActor)
        {
            _actor = actor;
            _fromActor = fromActor;
        }

        public UInt64 getBuffId()
        {
            return _conf.buffUID;
        }

        public void Init(ActBufConfig conf, float time = 30)
        {
            _conf = conf;
            _timeLeft = time;

            if (_conf.dot != null)
            {
                _dotTimeLeft = _conf.dot.time;
            }
        }

        protected virtual bool _checkTimeFinish()
        {
            _timeLeft -= UnityEngine.Time.deltaTime;
            if (_timeLeft > 0)
            {
                return false;
            }

            _timeLeft = 0;

            _actor.actionComponent.OnBufRemove(this);

            return true;
        }

        protected void _onAddBuffIcon()
        {
            var combatComp = _actor.getComponent("CombatComponent") as CombatComponent;
            combatComp?.AddBuffIcon(_conf.buffIcon);
        }

        protected void _onSubBuffIcon()
        {
            var combatComp = _actor.getComponent("CombatComponent") as CombatComponent;
            combatComp?.SubBuffIcon(_conf.buffIcon);
        }

        protected void _onAddState()
        {
            if (_conf.bufAddStates == null)
            {
                return;
            }

            foreach (var stKey in _conf.bufAddStates)
            {
                _actor.actionComponent.addState(stKey);
            }
        }

        protected void _onAddAttr()
        {
            if (_conf.bufAttrs == null)
            {
                return;
            }

            var combatComp = _actor.getComponent("CombatComponent") as CombatComponent;

            if (combatComp == null)
            {
                return;
            }

            foreach (var bufAttr in _conf.bufAttrs)
            {
                combatComp.AddBufAttr(bufAttr);
            }

            combatComp.RecalcAttributes();
        }

        public virtual void OnAdd()
        {
            _onAddBuffIcon();
            _onAddState();

            _onAddAttr();
        }


        protected void _onRemoveState()
        {
            if (_conf.bufAddStates == null)
            {
                return;
            }

            foreach (var stKey in _conf.bufAddStates)
            {
                _actor.actionComponent.subState(stKey);
            }
        }

        protected void _onRemoveAttr()
        {
            if (_conf.bufAttrs == null)
            {
                return;
            }

            var combatComp = _actor.getComponent("CombatComponent") as CombatComponent;

            if (combatComp == null)
            {
                return;
            }

            foreach (var bufAttr in _conf.bufAttrs)
            {
                combatComp.RemoveBufAttr(bufAttr);
            }

            combatComp.RecalcAttributes();
        }

        public virtual void OnRemove()
        {
            _onSubBuffIcon();
            _onRemoveState();

            _onRemoveAttr();
        }

        protected void _onDotUpdate()
        {
            if (_conf.dot == null)
            {
                return;
            }

            var combatComp = _actor.getComponent("CombatComponent") as CombatComponent;

            if (combatComp == null)
            {
                return;
            }

            _dotTimeLeft -= UnityEngine.Time.deltaTime;
            if (_dotTimeLeft <= 0)
            {
                _dotTimeLeft += _conf.dot.time;
                UInt64 dmg = combatComp.OnDamageCalByConf(new List<ActBufAttrConfig>() {_conf.dot.config});
                combatComp.OnDamageBy(_fromActor, dmg, DamageTypeEnum.Dot);
            }
        }

        public virtual void OnUpdate()
        {
            _onDotUpdate();
        }

        public virtual void OnPostUpdate()
        {
            _checkTimeFinish();
        }
    }
}