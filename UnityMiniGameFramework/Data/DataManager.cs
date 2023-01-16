using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class DataManager
    {
        protected LocalUserData _localUserData;
        public LocalUserData localUserData => _localUserData;

        protected NetPackageData _netPackageData;
        public NetPackageData netPackageData => _netPackageData;
        protected NetUserData _netUserData;
        public NetUserData netUserData => _netUserData;

        protected LocalStorageProvider _localDataProvider;
        protected RESTFulDataProvider _restfulDataProvider;

        public void Init()
        {
            _localDataProvider = new LocalStorageProvider("/localStorage/");
            _restfulDataProvider = new RESTFulDataProvider(UnityGameApp.Inst.RESTFulClient);

            _localDataProvider.regDataObjectCreator("user", LocalUserData.create);

            _restfulDataProvider.regDataObjectCreator("user", NetUserData.create);
            _restfulDataProvider.regDataObjectCreator("package", NetPackageData.create);

            _localDataProvider.Init();
            _restfulDataProvider.Init();
        }

        public async Task CreateLocalUserDataAsync()
        {
            _localUserData = _localDataProvider.CreateData("user") as LocalUserData;
            if (_localUserData == null)
            {
                return;
            }
            await _localUserData.initFromProviderAsync();
        }

        public async Task CreateNetUserDataAsync()
        {
            _netUserData = _restfulDataProvider.CreateData("user") as NetUserData;
            if (_netUserData == null)
            {
                return;
            }
            await _netUserData.initFromProviderAsync();
        }

        public async Task CreateNetPackageDataAsync()
        {
            _netPackageData = _restfulDataProvider.CreateData("package") as NetPackageData;
            if (_netPackageData == null)
            {
                return;
            }
            await _netPackageData.initFromProviderAsync();
        }

        private LocalUserConfig _localUserConfig;
        public LocalUserConfig GetLocalUserConfig()
        {
            if (_localUserConfig == null)
            {
                string jsonStr = GameApp.Inst.File.readStringFrom("/StreamingAssets/Config/UserConfig.json");
                var jsonObj = JsonUtil.FromJson<LocalUserConfig>(jsonStr);
                if (jsonObj != null)
                {
                    _localUserConfig = jsonObj;
                }
            }

            return _localUserConfig;
        }

    }
}
