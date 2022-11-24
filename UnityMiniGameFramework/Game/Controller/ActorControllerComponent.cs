using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class ActorControllerComponent : GameObjectComponent
    {
        override public string type => "ActorControllerComponent";
        public static ActorControllerComponent create()
        {
            return new ActorControllerComponent();
        }

        override public void Init(object config)
        {
            base.Init(config);


        }
    }
}
