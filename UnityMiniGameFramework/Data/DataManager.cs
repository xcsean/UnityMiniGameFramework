﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

        public void DelLocalSaveFile(string dataName)
        {
            string fileName = "/localStorage/" + dataName;
            if (!UnityGameApp.Inst.File.isLocalSaveFileExist(fileName))
            {
                return;
            }
            UnityGameApp.Inst.File.delLocalSaveFile(fileName);
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
        /// <summary>
        /// 改配置开发时使用
        /// </summary>
        public LocalUserConfig localUserConfig => _localUserConfig;
        public LocalUserConfig GetLocalUserConfig()
        {
            if (_localUserConfig == null)
            {
                string fileName = "/StreamingAssets/Config/UserConfig.json";
                if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    // 手动拷贝到手机【内部存储/Android/data/com.ltgames.android.roostergang/files】
                    fileName = "/UserConfig.json";
                }
                if (UnityGameApp.Inst.File.isFileExist(fileName))
                {
                    string jsonStr = UnityGameApp.Inst.File.readStringFrom(fileName);
                    var jsonObj = JsonUtil.FromJson<LocalUserConfig>(jsonStr);
                    if (jsonObj != null)
                    {
                        _localUserConfig = jsonObj;
                    }
                }
                else
                {
                    MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Debug, $"GetLocalUserConfig {fileName} not exist");
                }
            }

            return _localUserConfig;
        }

    }
}
