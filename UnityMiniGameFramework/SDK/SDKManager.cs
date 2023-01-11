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

        public static void InitSDK(ISDK s)
        {
            _sdk = s;
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
