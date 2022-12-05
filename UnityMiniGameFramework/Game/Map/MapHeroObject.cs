using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class MapHeroObject : MapRoleObject
    {
        protected MapHeroObjectConf _mapHeroConf;

        override public string type => "MapHeroObject";
        new public static MapHeroObject create()
        {
            return new MapHeroObject();
        }

        override protected ActorObjectConfig _getActorConf(string confname)
        {
            if (UnityGameApp.Inst.MapManager.MapConf == null)
            {
                return null;
            }
            _mapHeroConf = UnityGameApp.Inst.MapManager.MapConf.getMapHeroConf(confname);

            if (UnityGameApp.Inst.CharacterManager.CharacterConfs == null)
            {
                return null;
            }
            return UnityGameApp.Inst.CharacterManager.CharacterConfs.getActorConf(_mapHeroConf.actorConfName);
        }

        override public void Init(string confname)
        {
            base.Init(confname);

            // TO DO : init map hero 
        }
    }
}
