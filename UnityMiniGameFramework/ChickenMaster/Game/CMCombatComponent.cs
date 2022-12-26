using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class CMCombatComponent : CombatComponent
    {
        override public string type => "CombatComponent";

        protected ActorObject _actor;
        protected RigibodyDieAct _dieAct;
        protected RigibodyOnHitAct _onHitAct;

        public override void Init(object config)
        {
            base.Init(config);


            _actor = this.gameObject as ActorObject;

            _dieAct = new RigibodyDieAct(_actor);
        }

        override protected void _onHitMissed(WeaponObject weapon)
        {
            // TO DO : show missing text
        }
        override protected void _onDamage(WeaponObject weapon, int dmg, bool critical)
        {
            // TO DO : show damage text

            // perform onhit act
            if (_gameObject is MGGameObject mgGameObject)
                BattleNumberEmitter.CreateNumAction.Invoke(mgGameObject.unityGameObject, dmg,
                    critical);
        }
        override protected void _onDie(WeaponObject weapon)
        {
            _actor.actionComponent.AddAction(_dieAct);
        }
    }
}
