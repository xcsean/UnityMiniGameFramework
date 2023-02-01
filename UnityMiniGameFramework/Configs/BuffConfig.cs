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

    public struct ActBufAttrConfig
    {
        public string name { get; set; }
        public float addValue { get; set; }
        public float mulValue { get; set; }
        
        public float probability { get; set; }
        public bool isVaild()
        {
            return !string.IsNullOrEmpty(name);
        }
    }

    public struct ActBufConfig
    {
        public string bufName { get; set; }

        public string bufVFXName { get; set; }

        public float endTime { get; set; }

        public List<int> bufAddStates { get; set; }

        public List<ActBufAttrConfig> bufAttrs { get; set; }

        public ActBufDotConfig dot { get; set; }

        public bool isVaild()
        {
            return !string.IsNullOrEmpty(bufName);
        }

        public bool CheckAddBuff()
        {
            if (bufAttrs == null)
                return true;
            foreach (var attr in bufAttrs)
            {
                if (attr.name == BuffAttrNameDefine.TRIGGER_ADD_BUFF)
                    return UnityGameApp.Inst.Rand.IsRandomHit(attr.probability);
            }

            return true;
        }
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
            if (string.IsNullOrEmpty(buffName))
                return default;

            if (buffConfig.buffs == null || !buffConfig.buffs.ContainsKey(buffName))
            {
                return default;
            }

            //return buffConfig.buffs[buffName];
            var config = new ActBufConfig();
            buffConfig.buffs.TryGetValue(buffName, out config);
            return config;
        }

        public ActBufAttrConfig GetBuffAttrConfig(string attrName)
        {
            if (string.IsNullOrEmpty(attrName))
                return default;
            if (buffConfig.buffAttrs == null || !buffConfig.buffAttrs.ContainsKey(attrName))
                return default;

            //return buffConfig.buffAttrs[attrName];
            var config = new ActBufAttrConfig();
            buffConfig.buffAttrs.TryGetValue(attrName, out config);
            return config;
        }
    }
}