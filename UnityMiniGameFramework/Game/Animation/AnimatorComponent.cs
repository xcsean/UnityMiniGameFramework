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

    }

    public class AnimatorComponent : GameObjectComponent
    {
        override public string type => "AnimatorComponent";
        public static AnimatorComponent create()
        {
            return new AnimatorComponent();
        }

        protected Skelton _skel;
        protected Dictionary<string, Animation> _anis; // ani name => animation
        protected Dictionary<string, Animation> _currAnis; // ani slot name => current playing animation

        public Skelton skelton => _skel;

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
            return null;
        }

        override public void Init(object config)
        {
            base.Init(config);
        }
        override public void Dispose()
        {
            base.Dispose();
        }
        
        override public void OnUpdate(uint timeElasped)
        {

        }
        override public void OnPostUpdate(uint timeElasped)
        {

        }
    }
}
