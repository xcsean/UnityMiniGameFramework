using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class Character
    {
        protected Dictionary<string, Attribute> _attributes; // key name => attributes
        protected Dictionary<string, AvatarParts> _avatars; // position => avatar part
        protected Dictionary<string, AvatarParts> _attachments; // positoin => attached avatar


    }
}
