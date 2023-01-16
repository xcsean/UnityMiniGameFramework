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
        private SDKBehaviour _topOn;
        
        public bool isNull()
        {
            return _topOn == null;
        }

        public void Init(SDKBehaviour sdk)
        {
            _topOn = sdk;
        }

        public void showVideo(Action<SdkEvent> cb)
        {
            _topOn.showVideo(cb);
	    }
	
        public void loadVideo()
        {
            _topOn.loadVideo();
        }

        public void showAutoAd(Action<SdkEvent> cb)
        {
            _topOn.showAutoAd(cb);
        }

        public void onAdVideoPlayFail(string placementId)
        {
            // throw new NotImplementedException();
        }

        public void onAdVideoClosedEvent(string placementId)
        {
            // throw new NotImplementedException();
        }

        public void onAdClick(string placementId)
        {
            // throw new NotImplementedException();
        }

        public void onAdLoadFail(string placementId)
        {
            // throw new NotImplementedException();
        }
    }
}