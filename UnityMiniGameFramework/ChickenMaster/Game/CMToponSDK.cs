using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityMiniGameFramework
{
    public class CMToponSDK : ISDK
    {
        public virtual void Init()
        {
            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Debug, $"Developer CMToponSDK Init");
        }

        public virtual void showVideo(Action<SdkEvent> cb)
        {
        }
	
        public virtual void loadVideo()
        {
        }
        
        public virtual void showAutoAd(Action<SdkEvent> cb)
        {
            cb(new SdkEvent(AdEventType.RewardEvent, "test"));
        }

        public void onAdVideoPlayFail(string placementId)
        {
        }

        public void onAdVideoClosedEvent(string placementId)
        {
        }

        public void onAdClick(string placementId)
        {
        }

        public void onAdLoadFail(string placementId)
        {
        }
    }
}