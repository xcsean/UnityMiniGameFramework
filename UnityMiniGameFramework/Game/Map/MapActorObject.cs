using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    abstract public class MapActorObject : ActorObject
    {
        public event Action OnMapLevelFinish;
        public static Queue<MapActorObject> waitMapInitActors = new Queue<MapActorObject>();

        override public void Init(string confname)
        {
            base.Init(confname);
        }

        public override void PostInit()
        {
            base.PostInit();

            var map = UnityGameApp.Inst.MainScene.map as Map;
            if (map == null)
            {
                waitMapInitActors.Enqueue(this);
            }
            else
            {
                map.OnAddMapActor(this);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            (UnityGameApp.Inst.MainScene.map as Map).OnRemoveMapActor(this);
        }

        public virtual void DispatchMapLevelFinish()
        {
            if(OnMapLevelFinish!=null)
            {
                OnMapLevelFinish();
            }
        }
    }
}
