using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class MapNPCObject : MapRoleObject
    {
        override public string type => "MapNPCObject";
        new public static MapNPCObject create()
        {
            return new MapNPCObject();
        }

        protected MapNPCObjectConf _mapNpcConf;

        override protected ActorObjectConfig _getActorConf(string confname)
        {
            if (UnityGameApp.Inst.MapManager.MapConf == null)
            {
                return null;
            }
            _mapNpcConf = UnityGameApp.Inst.MapManager.MapConf.getMapNPCConf(confname);

            if (UnityGameApp.Inst.CharacterManager.CharacterConfs == null)
            {
                return null;
            }
            return UnityGameApp.Inst.CharacterManager.CharacterConfs.getActorConf(_mapNpcConf.actorConfName);
        }

        override public void Init(string confname)
        {
            base.Init(confname);

            this._name = this._unityGameObject.name;

            // TO DO : init map NPC 

            if(_mapNpcConf.aiStates != null && _mapNpcConf.aiStates.Count > 0)
            {
                // init ai
                var aiControlComp = new AIActorControllerComp();
                this.AddComponent(aiControlComp);
                aiControlComp.Init(_mapNpcConf.aiStates);
            }

            if(_mapNpcConf.defaultAniName != null)
            {
                _rigiMovAct.setDefaultAni(_mapNpcConf.defaultAniName);
            }
        }

        public override void PostInit()
        {
            base.PostInit();


        }
    }
}
