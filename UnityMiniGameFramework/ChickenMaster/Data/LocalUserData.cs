using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class LocalPackProductInfo
    {
        public string productName { get; set; }
        public int count { get; set; }
    }

    public class LocalFactoryInfo
    {
        public string mapBuildName { get; set; }
        public int level { get; set; }

        public LocalPackProductInfo buildingInputProduct { get; set; }
        public LocalPackProductInfo buildingOutputProduct { get; set; }
    }

    public class LocalWeaponInfo
    {
        public int id { get; set; }
        public int level { get; set; }
    }

    public class LocalHeroInfo
    {
        public string mapHeroName { get; set; }
        public string defAreaName { get; set; }
        public int level { get; set; }
        public JsonConfVector3 position { get; set; }
        public int holdWeaponId { get; set; }
    }

    public class LocalEggInfo
    {
        public int hp { get; set; }

        public long lastIncHpTime { get; set; }
        public long nextRecoverTime { get; set; }
    }

    public class LocalItemInfo
    {
        public string itemName { get; set; }
        public int count { get; set; }
    }

    public class LocalWorkerInfo
    {
        public JsonConfVector3 position { get; set; }

        public List<LocalPackProductInfo> carryProducts { get; set; }
    }

    public class LocalStoreHouseInfo
    {
        public string storeProductName { get; set; }
        public int storeCount { get; set; }
        public int level { get; set; }

        public List<LocalWorkerInfo> storeHouseWorkers { get; set; }
    }

    public class LocalTrainStationInfo
    {
        public List<LocalPackProductInfo> storeProducts { get; set; }
        public int level { get; set; }

        public long NextTrainArrivalTime { get; set; }

        public List<LocalWorkerInfo> trainStationWorkers { get; set; }
    }

    public class LocalAwardInfo
    {
        public int exp { get; set; }
        public int gold { get; set; }

        public Dictionary<string, int> products { get; set; }
        public Dictionary<string, int> items { get; set; }
    }

    public class LocalBuffInfo
    {
        public long doubleExp { get; set; }
        public long doubleAtk { get; set; }
        public long trainProterSpeed { get; set; }
        public long storehouseProterSpeed { get; set; }
        public long factory1Productivity { get; set; }
        public long factory2Productivity { get; set; }
        public long factory3Productivity { get; set; }
        public long factory4Productivity { get; set; }
        public long factory5Productivity { get; set; }
        public long factory6Productivity { get; set; }
    }

    public class LocalBaseInfo
    {
        public int gold { get; set; }
        public int level { get; set; }
        public int exp { get; set; }

        public int currentLevel { get; set; }
        public int currentFetchedAwardLevel { get; set; }

        public LocalAwardInfo unfetchedOfflineAward { get; set; }

        public LocalHeroInfo selfHero { get; set; }

        public LocalEggInfo egg { get; set; }

        public List<LocalHeroInfo> defenseHeros { get; set; }

        public Dictionary<int, LocalWeaponInfo> weapons { get; set; }

        public List<LocalFactoryInfo> factories { get; set; }
        public List<LocalPackProductInfo> backPackProds { get; set; }

        public List<LocalItemInfo> backPackItems { get; set; }

        public LocalStoreHouseInfo storeHouse { get; set; }

        public LocalTrainStationInfo trainStation { get; set; }

        public LocalBuffInfo buffs { get; set; }
    }

    public class LocalUserInfo
    {
        public string uid { get; set; }
        public string uuid { get; set; }
        public long lastLoginTime { get; set; }
        public long lastOnlineTime { get; set; }
    }

    public class LocalUserData : Data
    {
        public static LocalUserData create()
        {
            return new LocalUserData();
        }

        public LocalStorageProvider localProvider => (LocalStorageProvider)_provider;

        protected static readonly List<string> _initKeys = new List<string>() { "baseInfo", "userInfo" };
        override public List<string> initKeys => _initKeys;

        public LocalUserData()
        {

        }

        override public void writeBack()
        {
            base.writeBack();

            localProvider.writeFile(this.name);
        }

        override public async Task writeBackAsync()
        {
            await base.writeBackAsync();
            
            localProvider.writeFile(this.name);
        }
    }
}
