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

        protected CMEgg _egg;
        public CMEgg Egg => _egg;

        protected CMStoreHouse _storeHouse;
        public CMStoreHouse StoreHouse => _storeHouse;

        protected CMTrainStation _trainStation;
        public CMTrainStation TrainStation => _trainStation;


        protected Dictionary<string, CMNPCHeros> _cmNPCHeros;
        public Dictionary<string, CMNPCHeros> cmNPCHeros => _cmNPCHeros;

        protected Dictionary<string, CMFactory> _cmFactories;

        protected IGameObject _levelCenterObject;
        public IGameObject levelCenterObject => _levelCenterObject;

        UIGameMainPanel _uiMainPanel;
        UIDoubleAttackPanel _uiDmgPanel;
        public UIGameMainPanel uiMainPanel => _uiMainPanel;

        UILevelMainPanel _uiLevelMainPanel;
        public UILevelMainPanel uiLevelMainPanel => _uiLevelMainPanel;

        long _lastSaveTime;
        long _autoSaveTime;
        long _offlineAwardMinTime;

        public ChickenMasterGame()
        {
            _self = new SelfControl();
            _egg = new CMEgg();
            _storeHouse = new CMStoreHouse();
            _trainStation = new CMTrainStation();
            _cmNPCHeros = new Dictionary<string, CMNPCHeros>();
            _cmFactories = new Dictionary<string, CMFactory>();
        }

        public async Task InitAsync()
        {
            _gameConf = UnityGameApp.Inst.Conf.getConfig("cmgame") as CMGameConfig;

            if(_gameConf.gameConfs.autoSaveTime <= 0)
            {
                _autoSaveTime = 10000; // auto save per 10 sec 
            }
            else
            {
                _autoSaveTime = (long)(_gameConf.gameConfs.autoSaveTime * 1000);
            }

            if(_gameConf.gameConfs.offlineAwardMinTime <= 0)
            {
                _offlineAwardMinTime = 60000; // offline award min time
            }
            else
            {
                _offlineAwardMinTime = (long)(_gameConf.gameConfs.offlineAwardMinTime * 1000);
            }

            long nowMillisecond = (long)(DateTime.Now.Ticks / 10000);

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
                    lastLoginTime = nowMillisecond,
                    lastOnlineTime = nowMillisecond
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
                    currentLevel = 1,
                    selfHero = new LocalHeroInfo()
                    {
                        mapHeroName = "testHero",
                        holdWeaponId = 0,
                    },
                    egg = new LocalEggInfo()
                    {
                        hp = _gameConf.gameConfs.eggConf.maxHp,
                        lastIncHpTime = nowMillisecond,
                        nextRecoverTime = 0
                    },
                    defenseHeros = new List<LocalHeroInfo>(),
                    weapons = new Dictionary<int, LocalWeaponInfo>(),
                    factories = new List<LocalFactoryInfo>(),
                    backPackProds = new List<LocalPackProductInfo>(),
                    backPackItems = new List<LocalItemInfo>(),
                    storeHouse = new LocalStoreHouseInfo()
                    {
                        storeProductName = _gameConf.gameConfs.storeHouseConf.storeProductName,
                        storeCount = 0,
                        level = 1,
                        storeHouseWorkers = new List<LocalWorkerInfo>(), // init in CMWorker
                    },
                    trainStation = new LocalTrainStationInfo()
                    {
                        storeProducts = new List<LocalPackProductInfo>(),
                        level = 1,
                        NextTrainArrivalTime = nowMillisecond,
                        trainStationWorkers = new List<LocalWorkerInfo>(), // init in CMWorker
                    }
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

            _lastSaveTime = nowMillisecond;
        }

        public void OnStartSceneLoaded()
        {
        }

        public void OnMainSceneLoaded()
        {
            // init ui
            _uiMainPanel = UnityGameApp.Inst.UI.getUIPanel("GameMainUI") as UIGameMainPanel;

            // init self
            _self.Init();

            // init npc heros
            var bi = _baseInfo.getData() as LocalBaseInfo;
            for (int i = 0; i < bi.defenseHeros.Count; ++i)
            {
                if(bi.defenseHeros[i] == null)
                {
                    continue;
                }

                // create hero
                var cmHero = new CMNPCHeros();
                cmHero.Init(bi.defenseHeros[i]);

                _cmNPCHeros[cmHero.heroInfo.mapHeroName] = cmHero;
            }

            // init factories
            for(int i=0;i< bi.factories.Count; ++i)
            {
                if (bi.factories[i] == null)
                {
                    continue;
                }

                // create factory
                CMFactory fac = new CMFactory();
                if(fac.Init(bi.factories[i]))
                {
                    _cmFactories[fac.factoryName] = fac;
                }
            }

            // init egg
            _egg.Init(bi.egg);

            // init storehouse
            _storeHouse.Init(bi.storeHouse);

            // init train station
            _trainStation.Init(bi.trainStation);

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

            _checkOfflineAwards();

            // refresh main ui Info
            _uiMainPanel.refreshAll();
        }

        protected void _checkOfflineAwards()
        {
            // check offline award
            long nowMillisecond = (long)(DateTime.Now.Ticks / 10000);
            var userInfo = (_userInfo.getData() as LocalUserInfo);
            var bi = _baseInfo.getData() as LocalBaseInfo;

            var offLineMillisecond = nowMillisecond - userInfo.lastOnlineTime;
            if (offLineMillisecond < _offlineAwardMinTime)
            {
                return;
            }

            CMOfflineAwardConf offlineAwardConf = null;
            _gameConf.gameConfs.offlineAwardsByUserLevel.TryGetValue(bi.level, out offlineAwardConf);

            if (offlineAwardConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"user level [{bi.level}] offline award config not exist");
                return;
            }

            if (bi.unfetchedOfflineAward == null)
            {
                bi.unfetchedOfflineAward = new LocalAwardInfo()
                {
                    gold = 0,
                    exp = 0,
                    items = new Dictionary<string, int>(),
                    products = new Dictionary<string, int>()
                };
            }

            //bi.unfetchedOfflineAward.gold += (int)(offlineAwardConf.goldPerSec * offLineMillisecond / 1000);
            //bi.unfetchedOfflineAward.exp += (int)(offlineAwardConf.expPerSec * offLineMillisecond / 1000);

            //foreach(var itemAwd in offlineAwardConf.items)
            //{
            //    int count = (int)(itemAwd.countPerSec * offLineMillisecond / 1000);
            //    if(bi.unfetchedOfflineAward.items.ContainsKey(itemAwd.itemName))
            //    {
            //        bi.unfetchedOfflineAward.items[itemAwd.itemName] += count;
            //    }
            //    else
            //    {
            //        bi.unfetchedOfflineAward.items[itemAwd.itemName] = count;
            //    }
            //}
            //foreach (var prodAwd in offlineAwardConf.products)
            //{
            //    int count = (int)(prodAwd.countPerSec * offLineMillisecond / 1000);
            //    if (bi.unfetchedOfflineAward.products.ContainsKey(prodAwd.productName))
            //    {
            //        bi.unfetchedOfflineAward.products[prodAwd.productName] += count;
            //    }
            //    else
            //    {
            //        bi.unfetchedOfflineAward.products[prodAwd.productName] = count;
            //    }
            //}

            LocalAwardInfo offlineAward = new LocalAwardInfo()
            {
                gold = (int)(offlineAwardConf.goldPerSec * offLineMillisecond / 1000),
                exp = (int)(offlineAwardConf.expPerSec * offLineMillisecond / 1000),
                items = new Dictionary<string, int>(),
                products = new Dictionary<string, int>()
            };

            foreach (var itemAwd in offlineAwardConf.items)
            {
                int count = (int)(itemAwd.countPerSec * offLineMillisecond / 1000);
                if (offlineAward.items.ContainsKey(itemAwd.itemName))
                {
                    offlineAward.items[itemAwd.itemName] += count;
                }
                else
                {
                    offlineAward.items[itemAwd.itemName] = count;
                }
            }
            foreach (var prodAwd in offlineAwardConf.products)
            {
                int count = (int)(prodAwd.countPerSec * offLineMillisecond / 1000);
                if (offlineAward.products.ContainsKey(prodAwd.productName))
                {
                    offlineAward.products[prodAwd.productName] += count;
                }
                else
                {
                    offlineAward.products[prodAwd.productName] = count;
                }
            }

            //// TO DO : show offline award ui
            UIOfflineRewardPanel _ui = UnityGameApp.Inst.UI.createUIPanel("OfflineRewardUI") as UIOfflineRewardPanel;
            _ui.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.uiRootObject).unityGameObject.transform);
            _ui.showReward(offlineAward, offLineMillisecond);

            userInfo.lastOnlineTime = nowMillisecond;

            _baseInfo.markDirty();
            _userInfo.markDirty();
        }

        public void OnUpdate()
        {
            _self.OnUpdate();

            foreach(var fac in _cmFactories)
            {
                fac.Value.OnUpdate();
            }

            _egg.OnUpdate();

            _storeHouse.OnUpdate();

            _trainStation.OnUpdate();

            if(_uiMainPanel != null)
            {
                _uiMainPanel.refreshTrainTime(_trainStation.train.timeToTrainArrival);

                _uiMainPanel.OnUpdate();
            }

            //_uiDmgPanel = UnityGameApp.Inst.UI.getUIPanel("DoubleAttackUI") as UIDoubleAttackPanel;
            //if(_uiDmgPanel != null)
            //{
            //    var bi = _baseInfo.getData() as LocalBaseInfo;
            //    long nowMillisecond = (long)(DateTime.Now.Ticks);
            //    int time = (int)(bi.buffs.DoubleDmgBuff - nowMillisecond) / 1000;
            //    _uiDmgPanel.setBuffTime(time);
            //}

            if (_userInfo != null)
            {
                long nowMillisecond = (long)(DateTime.Now.Ticks / 10000);

                if (nowMillisecond - _lastSaveTime >= _gameConf.gameConfs.autoSaveTime)
                {
                    _lastSaveTime = nowMillisecond;

                    (_userInfo.getData() as LocalUserInfo).lastOnlineTime = nowMillisecond;
                    _userInfo.markDirty();

                    UnityGameApp.Inst.Datas.localUserData.writeBack();
                }
            }
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
        public Dictionary<string, CMNPCHeros> GetDefAreaHeros()
        {
            Dictionary<string, CMNPCHeros> ret = new Dictionary<string, CMNPCHeros>();
            foreach (var pair in _cmNPCHeros)
            {
                ret[pair.Value.heroInfo.defAreaName] = pair.Value;
            }
            return ret;
        }

        public CMNPCHeros AddDefenseHero(string mapHeroName)
        {
            var heroConf = _gameConf.getCMHeroConf(mapHeroName);
            if(heroConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"AddDefenseHero [{mapHeroName}] config not exist");
                return null;
            }

            LocalHeroInfo heroInfo = new LocalHeroInfo()
            {
                mapHeroName = mapHeroName,
                level = 1,
                position = null,
                holdWeaponId = 0,
            };

            var cmHero = new CMNPCHeros();
            cmHero.Init(heroInfo);

            _cmNPCHeros[cmHero.heroInfo.mapHeroName] = cmHero;

            // modify data
            (_baseInfo.getData() as LocalBaseInfo).defenseHeros.Add(cmHero.heroInfo);
            _baseInfo.markDirty();

            return cmHero;
        }

        public LocalWeaponInfo GetWeaponInfo(int weaponId)
        {
            LocalWeaponInfo info = null;
            var bi = (_baseInfo.getData() as LocalBaseInfo);
            bi.weapons.TryGetValue(weaponId, out info);
            return info;
        }
        public LocalWeaponInfo AddNewWeaponInfo(int weaponId)
        {
            var bi = (_baseInfo.getData() as LocalBaseInfo);
            if (bi.weapons.ContainsKey(weaponId))
            {
                return bi.weapons[weaponId];
            }

            var info = new LocalWeaponInfo()
            {
                id = weaponId,
                level = 1,
            };
            bi.weapons[weaponId] = info;
            return info;
        }

        public CMFactory GetFactory(string factoryName)
        {
            CMFactory fac = null;
            _cmFactories.TryGetValue(factoryName, out fac);
            return fac;
        }

        public List<CMFactory> GetFactories()
        {
            return _cmFactories.Values.ToList();
        }

        public CMFactory AddFactory(string factoryName)
        {
            CMFactory fac = new CMFactory();

            // new Factory
            var localFacInfo = new LocalFactoryInfo()
            {
                mapBuildName = factoryName,
                level = 1,
                buildingInputProduct = null,
                buildingOutputProduct = null
            };

            if (!fac.Init(localFacInfo))
            {
                return null;
            }

            _cmFactories[factoryName] = fac;

            // modify data
            (_baseInfo.getData() as LocalBaseInfo).factories.Add(fac.localFacInfo);
            _baseInfo.markDirty();

            // update worker factory
            _storeHouse.SyncWorkerFactories();
            _trainStation.SyncWorkerFactories();

            return fac;
        }

        public CMDefenseLevelConf GetCurrentDefenseLevelConf()
        {
            var bi = (_baseInfo.getData() as LocalBaseInfo);
            foreach(var lvlConf in _gameConf.gameConfs.defenseLevels)
            {
                if (bi.currentLevel >= lvlConf.levelRangeMin && bi.currentLevel <= lvlConf.levelRangeMax)
                {
                    return lvlConf;
                }
            }

            return null;
        }
    }
}
