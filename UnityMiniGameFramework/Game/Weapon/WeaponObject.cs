using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class WeaponAttack
    {
        public int attackMin { get; set; }
        public int attackMax { get; set; }
        public int missingRate { get; set; }
        public int criticalHitRate { get; set; }
        public float criticalHitPer { get; set; }

    }

    abstract public class WeaponObject : MGGameObject
    {
        protected ActorObject _holder;
        public ActorObject holder => _holder;

        protected WeaponAttack _attackInfo;
        public WeaponAttack attackInfo => _attackInfo;

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
                if (bufAttr.name == "ATK")
                {
                    attackAdd += bufAttr.addValue;
                    attackMul += bufAttr.mulValue;
                }
                else if (bufAttr.name == "MISS")
                {
                    missingRateAdd += bufAttr.addValue;
                }
                else if (bufAttr.name == "CRIT_RATE")
                {
                    criticalHitRateAdd += bufAttr.addValue;
                }
                else if (bufAttr.name == "CRIT_PER")
                {
                    criticalHitPerAdd += bufAttr.addValue;
                }
            }

            _attackInfo.attackMin = (int)(_attackInfo.attackMin * (1 + attackMul) + attackAdd);
            _attackInfo.attackMax = (int)(_attackInfo.attackMax * (1 + attackMul) + attackAdd);
            _attackInfo.missingRate = (int)(_attackInfo.missingRate + missingRateAdd);
            _attackInfo.criticalHitRate = (int)(_attackInfo.criticalHitRate + criticalHitRateAdd);
            _attackInfo.criticalHitPer = (int)(_attackInfo.criticalHitPer + criticalHitPerAdd);
        }

        public void setHolder(ActorObject h)
        {
            _holder = h;
        }
    }
}
