using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityMiniGameFramework
{
    public class Topon : ISDK
    {
        private SDKBehaviour _topOn;

        public void Init(SDKBehaviour sdk)
        {
            _topOn = sdk;
        }

        public void showVideo(Action<AdEventArgs> cb)
        {
            _topOn.showVideo(cb);
	    }
	
        public void loadVideo()
        {
            _topOn.loadVideo();
        }

        public void showAutoAd(Action<AdEventArgs> cb)
        {
            _topOn.showVideo(cb);
        }
    }
}