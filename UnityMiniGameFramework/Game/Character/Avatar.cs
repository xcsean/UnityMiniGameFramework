using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace UnityMiniGameFramework
{
    public class Avatar
    {
        protected Dictionary<string, AvatarParts> _parts;

        public AvatarParts getAvatarPartsByName(string name)
        {
            return _parts[name];
        }


    }
}
