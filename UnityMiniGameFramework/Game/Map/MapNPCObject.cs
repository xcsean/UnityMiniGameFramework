using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class MapNPCObject : MapActorObject
    {
        override public string type => "MapNPCObject";
        new public static MapNPCObject create()
        {
            return new MapNPCObject();
        }
    }
}
