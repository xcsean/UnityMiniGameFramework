using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UnityGameAppUpdateBehaviour : MonoBehaviour
    {

        protected virtual void Update()
        {
            GameApp.Inst.OnUpdate();
        }

        protected virtual void FixedUpdate()
        {
        }

        protected virtual void LateUpdate()
        {
            UnityGameApp.Inst.OnPostUpdate();
        }

        void OnApplicationFocus(bool hasFocus)
        {
            //isPaused = !hasFocus;
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                UnityGameApp.Inst.OnAppSuspended();
            }
            else
            {
                UnityGameApp.Inst.OnAppResume();
            }
        }
    }

    public class UnityGameAppBehaviour : MonoBehaviour
    {
        public string appConfigFileName;
        public string uiConfigName;
        public string preloaderUIConfName;
        public UnityEngine.GameObject GameObjectCachePool;
        public UnityEngine.GameObject AudioSourceRoot;
        public PanelSettings unityUIPanelSettings;

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

            UnityGameApp.setInst(new UnityGameApp());

            InitGameAppPlatform();
            UnityGameApp.Inst.isPublish = IsPublish();

            UnityEngine.GameObject.DontDestroyOnLoad(GameObjectCachePool);
            DontDestroyOnLoad(AudioSourceRoot);
            UnityGameApp.Inst.unityUIPanelSettings = unityUIPanelSettings;
            UnityGameApp.Inst.CachePoolRoot = GameObjectCachePool;
            GameAPPInitParameter InitParameter = new GameAPPInitParameter
            {
                appConfigFileName = appConfigFileName,
                uiConfigName = uiConfigName,
                preloaderUIConfName = preloaderUIConfName
            };
            UnityGameApp.Inst.Init(InitParameter);
            Application.targetFrameRate = 60;

            Application.quitting += UnityGameApp.Inst.OnAppExit;
        }

        protected virtual void InitGameAppPlatform()
        {
            GameApp.Inst.Platform = PlatformEnum.PlatformEditor;
        }

        protected virtual void InitSDK()
        {
            
        }
        protected virtual bool IsPublish()
        {
            return false;
        }

        protected virtual void Start()
        {
            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"Game initialized.");
        }

        protected virtual void Update()
        {
            GameApp.Inst.OnUpdate();
        }

        protected virtual void FixedUpdate()
        {
        }

        protected virtual void LateUpdate()
        {
            UnityGameApp.Inst.OnPostUpdate();
        }

        void OnApplicationFocus(bool hasFocus)
        {
            //isPaused = !hasFocus;
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                UnityGameApp.Inst.OnAppSuspended();
            }
            else
            {
                UnityGameApp.Inst.OnAppResume();
            }
        }
    }

    public class UnityGameApp : GameApp
    {
        new public static UnityGameApp Inst => (UnityGameApp)_inst;

        public PanelSettings unityUIPanelSettings;

        public UnityResourceManager UnityResource => (UnityResourceManager)_resManager;
        public Scene MainScene => (Scene)_mainScene;
        public Scene StartScene => (Scene)_startScene;

        protected AnimationManager _aniManager;
        public AnimationManager AniManager => _aniManager;

        protected AudioManager _audManager;
        public AudioManager AudioManager => _audManager;

        protected VFXManager _vfxManager;
        public VFXManager VFXManager => _vfxManager;

        protected CharacterManager _chaManager;
        public CharacterManager CharacterManager => _chaManager;

        protected MapManager _mapManager;
        public MapManager MapManager => _mapManager;

        protected WeaponManager _weaponManager;
        public WeaponManager WeaponManager => _weaponManager;

        protected AnalysisDataManager _analysisMgr;
        public AnalysisDataManager AnalysisMgr => _analysisMgr;

        protected BuffDataManager _buffDataMgr;

        public BuffDataManager BuffDataMgr => _buffDataMgr;

        protected DataManager _datamanager;
        public DataManager Datas => _datamanager;

        protected AIStateManager _aiStateManager;
        public AIStateManager AIStates => _aiStateManager;

        protected UIInputManager _uiInputManager;
        public UIInputManager UIInputManager => _uiInputManager;

        protected UnityNetworkClient _netClient;
        public UnityNetworkClient NetClient => _netClient;

        protected UnityRESTFulClient _restfulClient;
        public UnityRESTFulClient RESTFulClient => _restfulClient;

        protected Queue<Action> _nextFramePostUpdateCall;

        protected HashSet<Action> _updateCall;

        public UnityEngine.GameObject CachePoolRoot;

        public bool isClearUserData = false;
        public bool isPublish = false;

        protected float _panelScale;

        public void regNextFramePostUpdateCall(Action a)
        {
            _nextFramePostUpdateCall.Enqueue(a);
        }
        public void OnPostUpdate()
        {
            while (_nextFramePostUpdateCall.Count > 0)
            {
                Action a = _nextFramePostUpdateCall.Dequeue();
                a.Invoke();
            }
        }
        public void addUpdateCall(Action a)
        {
            _updateCall.Add(a);
        }
        public void removeUpdateCall(Action a)
        {
            _updateCall.Remove(a);
        }

        override public bool Init(GameAPPInitParameter par)
        {
            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"UnityGameApp start initializing...");

            GameApp.CreatGame = ChickenMasterGame.create;

            _nextFramePostUpdateCall = new Queue<Action>();
            _updateCall = new HashSet<Action>();

            // os implement assignment
            _file = new UnityProjFileSystem();
            _rand = new Randomness();

            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"OS implement initialized.");

            _panelScale = _GetPanelScale(unityUIPanelSettings);

            base.Init(par);

            _initNetwork();

            onListenerLogMessage();

            _loadStartScene();

            return true;
        }

        public List<GMLogInfo> logs;
        private void onListenerLogMessage()
        {
            logs = new List<GMLogInfo>();
            if (UnityGameApp.Inst.Datas.localUserConfig != null && UnityGameApp.Inst.Datas.localUserConfig.ShowGm)
            {
                Application.logMessageReceivedThreaded += (string condition, string stackTrace, LogType type) =>
                {
                    if (logs.Count > 50000)
                    {
                        return;
                    }
                    if (type != LogType.Log && type != LogType.Error && type != LogType.Warning)
                    {
                        return;
                    }
                    logs.Add(new GMLogInfo() 
                    {
                        time = $"[{DateTime.Now.ToString("hh:mm:ss")}]",
                        condition = condition,
                        stackTrace = stackTrace,
                        type = type
                    });
                };
            }
        }

        override public void OnUpdate()
        {
            base.OnUpdate();

            _vfxManager.OnUpdate(UnityEngine.Time.deltaTime);
            _weaponManager.OnUpdate(UnityEngine.Time.deltaTime);

            // var list = _updateCall.ToArray();
            // foreach (var a in list)
            // {
            //     a();
            // }
            var arry = _updateCall.ToArray();

            for (int i = arry.Length - 1; i >= 0; i--)
            {
                arry[i]();
            }
        }

        override public void OnAppSuspended()
        {
            if (_datamanager != null && _datamanager.localUserData != null)
            {
                _datamanager.localUserData.writeBack();
            }
            if (Game != null && (Game as ChickenMasterGame).userInfo != null && ((Game as ChickenMasterGame).userInfo.getData() as LocalUserInfo).uuid != null)
            {
                // 在线时间打点
                RESTFulClient.Report(AnalysisMgr.GetPointData1($"切后台时间", 2));
            }
        }
        override public void OnAppResume()
        {
            if (Game != null && (Game as ChickenMasterGame).userInfo != null && ((Game as ChickenMasterGame).userInfo.getData() as LocalUserInfo).uuid != null)
            {
                // 在线时间打点
                RESTFulClient.Report(AnalysisMgr.GetPointData1($"切前台时间", 3));
            }
        }

        override public void OnAppExit()
        {
            if (_datamanager != null && _datamanager.localUserData != null)
            {
                _datamanager.localUserData.writeBack();
            }
            if (Game != null && (Game as ChickenMasterGame).userInfo != null && ((Game as ChickenMasterGame).userInfo.getData() as LocalUserInfo).uuid != null)
            {
                // 在线时间打点
                RESTFulClient.Report(AnalysisMgr.GetPointData1($"退出时间", 4));
            }
            if (RESTFulClient != null)
            {
                RESTFulClient.Fin();
            }
            if (isClearUserData)
            {
                isClearUserData = false;
                Inst.Datas.DelLocalSaveFile("user");
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"Clear LocalStorage User Data.");
            }
        }

        override protected void _createManagers()
        {
            base._createManagers();

            // new managers
            _buffDataMgr = new BuffDataManager();
            _analysisMgr = new AnalysisDataManager();
            _datamanager = new DataManager();
            _aniManager = new AnimationManager();
            _audManager = AudioManager.Instance;
            _vfxManager = new VFXManager();
            _chaManager = new CharacterManager();
            _sceneManager = new UnitySceneManager();
            _resManager = new UnityResourceManager();
            _mapManager = new MapManager();
            _weaponManager = new WeaponManager();
            _aiStateManager = new AIStateManager();
            _uiInputManager = new UIInputManager();

            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"app managers created.");
        }

        override protected void _regClasses()
        {
            base._regClasses();

            // reg config create
            _conf.regConfigCreator("AnimationConfig", AnimationConfig.create);
            _conf.regConfigCreator("CharacterConfigs", CharacterConfigs.create);
            _conf.regConfigCreator("MapConfig", MapConfig.create);
            _conf.regConfigCreator("NetWorkConfig", NetWorkConfig.create);
            _conf.regConfigCreator("VFXConfig", VFXConfig.create);
            _conf.regConfigCreator("WeaponConfig", WeaponConfig.create);
            _conf.regConfigCreator("CMGameConfig", CMGameConfig.create);
            _conf.regConfigCreator("AnalysisConfig", AnalysisConfig.create);
            _conf.regConfigCreator("AudionConfig", AudionConfig.create);
            _conf.regConfigCreator("BuffConfig", BuffConfig.create);

            // reg component
            GameObjectManager.registerGameObjectComponentCreator("ActionComponent", ActionComponent.create);
            GameObjectManager.registerGameObjectComponentCreator("AnimatorComponent", AnimatorComponent.create);
            GameObjectManager.registerGameObjectComponentCreator("AIActorControllerComp", AIActorControllerComp.create);

            // reg object
            GameObjectManager.registerGameObjectCreator("MGGameObject", MGGameObject.create);
            GameObjectManager.registerGameObjectCreator("UnityGameCamera", UnityGameCamera.create);
            GameObjectManager.registerGameObjectCreator("ActorObject", ActorObject.create);
            GameObjectManager.registerGameObjectCreator("Map", Map.create);
            GameObjectManager.registerGameObjectCreator("MapDefAreaObject", MapDefAreaObject.create);
            GameObjectManager.registerGameObjectCreator("MapNPCObject", MapNPCObject.create);
            GameObjectManager.registerGameObjectCreator("MapHeroObject", MapHeroObject.create);
            GameObjectManager.registerGameObjectCreator("MapMonsterObject", MapMonsterObject.create);
            GameObjectManager.registerGameObjectCreator("MapBuildingObject", MapBuildingObject.create);
            GameObjectManager.registerGameObjectCreator("MapVehicleObject", MapVehicleObject.create);
            GameObjectManager.registerGameObjectCreator("GunObject", GunObject.create);

            // reg vfx objects
            _vfxManager.registerVfxObjectCreator("VFXObjectBase", VFXObjectBase.create);
            _vfxManager.registerVfxObjectCreator("VFXFootprintObject", VFXFootprintObject.create);
            _vfxManager.registerVfxObjectCreator("VFXLinerObject", VFXLinerObject.create);
            _vfxManager.registerVfxObjectCreator("VFXRangeCricle", VFXRangeCricle.create);

            // reg levels
            _mapManager.registerMapLevelCreator("CMShootingLevel", CMShootingLevel.create);

            // reg ai states
            _aiStateManager.registerAIStateObjectCreator("AITrace", AITrace.create);
            _aiStateManager.registerAIStateObjectCreator("AITryAttack", AITryAttack.create);
            _aiStateManager.registerAIStateObjectCreator("AIMoveOnPath", AIMoveOnPath.create);
            _aiStateManager.registerAIStateObjectCreator("AIMoveProduct", AIMoveProduct.create);

            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"objects registed.");
        }

        override protected void _initPreloaderUI(string preloaderUIConfName)
        {
            base._initPreloaderUI(preloaderUIConfName);

            if (_ui.preloaderPanel != null)
            {
                _ui.preloaderPanel.AddInitStep("initNetwork");
            }
        }

        override protected void _initConfigs(string appConfigFileName)
        {
            // init config
            base._initConfigs(appConfigFileName);

            MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"app config initialized.");
        }

        override protected void _initManagers()
        {
            base._initManagers();

            _buffDataMgr.Init();
            _audManager.Init();
            _analysisMgr.Init();
            _datamanager.Init();
            _aniManager.Init();
            _chaManager.Init();
            _sceneManager.Init();
            _resManager.Init();
            _mapManager.Init();
            _vfxManager.Init();
            _weaponManager.Init();
        }

        override protected void _initUI(string uiConfigName)
        {
            base._initUI(uiConfigName);

            // reg ui panel creator
            _ui.regUIPanelCreator("preloader", UIPreloaderPanel.create);
            _ui.regUIPanelCreator("UIMainPanel", UIMainPanel.create);
            _ui.regUIPanelCreator("UIPanelStartMain", UIPanelStartMain.create);
            _ui.regUIPanelCreator("UIUpgradePanel", UIUpgradePanel.create);
            _ui.regUIPanelCreator("UILevelEntryPanel", UILevelEntryPanel.create);
            _ui.regUIPanelCreator("UILevelMainPanel", UILevelMainPanel.create);
            _ui.regUIPanelCreator("UIEggPanel", UIEggPanel.create);
            _ui.regUIPanelCreator("UIHeroPanel", UIHeroPanel.create);
            _ui.regUIPanelCreator("UITrainStationPanel", UITrainStationPanel.create);
            _ui.regUIPanelCreator("UIStoreHousePanel", UIStoreHousePanel.create);
            _ui.regUIPanelCreator("UIFlagMainPanel", UIFlagMainPanel.create);
            _ui.regUIPanelCreator("UIFactory1Panel", UIFactory1Panel.create);
            _ui.regUIPanelCreator("UIFactory2Panel", UIFactory2Panel.create);

            _ui.regUIPanelCreator("UICommonFactoryPanel", UICommonFactoryPanel.create);
            _ui.regUIPanelCreator("UIProduceProgressPanel", UIProduceProgressPanel.create);
            _ui.regUIPanelCreator("UIGameMainPanel", UIGameMainPanel.create);
            _ui.regUIPanelCreator("UIOpeningCartoonPanel", UIOpeningCartoonPanel.create);
            _ui.regUIPanelCreator("UITowerHeroPanel", UITowerHeroPanel.create);
            _ui.regUIPanelCreator("UIWeaponAscendPanel", UIWeaponAscendPanel.create);
            _ui.regUIPanelCreator("UIStorehouseCapacityPanel", UIStorehouseCapacityPanel.create);
            _ui.regUIPanelCreator("UITrainStationCapatityPanel", UITrainStationCapatityPanel.create);
            _ui.regUIPanelCreator("UIJoyStickPanel", UIJoyStickPanel.create);
            _ui.regUIPanelCreator("UIMaskPanel", UIMaskPanel.create);
            _ui.regUIPanelCreator("UITrainStationGoldPopupPanel", UITrainStationGoldPopupPanel.create);
            _ui.regUIPanelCreator("UITowerHeroLockHudPanel", UITowerHeroLockHudPanel.create);
            _ui.regUIPanelCreator("UIGMPanel", UIGMPanel.create);
            _ui.regUIPanelCreator("UIGMLogPanel", UIGMLogPanel.create);

            _ui.regUIPanelCreator("UIDoubleAttackPanel", UIDoubleAttackPanel.create);
            _ui.regUIPanelCreator("UIDoubleExpPanel", UIDoubleExpPanel.create);
            _ui.regUIPanelCreator("UIGetSkillPanel", UIGetSkillPanel.create);
            _ui.regUIPanelCreator("UISkillUpgradePanel", UISkillUpgradePanel.create);
            _ui.regUIPanelCreator("UIPassRewardPanel", UIPassRewardPanel.create);
            _ui.regUIPanelCreator("UIOfflineRewardPanel", UIOfflineRewardPanel.create);
            _ui.regUIPanelCreator("UITipsPanel", UITipsPanel.create);

            // reg ui control creator
            _ui.regUIObjectCreator("UIObject", UIObject.create);
            _ui.regUIObjectCreator("UIJoyStickControl", UIJoyStickControl.create);
            _ui.regUIObjectCreator("UILevelStateControl", UILevelStateControl.create);
            _ui.regUIObjectCreator("UIFactoryControl", UIFactoryControl.create);
            _ui.regUIObjectCreator("UIHeroControl", UIHeroControl.create);
        }

        protected void _initNetwork()
        {
            if (_ui.preloaderPanel != null)
            {
                _ui.preloaderPanel.OnInitStep("initNetwork");
            }

            // network client
            NetWorkConfig conf = (NetWorkConfig)_conf.getConfig("network"); // get network config
            if (conf != null)
            {
                _netClient = new UnityNetworkClient();
                _netClient.Init(conf.netConf);

                if (conf.netConf.restfulConf != null)
                {
                    _restfulClient = new UnityRESTFulClient();
                    if (isPublish)
                    {
                        _restfulClient.Init(conf.netConf.restfulConf.url);
                    }
                    else
                    {
                        _restfulClient.Init(conf.netConf.restfulConf.testUrl);
                    }
                }

                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"network initialized.");
            }
            else
            {
                MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_System, $"no network.");
            }

        }

        override protected void _onMainSceneLoaded()
        {
            _uiInputManager.Init();
        }

        public UnityEngine.Vector2 ScreenToUIPos(UnityEngine.Vector2 screenPos)
        {
            //if (unityUIPanelSettings.scaleMode == PanelScaleMode.ScaleWithScreenSize)
            //{
            //    if(unityUIPanelSettings.screenMatchMode == PanelScreenMatchMode.MatchWidthOrHeight)
            //    {
            //        return new Vector2(unityUIPanelSettings.scale * screenPos.x * unityUIPanelSettings.referenceResolution.x / Screen.width, unityUIPanelSettings.scale * screenPos.y * unityUIPanelSettings.referenceResolution.y / Screen.height);
            //    }
            //}

            //return new Vector2(screenPos.x * unityUIPanelSettings.referenceResolution.x / Screen.width, screenPos.y * unityUIPanelSettings.referenceResolution.y / Screen.height);

            return screenPos * _panelScale;
        }

        static protected float _GetPanelScale(PanelSettings settings)
        {
            // Calculate scaling
            float resolvedScale = 1.0f;
            switch (settings.scaleMode)
            {
                case PanelScaleMode.ConstantPixelSize:
                    break;
                case PanelScaleMode.ConstantPhysicalSize:
                    {
                        var dpi = Screen.dpi == 0.0f ? settings.fallbackDpi : Screen.dpi;
                        if (dpi != 0.0f)
                            resolvedScale = settings.referenceDpi / dpi;
                    }
                    break;
                case PanelScaleMode.ScaleWithScreenSize:
                    if (settings.referenceResolution.x * settings.referenceResolution.y != 0)
                    {
                        var refSize = (Vector2)settings.referenceResolution;
                        var sizeRatio = new Vector2(Screen.width / refSize.x, Screen.height / refSize.y);

                        var denominator = 0.0f;
                        switch (settings.screenMatchMode)
                        {
                            case PanelScreenMatchMode.Expand:
                                denominator = Mathf.Min(sizeRatio.x, sizeRatio.y);
                                break;
                            case PanelScreenMatchMode.Shrink:
                                denominator = Mathf.Max(sizeRatio.x, sizeRatio.y);
                                break;
                            default: // PanelScreenMatchMode.MatchWidthOrHeight:
                                var widthHeightRatio = Mathf.Clamp01(settings.match);
                                denominator = Mathf.Lerp(sizeRatio.x, sizeRatio.y, widthHeightRatio);
                                break;
                        }
                        if (denominator != 0.0f)
                            resolvedScale = 1.0f / denominator;
                    }
                    break;
            }

            return settings.scale > 0.0f ? resolvedScale / settings.scale : 0.0f;
        }
    }
}
