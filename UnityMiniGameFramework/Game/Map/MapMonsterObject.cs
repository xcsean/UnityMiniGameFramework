using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{ 
    public class MapMonsterObject : MapActorObject
    {
        override public string type => "MapMonsterObject";
        new public static MapMonsterObject create()
        {
            return new MapMonsterObject();
        }
    }
}
