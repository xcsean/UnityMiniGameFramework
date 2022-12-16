using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class UnityMapNPCTrigger : UnityEngine.MonoBehaviour
    {
        protected MapNPCObject _mapNpc;

        private void Start()
        {
            var comp = this.gameObject.transform.parent.gameObject.GetComponent<UnityGameObjectBehaviour>();

            _mapNpc = comp.mgGameObject as MapNPCObject;
        }

        private void OnTriggerEnter(UnityEngine.Collider other)
        {
            if (UnityGameApp.Inst.currInitStep != MiniGameFramework.GameAppInitStep.EnterMainScene)
            {
                return;
            }

            _mapNpc.OnTriggerEnter(this.gameObject.name, other);
        }
        private void OnTriggerExit(UnityEngine.Collider other)
        {
            if (UnityGameApp.Inst.currInitStep != MiniGameFramework.GameAppInitStep.EnterMainScene)
            {
                return;
            }

            _mapNpc.OnTriggerExit(this.gameObject.name, other);
        }

    }

    public class MapNPCObject : MapRoleObject
    {
        override public string type => "MapNPCObject";
        new public static MapNPCObject create()
        {
            return new MapNPCObject();
        }

        protected MapNPCObjectConf _mapNpcConf;

        public event Action<string, MapNPCObject, UnityEngine.Collider> OnMapNPCTriggerEnter;
        public event Action<string, MapNPCObject, UnityEngine.Collider> OnMapNPCTriggerExit;

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

        public void OnTriggerEnter(string triggerObjectName, UnityEngine.Collider other)
        {
            OnMapNPCTriggerEnter(triggerObjectName, this, other);

            (UnityGameApp.Inst.MainScene.map as Map).OnMapNPCTriggerEnter(triggerObjectName, this, other);
        }
        public void OnTriggerExit(string triggerObjectName, UnityEngine.Collider other)
        {
            OnMapNPCTriggerExit(triggerObjectName, this, other);

            (UnityGameApp.Inst.MainScene.map as Map).OnMapNPCTriggerExit(triggerObjectName, this, other);
        }
    }
}
