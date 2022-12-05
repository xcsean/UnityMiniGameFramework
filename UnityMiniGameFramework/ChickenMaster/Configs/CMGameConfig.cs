using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{

    public class CMGunConf
    {
        public int id { get; set; }
        public string prefabName { get; set; }

        public AttackConf attack { get; set; }
    }

    public class CMGameConf
    {
        public Dictionary<int, CMGunConf> gunConfs { get; set; }

        public CombatConf selfCombatConf { get; set; }
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
