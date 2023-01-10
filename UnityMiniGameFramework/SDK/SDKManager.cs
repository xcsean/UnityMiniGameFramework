using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityMiniGameFramework
{
    public class SDKManager
    {
        protected static ISDK _sdk;
        public static ISDK sdk => _sdk;
        public static void InitSDK(ISDK s)
        {
            _sdk = s;
        }
    }
}
