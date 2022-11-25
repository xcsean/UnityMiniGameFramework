using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class WeaponManager
    {
        protected WeaponConfig _weaponConf;
        public WeaponConfig WeaponConf => _weaponConf;

        public void Init()
        {
            _weaponConf = (WeaponConfig)UnityGameApp.Inst.Conf.getConfig("weapons");
        }


    }
}
