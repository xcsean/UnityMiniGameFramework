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
        public static ActorObject create()
        {
            return new ActorObject();
        }

        protected ActionComponent _actionComponent;
        public ActionComponent actionComponent => _actionComponent;
        protected AnimatorComponent _animatorComponent;
        public AnimatorComponent animatorComponent => _animatorComponent;
        protected AudioComponent _audioComponent;
        public AudioComponent audioComponent => _audioComponent;
        protected VFXComponent _vfxComponent;
        public VFXComponent vfxComponent => _vfxComponent;

        virtual protected ActorObjectConfig _getActorConf(string confname)
        {
            if(UnityGameApp.Inst.CharacterConfs == null)
            {
                return null;
            }
            return UnityGameApp.Inst.CharacterConfs.getActorConf(confname);
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
            _actionComponent.Init(conf.ActionConf);
            this.AddComponent(_actionComponent);

            _animatorComponent = (AnimatorComponent)GameObjectManager.createGameObjectComponent("AnimatorComponent");
            _animatorComponent.Init(conf.AnimatorConf);
            this.AddComponent(_animatorComponent);

            if (conf.AudioConf != null)
            {
                _audioComponent = (AudioComponent)GameObjectManager.createGameObjectComponent("AudioComponent");
                _audioComponent.Init(conf.AudioConf);
                this.AddComponent(_audioComponent);
            }

            if(conf.VFXConf != null)
            {
                _vfxComponent = (VFXComponent)GameObjectManager.createGameObjectComponent(conf.VFXConf.componentType);
                _vfxComponent.Init(conf.VFXConf);
                this.AddComponent(_vfxComponent);
            }
        }
        override public void Dispose()
        {
            base.Dispose();
        }

        override public void OnUpdate(uint timeElasped)
        {
            base.OnUpdate(timeElasped);


        }
        override public void OnPostUpdate(uint timeElasped)
        {


            base.OnPostUpdate(timeElasped);
        }
    }
}
