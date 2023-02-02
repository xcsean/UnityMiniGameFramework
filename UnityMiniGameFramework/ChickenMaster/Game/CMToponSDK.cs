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
            showTips("ad play success !");
        }

        public void onAdVideoPlayFail(string placementId)
        {
            showTips("ad play fail !");
        }

        public void onAdVideoClosedEvent(string placementId)
        {
        }

        public void onAdClick(string placementId)
        {
        }

        public void onAdLoadFail(string placementId)
        {
            showTips("ad load fail !");
        }

        private void showTips(string str)
        {
            var cmGame = UnityGameApp.Inst.Game;
            if (cmGame != null)
            {
                (cmGame as ChickenMasterGame).ShowTips(CMGNotifyType.CMG_ERROR, str);
            }
        }
    }
}