using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class MapVehicleObject : MapActorObject
    {
        override public string type => "MapVehicleObject";
        new public static MapVehicleObject create()
        {
            return new MapVehicleObject();
        }
    }
}
