using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class SDKManager
    {
        protected static ISDK _sdk;

        public static ISDK sdk => _sdk;

        protected static SDKBehaviour _sdkBehaviour;

        public static SDKBehaviour sdkBehaviour => _sdkBehaviour;

        public static void InitSDK(ISDK s)
        {
            _sdk = s;
            _sdk.Init(_sdkBehaviour);
        }

        public static void setSdkBehaviour(SDKBehaviour s)
        {
            _sdkBehaviour = s;
        }

        public static void showAutoAd(Action<SdkEvent> callball)
        {
            if (SDKManager._sdk != null)
            {
                SDKManager._sdk.showAutoAd(callball);
            }
            else
            {
                callball(new SdkEvent(AdEventType.RewardEvent));
            }
        }
    }
}
