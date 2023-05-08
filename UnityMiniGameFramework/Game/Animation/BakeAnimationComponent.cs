using System.Collections.Generic;
using MiniGameFramework;
using UnityEngine;
using Debug = MiniGameFramework.Debug;

namespace UnityMiniGameFramework
{
    public class BakeAnimationComponent : GameObjectComponent, IAnimation
    {
        private BakeClipConf _clipConf;
        private string _curClip;
        private UnityEngine.Renderer _renderer;
        private float sumTime = 0.0f;
        private float beginTime = 0.0f;

        public static BakeAnimationComponent create()
        {
            return new BakeAnimationComponent();
        }

        private BakeAnimationComponent()
        {
            _curClip = string.Empty;
        }

        public override void Init(object config)
        {
            base.Init(config);

            AnimatorComponentConfig acConf = config as AnimatorComponentConfig;

            _clipConf =
                UnityGameApp.Inst.AniManager.ClipConfig.getBakeClipConf(acConf.AnimatorName);
            if (_clipConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error,
                    $"Init [{_gameObject.name}] BakeAnimationComponent with animator name [{acConf.AnimatorName}] config not exist");
                return;
            }

            _renderer = ((MGGameObject) _gameObject).unityGameObject.GetComponent<UnityEngine.Renderer>();
            if (!_renderer)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error,
                    $"Init [{_gameObject.name}] BakeAnimationComponent with Renderer name [{((MGGameObject) _gameObject).unityGameObject.name}] config not exist");
                return;
            }
        }

        public float playSpeed { get; set; }

        public bool isCurrBaseAnimation(string aniName)
        {
            return _curClip.Equals(aniName);
        }

        public void playAnimation(string aniName, float speed = 1)
        {
            if (!_clipConf.clips.ContainsKey(aniName))
            {
                return;
            }

            _curClip = aniName;
            var clipConfig = _clipConf.clips[aniName];
            MaterialPropertyBlock prop = new MaterialPropertyBlock();
            prop.SetFloat("_SpawnTime", Time.timeSinceLevelLoad);
            prop.SetFloat("_BeginFrame", clipConfig.BeginFrame);
            prop.SetFloat("_EndFrame", clipConfig.EndFrame);
            prop.SetInt("_IsLoopPlay", clipConfig.IsLoop ? 1 : 0);
            sumTime = (clipConfig.EndFrame - clipConfig.BeginFrame + 1) / _clipConf.FrameRate;
            _renderer.SetPropertyBlock(prop);
        }

        public void stopAnimation(string aniName)
        {
            //throw new System.NotImplementedException();
        }

        public float getAnimatorStateInfoNormalizedTime()
        {
            if (!_clipConf.clips.ContainsKey(_curClip))
                return 0;
            if (_clipConf.clips[_curClip].IsLoop)
                return 0;
            if (sumTime <= 0)
                return 0;
            float nowTime = Time.timeSinceLevelLoad;
            return (nowTime - beginTime) / sumTime;
        }
    }
}