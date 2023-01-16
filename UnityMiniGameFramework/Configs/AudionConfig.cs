using System;
using System.Collections.Generic;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    
    public class AudionConf
    {
        public string Name { get; set; }
        public string SrcPath { get; set; }
    }
    
    
    [Serializable]
    public class AudionConfigs
    {
        public Dictionary<string, AudionConf> Audios { get; set; }
    }


    public class AudionConfig : JsonConfig
    {
        public override string type => "AudionConfig";

        public static AudionConfig create()
        {
            return new AudionConfig();
        }
        
        public AudionConfigs audionsConf => (AudionConfigs)_conf;
        
        protected override object _JsonDeserialize(string confStr)
        {
            return JsonUtil.FromJson<AudionConfigs>(confStr);
        }
        
        public AudionConf getAudioConfig(string audioName)
        {
            if (audionsConf.Audios == null || audionsConf.Audios.ContainsKey(audioName))
                return null;
            return audionsConf.Audios[audioName];
        }
        
    }
}