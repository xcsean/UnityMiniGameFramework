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

        public void setHolder(ActorObject h)
        {
            _holder = h;
        }
    }
}
