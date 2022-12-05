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
        public int criticalHitPer { get; set; }

    }

    abstract public class WeaponObject : MGGameObject
    {
        protected ActorObject _holder;
        public ActorObject holder => _holder;

        protected WeaponAttack _attackInfo;
        public WeaponAttack attackInfo => _attackInfo;

        public void initAttack(AttackConf conf, int level)
        {
            _attackInfo = new WeaponAttack()
            {
                attackMin = conf.attackMin + conf.attackMinAddPerLevel * level,
                attackMax = conf.attackMax + conf.attackMaxAddPerLevel * level,
                missingRate = conf.missingRate.HasValue ? conf.missingRate.Value : 0,
                criticalHitRate = conf.criticalHitRate.HasValue ? conf.criticalHitRate.Value : 0,
                criticalHitPer = conf.criticalHitPer.HasValue ? conf.criticalHitPer.Value : 1
            };
        }

        public void setHolder(ActorObject h)
        {
            _holder = h;
        }
    }
}
