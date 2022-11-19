using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class CharacterManager
    {

        protected CharacterConfigs _charConf;
        public CharacterConfigs CharacterConfs => _charConf;

        public void Init()
        {
            _charConf = (CharacterConfigs)UnityGameApp.Inst.Conf.getConfig("characters");
        }
        
    }
}
