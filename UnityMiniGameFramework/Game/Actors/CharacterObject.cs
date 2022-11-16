using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace UnityMiniGameFramework
{
    public class CharacterObject : ActorObject
    {
        override public string type => "CharacterObject";
        new public static CharacterObject create()
        {
            return new CharacterObject();
        }

        protected Dictionary<string, Attribute> _attributes; // key name => attributes
        protected Dictionary<string, AvatarParts> _avatars; // position => avatar part
        protected Dictionary<string, AvatarParts> _attachments; // positoin => attached avatar


    }
}
