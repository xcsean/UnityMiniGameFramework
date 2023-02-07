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
            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Debug, "------------------ ad play success. ------------------");
            //showTips("ad play success.");
        }

        public void onAdVideoPlayFail(string placementId)
        {
            showTips($"Sorry, not available now. Plz try again later.");
        }

        public void onAdVideoClosedEvent(string placementId)
        {
        }

        public void onAdClick(string placementId)
        {
        }

        public void onAdLoadFail(string placementId)
        {
            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Debug, "------------------ ad load fail. ------------------");
            //showTips($"ad load fail.");
        }

        private void showTips(string str)
        {
            if (UnityGameApp.Inst.Game is ChickenMasterGame cmGame)
            {
                cmGame.ShowTips(CMGNotifyType.CMG_ERROR, str);
            }
        }

        public virtual void FBInit()
        {
        }
    }
}