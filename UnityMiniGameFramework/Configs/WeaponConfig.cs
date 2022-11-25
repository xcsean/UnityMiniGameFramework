using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class GunFireConf
    {
        public string shootVFX { get; set; }
        public string fireType { get; set; } // "projectile","ray","emmiter"

        public string bulletVFX { get; set; }

        public string hitVFX { get; set; }

        public string rayImpactVFX { get; set; } // only in fireType=ray case

        public float fireCdTime { get; set; }

        public float? projectileFlySpeed { get; set; } // only in fireType=projectile case

        public float? maxRayLength { get; set; } // only in fireType=ray case

        public string collideExplosiveEffect { get; set; }

        public string collideBuf { get; set; } // give dot or other buf to target
    }

    public class GunConf
    {
        public string name { get; set; }
        public string type { get; set; }

        public AnimatorComponentConfig AnimatorConf { get; set; }

        public GunFireConf FireConf { get; set; }
    }

    public class WeaponConfs
    {
        public Dictionary<string, GunConf> guns { get; set; }
    }

    public class WeaponConfig : JsonConfig
    {
        override public string type => "WeaponConfig";
        public static WeaponConfig create()
        {
            return new WeaponConfig();
        }

        public WeaponConfs weaponConfs => (WeaponConfs)_conf;

        override protected object _JsonDeserialize(string confStr)
        {
            return JsonSerializer.Deserialize<WeaponConfs>(confStr);
        }

        public GunConf getGunConf(string gunName)
        {
            if (weaponConfs.guns == null || !weaponConfs.guns.ContainsKey(gunName))
            {
                return null;
            }
            return weaponConfs.guns[gunName];
        }

    }
}
