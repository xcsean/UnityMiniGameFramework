using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace UnityMiniGameFramework
{
    public class VideoManager
    {
        public VideoComponent videoComp;

        public void Init()
        {
        }

        /// <summary>
        /// 预加载视频
        /// </summary>
        public void PreLoadVideo()
        {
            if (videoComp == null)
            {
                var obj = UnityGameApp.Inst.UnityResource.LoadUnityPrefabObject("Video/AdPlayView");
                obj = GameObject.Instantiate(obj);
                videoComp = obj.GetComponent<VideoComponent>();
                videoComp.Hide();
            }
        }

        public void PlayVideo(Action cb = null)
        {
            if (videoComp == null)
            {
                PreLoadVideo();
            }
            if (UnityGameApp.Inst.currInitStep != MiniGameFramework.GameAppInitStep.EnterMainScene)
            {
                // 暂时只有主场景加了单独的canvas，可以使用
                return;
            }
            UnityGameApp.Inst.MainScene.SetObjToVideoCanvas(videoComp.gameObject);
 
            videoComp.Play(cb);
        }

        public void ClosePlay()
        {
            if (videoComp == null)
            {
                return;
            }
            videoComp.Stop();
        }
    }
}
