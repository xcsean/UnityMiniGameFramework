using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class ChickenMasterGame : IGame
    {
        public static IGame create()
        {
            return new ChickenMasterGame();
        }

        protected IDataObject _userInfo;
        public IDataObject userInfo => _userInfo;

        protected IDataObject _baseInfo;
        public IDataObject baseInfo => _baseInfo;

        protected CMGameConfig _gameConf;
        public CMGameConfig gameConf => _gameConf;

        protected SelfControl _self;
        public SelfControl Self => _self;

        protected Dictionary<string, CMNPCHeros> _cmNPCHeros;
        public Dictionary<string, CMNPCHeros> cmNPCHeros => _cmNPCHeros;

        protected IGameObject _levelCenterObject;
        public IGameObject levelCenterObject => _levelCenterObject;

        UIMainPanel _uiMainPanel;
        public UIMainPanel uiMainPanel => _uiMainPanel;

        UILevelMainPanel _uiLevelMainPanel;
        public UILevelMainPanel uiLevelMainPanel => _uiLevelMainPanel;


        public ChickenMasterGame()
        {
            _self = new SelfControl();
            _cmNPCHeros = new Dictionary<string, CMNPCHeros>();
        }

        public async Task InitAsync()
        {
            _gameConf = UnityGameApp.Inst.Conf.getConfig("cmgame") as CMGameConfig;

            await UnityGameApp.Inst.Datas.CreateLocalUserDataAsync();

            bool newDataAdded = false;
            _userInfo = UnityGameApp.Inst.Datas.localUserData.getData("userInfo");
            if (_userInfo == null)
            {
                // new user info
                var userInfo = new LocalUserInfo()
                {
                    uid = "test",
                    uuid = "test",
                    lastLoginTime = (int)(DateTime.Now.Ticks / 1000)
                };
                _userInfo = new DataObject(userInfo);
                _userInfo.markDirty();

                UnityGameApp.Inst.Datas.localUserData.addNewData("userInfo", _userInfo);
                newDataAdded = true;
            }

            _baseInfo = UnityGameApp.Inst.Datas.localUserData.getData("baseInfo");
            if (_baseInfo == null)
            {
                // new base info
                var baseInfo = new LocalBaseInfo()
                {
                    gold = 100,
                    level = 1,
                    exp = 0,
                    selfHero = new LocalHeroInfo()
                    {
                        mapHeroName = "testHero",
                        holdWeapon = new LocalWeaponInfo()
                        {
                            id = 1,
                            level = 1,
                        }
                    },
                    defenseHeros = new List<LocalHeroInfo>(),
                    buildings = new List<LocalBuildingInfo>(),
                    weapons = new List<LocalWeaponInfo>(),
                    backPackItems = new List<LocalPackItemInfo>()
                };
                _baseInfo = new DataObject(baseInfo);
                _baseInfo.markDirty();

                UnityGameApp.Inst.Datas.localUserData.addNewData("baseInfo", _baseInfo);
                newDataAdded = true;
            }

            if(newDataAdded)
            {
                // write back
                await UnityGameApp.Inst.Datas.localUserData.writeBackAsync();
            }
        }

        public void OnStartSceneLoaded()
        {
        }

        public void OnMainSceneLoaded()
        {
            // init self
            _self.Init();

            // init npc heros
            var bi = _baseInfo.getData() as LocalBaseInfo;
            for (int i = 0; i < bi.defenseHeros.Count; ++i)
            {
                var cmHero = new CMNPCHeros();
                cmHero.Init(bi.defenseHeros[i]);

                _cmNPCHeros[cmHero.heroInfo.mapHeroName] = cmHero;
            }

            // init ui
            _uiMainPanel = UnityGameApp.Inst.UI.getUIPanel("MainUI") as UIMainPanel;

            var tr = UnityGameApp.Inst.MainScene.mapRoot.transform.Find(_gameConf.gameConfs.levelCenterObjectName);
            if(tr == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Main Scene [{UnityGameApp.Inst.MainScene.name}] map root without level center object [{_gameConf.gameConfs.levelCenterObjectName}]");
            }
            else
            {
                var comp = tr.gameObject.GetComponent<UnityGameObjectBehaviour>();
                if(comp == null)
                {
                    Debug.DebugOutput(DebugTraceType.DTT_Error, $"Main Scene [{UnityGameApp.Inst.MainScene.name}] level center object [{_gameConf.gameConfs.levelCenterObjectName}] without UnityGameObjectBehaviour ");
                }
                else
                {
                    _levelCenterObject = comp.mgGameObject;
                }
            }
        }

        public void OnUpdate()
        {
            _self.OnUpdate();
        }

        public void InitUILevelMainPanel(string pannelName)
        {
            if (_uiLevelMainPanel == null)
            {
                // create level main UI
                _uiLevelMainPanel = UnityGameApp.Inst.UI.createUIPanel(pannelName) as UILevelMainPanel;
                if (_uiLevelMainPanel == null)
                {
                    Debug.DebugOutput(DebugTraceType.DTT_Error, $"LevelMainUI panel config not exist");
                    return;
                }
                _uiLevelMainPanel.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.uiRootObject).unityGameObject.transform);
            }
        }

        public void AddDefenseHero(string mapHeroName)
        {
            var heroConf = _gameConf.getCMHeroConf(mapHeroName);
            if(heroConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"AddDefenseHero [{mapHeroName}] config not exist");
                return;
            }

            LocalHeroInfo heroInfo = new LocalHeroInfo()
            {
                mapHeroName = mapHeroName,
                position = null,
                holdWeapon = null,
            };

            var cmHero = new CMNPCHeros();
            cmHero.Init(heroInfo);

            _cmNPCHeros[cmHero.heroInfo.mapHeroName] = cmHero;

            // modify data
            (_baseInfo.getData() as LocalBaseInfo).defenseHeros.Add(cmHero.heroInfo);
            _baseInfo.markDirty();
        }
    }
}
