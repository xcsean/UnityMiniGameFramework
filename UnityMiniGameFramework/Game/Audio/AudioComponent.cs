using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class AudioComponentConfig
    {
    }

    public class AudioComponent : GameObjectComponent
    {
        override public string type => "AudioComponent";
        public static AudioComponent create()
        {
            return new AudioComponent();
        }
        
        protected Dictionary<string, Audio> _audios; // audio name => audio

        public Audio playAudio(string audioName)
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
