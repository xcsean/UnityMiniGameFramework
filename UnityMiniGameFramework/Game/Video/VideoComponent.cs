using System;

using UnityEngine;
using UnityEngine.Video;

namespace UnityMiniGameFramework
{
    /// <summary>
    /// 视频播放
    /// </summary>
    public class VideoComponent : MonoBehaviour
    {
        [SerializeField] protected GameObject _closeBtn;
        public GameObject _videoObj;
        protected VideoPlayer _videoPlayer;
        protected RenderTexture _rt;

        protected Action playCb;

        protected void Awake()
        {
            if (_videoObj != null)
            {
                _rt = _videoObj.GetComponent<RenderTexture>();

                _videoPlayer = _videoObj.GetComponent<VideoPlayer>();
                _videoPlayer.isLooping = false;
                _videoPlayer.playOnAwake = false;
                _videoPlayer.Stop();
            }
            else
            {
                MiniGameFramework.Debug.DebugOutput(MiniGameFramework.DebugTraceType.DTT_Debug, "VideoComponent VideoGameObject is null.");
            }
            // 宽适配下
            transform.localScale = new Vector3(Screen.width / 750f, Screen.width / 750f, 1);
        }

        public void Hide()
        {
            //gameObject.SetActive(false);
            transform.localPosition = new Vector3(-10000, 0, 0);

            UnityGameApp.Inst.AudioManager.ResumeAll();

            Time.timeScale = 1;
        }

        public void Show()
        {
            //gameObject.SetActive(true);
            transform.localPosition = new Vector3(0, 0, 0);

            UnityGameApp.Inst.AudioManager.PauseAll();

            Time.timeScale = 0;
        }

        public void Play(Action endCb = null)
        {
            if (_videoPlayer == null)
            {
                if (endCb != null) endCb();
                return;
            }
            if (_videoPlayer.isPlaying)
            {
                return;
            }

            _videoPlayer.playbackSpeed = 1f;
            _videoPlayer.loopPointReached += OnPlayVideoCb;
            _videoPlayer.Play();

            playCb = endCb;

            if (_closeBtn != null)
            {
                _closeBtn.SetActive(false);
            }

            Invoke("DelayShow", 0.2f);
        }

        protected void DelayShow()
        {
            Show();
        }

        protected void DelayShowCloseBtn()
        {
            if (_closeBtn == null || _closeBtn.activeSelf)
            {
                return;
            }
            if (_videoPlayer != null && _videoPlayer.isPlaying)
            {
                _closeBtn.SetActive(true);
            }
        }

        protected void Update()
        {
            if (_closeBtn == null || _closeBtn.activeSelf)
            {
                return;
            }
            if (_videoPlayer.time > 11f)
            {
                DelayShowCloseBtn();
            }
        }

        protected void OnPlayVideoCb(VideoPlayer vp)
        {
            //vp.playbackSpeed = 0.1f;
            DelayShowCloseBtn();
        }

        public void Stop()
        {
            Hide();

            if (_videoPlayer != null)
            {
                _videoPlayer.Stop();
                _videoPlayer.loopPointReached -= OnPlayVideoCb;
            }
            if (_rt != null)
            {
                _rt.Release();
            }
        }

        public void onClickBtnClose()
        {
            if (playCb != null)
            {
                playCb();
                playCb = null;
            }
            Stop();
        }
    }
}
