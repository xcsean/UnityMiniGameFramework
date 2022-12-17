using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class ActorObject : MGGameObject
    {
        override public string type => "ActorObject";
        new public static ActorObject create()
        {
            return new ActorObject();
        }

        protected ActionComponent _actionComponent;
        public ActionComponent actionComponent => _actionComponent;

        protected AnimatorComponent _animatorComponent;
        public AnimatorComponent animatorComponent => _animatorComponent;

        protected ActorControllerComponent _ctrlComponent;
        public ActorControllerComponent controllerComponent => _ctrlComponent;

        protected bool _needDestroy;
        public bool needDestroy => _needDestroy;
        virtual public void markNeedDestroy()
        {
            _needDestroy = true;
        }

        virtual protected ActorObjectConfig _getActorConf(string confname)
        {
            if(UnityGameApp.Inst.CharacterManager.CharacterConfs == null)
            {
                return null;
            }
            return UnityGameApp.Inst.CharacterManager.CharacterConfs.getActorConf(confname);
        }

        override public void Init(string confname)
        {
            base.Init(confname);

            ActorObjectConfig conf = _getActorConf(confname);
            if (conf == null)
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"Init Actor config({confname}) not exist.");
                return;
            }
            _name = conf.name;

            _actionComponent = (ActionComponent)GameObjectManager.createGameObjectComponent(conf.ActionConf.componentType);
            this.AddComponent(_actionComponent);
            _actionComponent.Init(conf.ActionConf);

            if(conf.AnimatorConf != null)
            {
                _animatorComponent = (AnimatorComponent)GameObjectManager.createGameObjectComponent(conf.AnimatorConf.componentType);
                this.AddComponent(_animatorComponent);
                _animatorComponent.Init(conf.AnimatorConf);
            }

            _needDestroy = false;
        }

        override protected void _onAddComponent(IGameObjectComponent comp)
        {
            if(comp.GetType().IsSubclassOf(typeof(ActorControllerComponent)))
            {
                _ctrlComponent = comp as ActorControllerComponent;
            }
        }

        override public void Dispose()
        {
            UnityGameApp.Inst.VFXManager.onGameObjDestroy(this);

            base.Dispose();
        }

        override public void OnUpdate(float timeElasped)
        {
            base.OnUpdate(timeElasped);


        }
        override public void OnPostUpdate(float timeElasped)
        {
            base.OnPostUpdate(timeElasped);

            if(_needDestroy)
            {
                this.Dispose();
                UnityEngine.GameObject.Destroy(_unityGameObject);
            }
        }
    }
}
