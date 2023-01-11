using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Text.Json;

using MiniGameFramework;
using UnityEngine;

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

        public List<int> bufAddStates { get; set; }

        public List<ActBufAttrConfig> bufAttrs { get; set; }

        public ActBufDotConfig dot { get; set; }
    }

    public class ActorObjectConfig
    {
        public string name { get; set; }
        public ActionComponentConfig ActionConf { get; set; }
        public AnimatorComponentConfig AnimatorConf { get; set; }
    }

    public class CharacterObjectConfig : ActorObjectConfig
    {
        public AvatarConfig AvatarConf { get; set; }
    }

    [Serializable]
    public class CharacterConf
    {
        public Dictionary<string, ActorObjectConfig> Actors { get; set; }
        public Dictionary<string, CharacterObjectConfig> Characters { get; set; }
        public Dictionary<string, AvatarPartConfig> AvatarParts { get; set; }
    }

    public class CharacterConfigs : JsonConfig
    {
        override public string type => "CharacterConfigs";
        public static CharacterConfigs create()
        {
            return new CharacterConfigs();
        }

        public CharacterConf chaConf => (CharacterConf)_conf;

        override protected object _JsonDeserialize(string confStr)
        {
            //return JsonSerializer.Deserialize<CharacterConf>(confStr);
            return JsonUtil.FromJson<CharacterConf>(confStr);
        }

        public ActorObjectConfig getActorConf(string actorName)
        {
            if(chaConf.Actors == null || !chaConf.Actors.ContainsKey(actorName))
            {
                return null;
            }
            return chaConf.Actors[actorName];
        }
        public CharacterObjectConfig getCharacterConf(string chaName)
        {
            if (chaConf.Characters == null ||  !chaConf.Characters.ContainsKey(chaName))
            {
                return null;
            }
            return chaConf.Characters[chaName];
        }
        public AvatarPartConfig getAvatarPartConf(string avatarPartName)
        {
            if (chaConf.AvatarParts == null ||  !chaConf.AvatarParts.ContainsKey(avatarPartName))
            {
                return null;
            }
            return chaConf.AvatarParts[avatarPartName];
        }
    }
}
