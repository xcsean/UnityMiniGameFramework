using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public interface ISDK
    {
        void Init(SDKBehaviour sdk);

        void showVideo(Action<SdkEvent> cb);

        void showAutoAd(Action<SdkEvent> cb);

        void loadVideo();

        void onAdVideoPlayFail(string placementId);

        void onAdVideoClosedEvent(string placementId);

        void onAdClick(string placementId);

        void onAdLoadFail(string placementId);
    }
}
