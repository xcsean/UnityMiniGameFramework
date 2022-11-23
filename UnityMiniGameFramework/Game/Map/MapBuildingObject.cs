using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    class MapBuildingObject : MapActorObject
    {
        override public string type => "MapBuildingObject";
        new public static MapBuildingObject create()
        {
            return new MapBuildingObject();
        }
    }
}
