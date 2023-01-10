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

        public virtual void loadVideo()
        {
        }

        public virtual void showVideo(Action<AdEventArgs> cb)
        {
        }

        public virtual void showAutoAd(Action<AdEventArgs> cb)
        {
        }
    }

    public class SDKManager
    {
        protected static ISDK _sdk;
        public static ISDK sdk => _sdk;
        public static void InitSDK(ISDK s)
        {
            _sdk = s;
        }
    }
}
