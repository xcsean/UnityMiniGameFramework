using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class InputActorControllerComp : ActorControllerComponent
    {
        override public string type => "NetworkActorControllerComp";
        new public static InputActorControllerComp create()
        {
            return new InputActorControllerComp();
        }


    }
}
