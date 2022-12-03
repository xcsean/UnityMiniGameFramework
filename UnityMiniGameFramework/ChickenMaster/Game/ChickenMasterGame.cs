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
        protected LocalUserInfo _userInfo;
        public LocalUserInfo userInfo => _userInfo;

        protected LocalBaseInfo _baseInfo;
        public LocalBaseInfo baseInfo => _baseInfo;

        protected CMGameConfig _gameConf;
        public CMGameConfig gameConf => _gameConf;

        public async void Init()
        {
            _gameConf = UnityGameApp.Inst.Conf.getConfig("cmgame") as CMGameConfig;

            await UnityGameApp.Inst.Datas.CreateLocalUserDataAsync();

            _userInfo = UnityGameApp.Inst.Datas.localUserData.getData("userInfo") as LocalUserInfo;
            if (_userInfo == null)
            {
                // new user info
                _userInfo = new LocalUserInfo()
                {

                };

                UnityGameApp.Inst.Datas.localUserData.modifyData("userInfo", _userInfo);
            }

            _baseInfo = UnityGameApp.Inst.Datas.localUserData.getData("baseInfo") as LocalBaseInfo;
            if (_baseInfo == null)
            {
                // new base info
                _baseInfo = new LocalBaseInfo()
                {

                };

                UnityGameApp.Inst.Datas.localUserData.modifyData("baseInfo", _baseInfo);
            }

            if(UnityGameApp.Inst.Datas.localUserData.isDirty)
            {
                // write back
                await UnityGameApp.Inst.Datas.localUserData.writeBackAsync();
            }



        }

        public void OnUpdate()
        {
            
        }
    }
}
