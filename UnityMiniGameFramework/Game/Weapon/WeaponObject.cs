using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class WeaponAttack
    {
        public int attackMin { get; set; }
        public int attackMax { get; set; }
        public int missingRate { get; set; }
        public float criticalHitRate { get; set; }
        public float criticalHitPer { get; set; }
    }

    abstract public class WeaponObject : MGGameObject
    {
        protected ActorObject _holder;
        public ActorObject holder => _holder;

        protected WeaponAttack _attackInfo;
        public WeaponAttack attackInfo => _attackInfo;


        protected List<ActBufConfig> _actBuffs;
        protected float _actbufMul;
        public float ActbufMul => _actbufMul;
        public List<ActBufConfig> ActBuffs => _actBuffs;

        public void initAttack(AttackConf conf)
        {
            _attackInfo = new WeaponAttack()
            {
                attackMin = conf.attackMin,
                attackMax = conf.attackMax,
                missingRate = conf.missingRate.HasValue ? conf.missingRate.Value : 0,
                criticalHitRate = conf.criticalHitRate.HasValue ? conf.criticalHitRate.Value : 0,
                criticalHitPer = conf.criticalHitPer.HasValue ? conf.criticalHitPer.Value : 1
            };
        }

        public void initActBuf(List<string> buffsList, CMGunLevelConf gunLevelConf)
        {
            if (_actBuffs == null)
                _actBuffs = new List<ActBufConfig>();
            _actBuffs.Clear();
            if (buffsList == null)
                return;
            List<string> attrs = new List<string>();
            foreach (var buffName in buffsList)
            {
                ActBufConfig actBuff = UnityGameApp.Inst.BuffDataMgr.BuffConfig.GetBuffConfig(buffName);
                if (!actBuff.isVaild())
                    continue;
                actBuff.bufAttrs.Clear();
                if (gunLevelConf.buffAttrs == null)
                {
                    _actBuffs.Add(actBuff);
                    continue;
                }
                if (gunLevelConf.buffAttrs.TryGetValue(buffName + "Attrs", out attrs))
                    foreach (var attrName in attrs)
                    {
                        var config = UnityGameApp.Inst.BuffDataMgr.BuffConfig.GetBuffAttrConfig(attrName);
                        if (!config.isVaild())
                            continue;
                        actBuff.bufAttrs.Add(config);
                        if (config.type == "dot")
                        {
                            if (actBuff.dot == null)
                                actBuff.dot = new ActBufDotConfig();
                            actBuff.dot.time = config.time;
                            actBuff.dot.config = config;
                        }
                    }
                else
                    Debug.DebugOutput(DebugTraceType.DTT_Error, $"{buffName} not exit in gunLevelConf BuffAttrs");

                _actBuffs.Add(actBuff);
            }

            _actbufMul = gunLevelConf.actbufMul;
        }

        public void addAttackInfo(WeaponAttack atkAdd)
        {
            _attackInfo.attackMin += atkAdd.attackMin;
            _attackInfo.attackMax += atkAdd.attackMax;
            _attackInfo.missingRate += atkAdd.missingRate;
            _attackInfo.criticalHitRate += atkAdd.criticalHitRate;
            _attackInfo.criticalHitPer += atkAdd.criticalHitPer;
        }

        public void onRecalcAttributes(ActBufAttrConfig[] bufAttrs, float extraAtkMul = 0)
        {
            float attackAdd = 0;
            float attackMul = extraAtkMul;
            float missingRateAdd = 0;
            float criticalHitRateAdd = 0;
            float criticalHitPerAdd = 0;

            foreach (var bufAttr in bufAttrs)
            {
                if (bufAttr.name == BuffAttrNameDefine.ATK)
                {
                    attackAdd += bufAttr.addValue;
                    attackMul += bufAttr.mulValue;
                }
                else if (bufAttr.name == BuffAttrNameDefine.MISS)
                {
                    missingRateAdd += bufAttr.addValue;
                }
                else if (bufAttr.name == BuffAttrNameDefine.CRIT_RATE)
                {
                    criticalHitRateAdd += bufAttr.addValue;
                }
                else if (bufAttr.name == BuffAttrNameDefine.CRIT_PER)
                {
                    criticalHitPerAdd += bufAttr.addValue;
                }
            }

            if (_actBuffs != null)
            {
                foreach (var buffConfig in _actBuffs)
                {
                    if (!buffConfig.isVaild())
                        continue;
                    if (buffConfig.bufAttrs == null)
                        continue;
                    foreach (var buffAttr in buffConfig.bufAttrs)
                    {
                        if (buffAttr.name == BuffAttrNameDefine.ATK)
                        {
                            attackAdd += buffAttr.addValue;
                            attackMul += buffAttr.mulValue;
                        }
                        else if (buffAttr.name == BuffAttrNameDefine.MISS)
                        {
                            missingRateAdd += buffAttr.addValue;
                        }
                        else if (buffAttr.name == BuffAttrNameDefine.CRIT_RATE)
                        {
                            criticalHitRateAdd += buffAttr.addValue;
                        }
                        else if (buffAttr.name == BuffAttrNameDefine.CRIT_PER)
                        {
                            criticalHitPerAdd += buffAttr.addValue;
                        }
                    }
                }
            }

            _attackInfo.attackMin = (int) (_attackInfo.attackMin * (1 + attackMul) + attackAdd);
            _attackInfo.attackMax = (int) (_attackInfo.attackMax * (1 + attackMul) + attackAdd);
            _attackInfo.missingRate = (int) (_attackInfo.missingRate + missingRateAdd);
            _attackInfo.criticalHitRate = _attackInfo.criticalHitRate + criticalHitRateAdd;
            _attackInfo.criticalHitPer = _attackInfo.criticalHitPer + criticalHitPerAdd;
        }

        public void setHolder(ActorObject h)
        {
            _holder = h;
        }
    }
}