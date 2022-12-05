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

        public ChickenMasterGame()
        {
            _self = new SelfControl();
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
                    hero = new LocalHeroInfo()
                    {
                        mapHeroName = "testHero",
                        holdWeapon = new LocalWeaponInfo()
                        {
                            id = 1,
                            level = 1,
                        }
                    },
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
            _self.Init();
        }

        public void OnUpdate()
        {
            _self.OnUpdate();
        }
    }
}
