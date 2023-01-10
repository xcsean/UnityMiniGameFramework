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
            Topon sdk = new Topon();
            sdk.Init(this);
            SDKManager.InitSDK(sdk);
        }

        public virtual void initSDK()
        {
        }

        public virtual void loadVideo()
        {
        }
	
        public virtual void showVideo()
        {
        }
    }

    public class Topon : ISDK
    {
        private SDKBehaviour _topOn;

        public void Init(SDKBehaviour sdk)
        {
            _topOn = sdk;
            _topOn.initSDK();
        }

        public void showVideo()
        {
            _topOn.showVideo();
	    }
	
        public void loadVideo()
        {
            _topOn.loadVideo();
        }
    }
}