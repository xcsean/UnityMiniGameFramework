using System;
using System.Collections.Generic;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class ActBufDotConfig
    {
        public float time { get; set; }
        public int damage { get; set; }
    }

    public class ActBufAttrConfig
    {
        public string name { get; set; }
        public float addValue { get; set; }
        public float mulValue { get; set; }
    }

    public class ActBufConfig
    {
        public string bufName { get; set; }

        public string bufVFXName { get; set; }

        public float endTime { get; set; }

        public List<int> bufAddStates { get; set; }

        public List<ActBufAttrConfig> bufAttrs { get; set; }

        public ActBufDotConfig dot { get; set; }
    }

    [Serializable]
    public class BuffConfigs
    {
        public Dictionary<string, ActBufConfig> buffs { get; set; }
        public Dictionary<string, ActBufAttrConfig> buffAttrs { get; set; }
    }

    public class BuffConfig : JsonConfig
    {
        public override string type => "BuffConfig";

        public static BuffConfig create()
        {
            return new BuffConfig();
        }

        public BuffConfigs buffConfig => (BuffConfigs) _conf;

        protected override object _JsonDeserialize(string confStr)
        {
            return JsonUtil.FromJson<BuffConfigs>(confStr);
        }

        public ActBufConfig GetBuffConfig(string buffName)
        {
            if (buffConfig.buffs == null || !buffConfig.buffs.ContainsKey(buffName))
            {
                return null;
            }
            return buffConfig.buffs[buffName];
        }

        public ActBufAttrConfig GetBuffAttrConfig(string attrName)
        {
            if (buffConfig.buffAttrs == null || !buffConfig.buffAttrs.ContainsKey(attrName))
                return null;
            
            return buffConfig.buffAttrs[attrName];
        }
    }
}