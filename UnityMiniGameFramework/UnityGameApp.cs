using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;
using UnityEngine;

namespace UnityMiniGameFramework
{
    public class UnityGameAppBehaviour : MonoBehaviour
    {
        public string appConfigFileName;

        static void dbgOutput(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }
        static void dbgError(string msg)
        {
            UnityEngine.Debug.LogError(msg);
        }

        protected virtual void Awake()
        {
            MiniGameFramework.Debug.Init(dbgOutput, dbgError);

            GameApp.setInst(new UnityGameApp());

            GameApp.Inst.Init(appConfigFileName);
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
        }

        protected virtual void FixedUpdate()
        {
        }

        protected virtual void LateUpdate()
        {
        }
    }

    public class UnityGameApp : GameApp
    {
        new public static UnityGameApp Inst => (UnityGameApp)_inst;

        protected AnimationManager _aniManager;
        public AnimationManager AniManager => _aniManager;

        protected AudioManager _audManager;
        public AudioManager AudioManager => _audManager;

        protected VFXManager _vfxManager;
        public VFXManager VFXManager => _vfxManager;

        protected CharacterManager _chaManager;
        public CharacterManager CharacterManager => _chaManager;

        protected UnityNetworkClient _netClient;
        public UnityNetworkClient NetClient => _netClient;

        protected CharacterConfigs _charConf;
        public CharacterConfigs CharacterConfs => _charConf;

        override public bool Init(string appConfigFileName)
        {
            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"UnityGameApp start initializing...");

            base.Init(appConfigFileName);

            // os implement assignment
            _file = new UnityProjFileSystem();

            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"OS implement initialized.");

            // reg config create
            _conf.regConfigCreator("CharacterConfigs", CharacterConfigs.create);
            _conf.regConfigCreator("NetWorkConfig", NetWorkConfig.create);

            // reg component
            GameObjectManager.registerGameObjectComponentCreator("ActionComponent",     ActionComponent.create);
            GameObjectManager.registerGameObjectComponentCreator("AnimatorComponent",   AnimatorComponent.create);
            GameObjectManager.registerGameObjectComponentCreator("AudioComponent",      AudioComponent.create);
            GameObjectManager.registerGameObjectComponentCreator("VFXComponent",        VFXComponent.create);

            // reg object
            GameObjectManager.registerGameObjectCreator("ActorObject", ActorObject.create);
            GameObjectManager.registerGameObjectCreator("CharacterObject", CharacterObject.create);
            
            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"objects registed.");

            // new managers
            _aniManager = new AnimationManager();
            _audManager = new AudioManager();
            _vfxManager = new VFXManager();
            _chaManager = new CharacterManager();

            // init config
            _initConfigs(appConfigFileName);

            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"app config initialized.");

            // network client
            NetWorkConfig conf = (NetWorkConfig)_conf.getConfig("network"); // get network config
            if(conf != null)
            {
                _netClient = new UnityNetworkClient();
                _netClient.Init(conf.netConf);

                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"network initialized.");
            }
            else
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"no network.");
            }

            return true;
        }

        protected void _initConfigs(string appConfigFileName)
        {
            // init config
            _conf.InitAppConfig(appConfigFileName);

            _charConf = (CharacterConfigs)_conf.getConfig("characters");
        }
    }
}
