using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class AIActorControllerComp : ActorControllerComponent
    {
        override public string type => "NetworkActorControllerComp";
        new public static AIActorControllerComp create()
        {
            return new AIActorControllerComp();
        }


    }
}
