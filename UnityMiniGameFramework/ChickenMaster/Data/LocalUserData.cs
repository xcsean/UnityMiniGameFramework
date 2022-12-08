using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class LocalPackItemInfo
    {
        public int itemId { get; set; }
        public int count { get; set; }
    }

    public class LocalBuildingInfo
    {
        public int id { get; set; }
        public int level { get; set; }

        public List<LocalPackItemInfo> buildingInputItems { get; set; }
        public List<LocalPackItemInfo> buildingOutputItems { get; set; }
    }

    public class LocalWeaponInfo
    {
        public int id { get; set; }
        public int level { get; set; }
    }

    public class LocalHeroInfo
    {
        public string mapHeroName { get; set; }
        public JsonConfVector3 position { get; set; }
        public LocalWeaponInfo holdWeapon { get; set; }
    }

    public class LocalBaseInfo
    {
        public int gold { get; set; }
        public int level { get; set; }
        public int exp { get; set; }

        public LocalHeroInfo selfHero { get; set; }

        public List<LocalHeroInfo> defenseHeros { get; set; }

        public List<LocalBuildingInfo> buildings { get; set; }
        public List<LocalWeaponInfo> weapons { get; set; }

        public List<LocalPackItemInfo> backPackItems { get; set; }
    }

    public class LocalUserInfo
    {
        public string uid { get; set; }
        public string uuid { get; set; }
        public int lastLoginTime { get; set; }
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
