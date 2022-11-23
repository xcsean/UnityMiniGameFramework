using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class AnimatorComponentConfig
    {
        public string componentType { get; set; }
        public string AnimatorName { get; set; }
    }

    public class AnimatorComponent : GameObjectComponent
    {
        override public string type => "AnimatorComponent";
        public static AnimatorComponent create()
        {
            return new AnimatorComponent();
        }

        protected UnityEngine.Animator _unityAnimator;

        protected Skelton _skel;
        protected Dictionary<string, Animation> _anis; // ani name => animation
        protected Dictionary<string, Animation> _currAnis; // ani slot name => current playing animation

        public Skelton skelton => _skel;

        public AnimatorComponent()
        {
            _currAnis = new Dictionary<string, Animation>();
            _anis = new Dictionary<string, Animation>();
        }

        public Animation currBaseAnimation
        {
            get
            {
                if (_currAnis.ContainsKey(""))
                {
                    return _currAnis[""];
                }
                return null;
            }
        }
        public Animation getCurrentAnimation(string aniSlotName)
        {
            if (_currAnis.ContainsKey(aniSlotName))
            {
                return _currAnis[aniSlotName];
            }
            return null;
        }

        public Animation playAnimation(string aniName)
        {
            if(!_anis.ContainsKey(aniName))
            {
                return null;
            }

            Animation ani = _anis[aniName];

            if(_currAnis.ContainsKey(ani.aniSlotName))
            {
                // TO DO : stop or transit old ani
                //_currAnis[ani.aniSlotName];

                _currAnis.Remove(ani.aniSlotName);
            }

            _unityAnimator.Play(ani.aniClipName, ani.slotIndex);

            _currAnis[ani.aniSlotName] = ani;

            return ani;
        }

        public void stopAnimation(string aniName)
        {
            if (!_anis.ContainsKey(aniName))
            {
                return;
            }

            Animation ani = _anis[aniName];

            if (!_currAnis.ContainsKey(ani.aniSlotName))
            {
                return;
            }

            _currAnis.Remove(ani.aniSlotName);

            // TO DO : stop playing
        }

        override public void Init(object config)
        {
            base.Init(config);

            AnimatorComponentConfig acConf = config as AnimatorComponentConfig;

            AnimatorConf animatorConf = UnityGameApp.Inst.AniManager.AnimationConfs.getAnimatorConf(acConf.AnimatorName);
            if(animatorConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Init [{_gameObject.name}] Animator component with animator name [{acConf.AnimatorName}] config not exist");
                return;
            }

            UnityEngine.Transform skelTrans = ((MGGameObject)_gameObject).unityGameObject.transform.Find(animatorConf.SkeltonRootName);
            if(skelTrans == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Init [{_gameObject.name}] Animator component [{acConf.AnimatorName}] skelton [{animatorConf.SkeltonRootName}] not exist");
                return;
            }
            _skel = new Skelton(skelTrans.gameObject);

            _unityAnimator = ((MGGameObject)_gameObject).unityGameObject.GetComponent<UnityEngine.Animator>();
            if (skelTrans == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Init [{_gameObject.name}] Animator component [{acConf.AnimatorName}] animator not exist");
                return;
            }

            Dictionary<string, UnityEngine.AnimationClip> clips = new Dictionary<string, UnityEngine.AnimationClip>();
            foreach (var clipInfo in _unityAnimator.runtimeAnimatorController.animationClips)
            {
                clips[clipInfo.name] = clipInfo;
            }

            foreach(var pair in animatorConf.Animations)
            {
                if(!clips.ContainsKey(pair.Value.aniFile))
                {
                    Debug.DebugOutput(DebugTraceType.DTT_Error, $"Init [{_gameObject.name}] Animator component [{acConf.AnimatorName}] aniclip [{pair.Value.aniFile}] not exist");
                    continue;
                }

                int slot = 0;
                try
                {
                    slot = _unityAnimator.GetLayerIndex(pair.Value.aniSlot); // TO DO : use slot above in clipInfos;
                    if(slot < 0)
                    {
                        slot = 0;
                    }
                }
                catch(Exception )
                {
                    slot = 0;
                }

                var ani = new Animation(pair.Value, clips[pair.Value.aniFile], slot);
                _anis[ani.aniName] = ani;

                if(pair.Value.evetns != null)
                {
                    foreach(var evt in pair.Value.evetns)
                    {
                        ani.aniClip.AddEvent(new UnityEngine.AnimationEvent()
                        {
                            time = evt.length,
                            functionName = $"__onAniEvent_{evt.name}"
                        });
                    }
                }
            }
        }
        override public void Dispose()
        {
            base.Dispose();
        }
        
        override public void OnUpdate(uint timeElasped)
        {
            List<string> toRemoveCurrAnis = new List<string>();
            foreach(var pair in _currAnis)
            {
                var ani = pair.Value;
                var stateInfo = _unityAnimator.GetCurrentAnimatorStateInfo(ani.slotIndex);
                if (!stateInfo.loop)
                {
                    if(stateInfo.normalizedTime >= 1 && !_unityAnimator.IsInTransition(ani.slotIndex))
                    {
                        toRemoveCurrAnis.Add(pair.Key);
                    }
                }
            }

            foreach(var key in toRemoveCurrAnis)
            {
                _currAnis.Remove(key);
            }
        }
        override public void OnPostUpdate(uint timeElasped)
        {

        }
    }
}
