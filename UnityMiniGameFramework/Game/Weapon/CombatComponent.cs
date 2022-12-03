using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    abstract public class CombatComponent : GameObjectComponent
    {
        virtual public void OnHitby(WeaponObject weapon)
        {

        }


    }
}
