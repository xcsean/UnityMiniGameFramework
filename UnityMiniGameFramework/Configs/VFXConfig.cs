using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{

    public class VFXConf
    {
        public string name { get; set; }
        public string type { get; set; }

        public string prefabName { get; set; }

        public uint? maxCacheCount { get; set; }
        public uint? maxShowCount { get; set; }

        public float? lifeTime { get; set; }

        public float? uvSpeed { get; set; }
    }

    public class VFXConfs
    {
        public Dictionary<string, VFXConf> vfxs { get; set; }
    }

    public class VFXConfig : JsonConfig
    {
        override public string type => "VFXConfig";
        public static VFXConfig create()
        {
            return new VFXConfig();
        }

        public VFXConfs vfxConfs => (VFXConfs)_conf;

        override protected object _JsonDeserialize(string confStr)
        {
            return JsonSerializer.Deserialize<VFXConfs>(confStr);
        }

        public VFXConf getVFXConfig(string vfxName)
        {
            if (vfxConfs.vfxs == null || !vfxConfs.vfxs.ContainsKey(vfxName))
            {
                return null;
            }
            return vfxConfs.vfxs[vfxName];
        }
    }
}
