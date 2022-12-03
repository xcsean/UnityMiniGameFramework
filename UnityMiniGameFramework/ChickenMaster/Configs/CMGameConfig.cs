using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class AttackDefConf
    {
        public int attackMin { get; set; }
        public int attackMax { get; set; }
        public int attackMinAddPerLevel { get; set; }
        public int attackMaxAddPerLevel { get; set; }

        public int? missingRate { get; set; }
        public int? criticalHitRate { get; set; }
        public int? criticalHitPer { get; set; }
    }

    public class CMGunConf
    {
        public int id { get; set; }
        public string prefabName { get; set; }

        AttackDefConf attack { get; set; }
    }

    public class CMGameConf
    {
        public Dictionary<int, CMGunConf> gunConfs { get; set; }
    }

    public class CMGameConfig : JsonConfig
    {
        override public string type => "CMGameConfig";
        public static CMGameConfig create()
        {
            return new CMGameConfig();
        }
        
        public CMGameConf gameConfs => (CMGameConf)_conf;

        override protected object _JsonDeserialize(string confStr)
        {
            return JsonSerializer.Deserialize<CMGameConf>(confStr);
        }

        public CMGunConf getCMGunConf(int cmGunID)
        {
            if (gameConfs.gunConfs == null || !gameConfs.gunConfs.ContainsKey(cmGunID))
            {
                return null;
            }
            return gameConfs.gunConfs[cmGunID];
        }
    }
}
