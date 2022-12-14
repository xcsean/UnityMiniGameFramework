using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class AIMoveOnPath : AIState
    {
        public static AIMoveOnPath create(ActorObject actor)
        {
            return new AIMoveOnPath(actor);
        }

        protected RigibodyMoveAct _movAct;

        public AIMoveOnPath(ActorObject actor) : base(actor)
        {
            _movAct = (actor as MapRoleObject).moveAct;
        }

        public override void Init(MapConfAIState conf)
        {
            base.Init(conf);

            var map = (UnityGameApp.Inst.MainScene.map as Map);
            var path = map.getPath(conf.targetName);
            if(path == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"map [{map.name}] path [{conf.targetName}] not exist");
                return;
            }

            _movAct.moveOn(path, 0.5f);
        }
    }
}
