using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityMiniGameFramework
{
    public class SDKBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
            SDKManager.setSdkBehaviour(this);
        }

        public virtual void loadVideo()
        {
        }

        public virtual void showVideo(Action<SdkEvent> callback)
        {
        }

        public virtual void showAutoAd(Action<SdkEvent> callback)
        {
        }

        public void onAdVideoPlayFail(string placementId)
        {
            if (SDKManager.sdk != null)
            {
                SDKManager.sdk.onAdVideoPlayFail(placementId);
            }
        }

        public void onAdVideoClosedEvent(string placementId)
        {
            if (SDKManager.sdk != null)
            {
                SDKManager.sdk.onAdVideoClosedEvent(placementId);
            }
        }

        public void onAdClick(string placementId)
        {
            if (SDKManager.sdk != null)
            {
                SDKManager.sdk.onAdClick(placementId);
            }
        }

        public void onAdLoadFail(string placementId)
        {
            if (SDKManager.sdk != null)
            {
                SDKManager.sdk.onAdLoadFail(placementId);
            }
        }
    }
}
