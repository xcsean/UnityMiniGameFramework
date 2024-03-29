﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Text.Json;
using System.Threading.Tasks;
using MiniGameFramework;
using UnityEngine;

namespace UnityMiniGameFramework
{
    public class ExplosiveConf
    {
        public string explosiveVFX { get; set; }
        public string hitVFX { get; set; }

        public float? startTime { get; set; }
        public float? keepTime { get; set; }

        public float? hitForce { get; set; }
        public float? blastRange { get; set; }          // 利用特效缩放来控制爆炸范围
    }

    public class GunFireConf
    {
        public string shootVFX { get; set; }
        public string fireType { get; set; } // "projectile","ray","emmiter"

        public string bulletVFX { get; set; }

        public string hitVFX { get; set; }

        public string rayImpactVFX { get; set; } // only in fireType=ray case

        public float fireCdTime { get; set; }

        public float? attackRange { get; set; }

        public float? projectileFlySpeed { get; set; } // only in fireType=projectile case

        public string fireAudio { get; set; }
        public string hitAudio { get; set; }

        public float? hitForce { get; set; }

        public int? Multiple { get; set; }
        
        public int? bulletCount { get; set; }   // 加特林模式
        public int? pierceCount { get; set; }   // 穿透数量

        public float? shootOffsetAngleBegin { get; set; }

        public float? shootOffsetAngleEnd { get; set; }
        public float? baseattackspeedrate { get; set; }

        public ExplosiveConf collideExplosive { get; set; }

        public string collideBuf { get; set; } // give dot or other buf to target
    }

    public class GunConf
    {
        public string name { get; set; }
        public string type { get; set; }

        public string attachToBone { get; set; }

        public JsonConfVector3 attachPos { get; set; }
        public JsonConfVector3 attachRot { get; set; }

        public AnimatorComponentConfig AnimatorConf { get; set; }

        public GunFireConf FireConf { get; set; }
    }

    [Serializable]
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
            //return JsonSerializer.Deserialize<WeaponConfs>(confStr);
            return JsonUtil.FromJson<WeaponConfs>(confStr);
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
