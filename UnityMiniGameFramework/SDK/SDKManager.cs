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
            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Debug, $"Developer SDKManager InitSDK:" +  _sdk);
        }

        public static void showAutoAd(Action callball, string eventName = "")
        {
            UnityGameApp.Inst.RESTFulClient.Report(UnityGameApp.Inst.AnalysisMgr.GetPointData9($"{eventName}"));
            try
            {
                _sdk.showAutoAd((SdkEvent args) =>
                {
                    if (args.type == AdEventType.RewardEvent)
                    {
                        //TODO 看完视频下发奖励
                        MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Debug, $"Callback AdEventArgs." + args.type.ToString());
                        UnityGameApp.Inst.RESTFulClient.Report(UnityGameApp.Inst.AnalysisMgr.GetPointData10($"{eventName}"));
                        callball();
                    }
                });
            }
            catch (Exception ex)
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Mybe SDK not init details ===>>>> " + ex);
            }
        }
}
}
