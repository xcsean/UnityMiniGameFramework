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
        void showVideo(Action<AdEventArgs> cb);
        void showAutoAd(Action<AdEventArgs> cb);
        void loadVideo();
    }

    public class AdEventArgs
    {
        public String placementId { get; set; }
        public VideoEvent type { get; set; }
    }

    public enum VideoEvent
    {
        //广告加载成功
        AdLoadEvent,
        //广告加载失败
        AdLoadFailureEvent,
        //广告激励回调（可依赖该监听下发游戏激励）
        RewardEvent,
        //广告被关闭
        AdVideoCloseEvent,
        //广告播放结束
        AdVideoEndEvent,
        //广告展示的回调（可依赖这个回调统计展示数据）
        AdVideoStartEvent,
        //广告点击
        AdClickEvent,
        AdCloseEvent,
        //广告视频播放失败
        AdVideoFailureEvent,
        PlayAgainStart,
        PlayAgainFailure,
        PlayAgainEnd,
        PlayAgainClick,
        PlayAgainReward,
        AdSourceAttemptEvent,
        AdSourceFilledEvent,
        AdSourceLoadFailureEvent,
        AdSourceBiddingAttemptEvent,
        AdSourceBiddingFilledEvent,
        AdSourceBiddingFailureEvent,
    }
}
