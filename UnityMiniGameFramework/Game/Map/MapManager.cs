using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class MapManager
    {

        protected MapConfig _mapConf;
        public MapConfig MapConf => _mapConf;

        public void Init()
        {
            _mapConf = (MapConfig)UnityGameApp.Inst.Conf.getConfig("maps");
        }
    }
}
