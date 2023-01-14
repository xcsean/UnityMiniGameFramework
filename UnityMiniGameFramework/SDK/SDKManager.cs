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

        public static void showAutoAd(Action callball, string eventName = "")
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.sendVideoEvent(0, eventName);

            if (!SDKManager._sdk.isNull())
            {
                SDKManager._sdk.showAutoAd((SdkEvent args) =>
                {
                    if (args.type == AdEventType.RewardEvent)
                    {
                        //TODO 看完视频下发奖励
                        MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Debug, $"Callback AdEventArgs." + args.type.ToString());
                        cmGame.sendVideoEvent(1, eventName);
                        callball();
                    }
                });
            }
            else
            {
                callball();
            }
        }
    }
}
