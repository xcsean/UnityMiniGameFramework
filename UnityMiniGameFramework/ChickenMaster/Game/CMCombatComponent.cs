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

        protected int _HP;
        protected int _maxHP;


        override public void OnHitby(WeaponObject weapon)
        {

        }
    }
}
