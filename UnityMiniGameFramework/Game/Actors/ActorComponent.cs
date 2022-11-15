using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace UnityMiniGameFramework
{
    public class ActorComponent : MonoBehaviour
    {
        protected Skelton _skel;
        protected Dictionary<string, Animation> _anis; // ani name => animation

        protected Dictionary<string, Animation> _currAnis; // ani slot name => current playing animation

        protected Dictionary<string, Audio> _audios; // audio name => audio

        protected Dictionary<string, VFX> _vfxs; // name => vfx
        protected List<VFX> _currVfxs;

        protected List<Action> _penddingActions;
        protected List<Action> _currActions;
        
        public Skelton skelton => _skel;

        public List<Action> penddingActions => _penddingActions;
        public List<Action> currActions => _currActions;

        public List<VFX> currVFXs => _currVfxs;

        public Animation currBaseAnimation
        {
            get
            {
                if(_currAnis.ContainsKey(""))
                {
                    return _currAnis[""];
                }
                return null;
            }
        }
        public Animation getCurrentAnimation(string aniSlotName)
        {
            if (_currAnis.ContainsKey(""))
            {
                return _currAnis[""];
            }
            return null;
        }

        public void StartAction(Action act)
        {

        }

        public void PendAction(Action act)
        {

        }

        public Animation playAnimation(string aniName)
        {
            return null;
        }

        public Audio playAudio(string audioName)
        {
            return null;
        }

        public VFX createVFX(string vfxName)
        {
            return null;
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
