using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class MapManager
    {
        protected Dictionary<string, Func<IMapLevel>> _mapLevelCreator;

        protected MapConfig _mapConf;
        public MapConfig MapConf => _mapConf;

        public MapManager()
        {
            _mapLevelCreator = new Dictionary<string, Func<IMapLevel>>();
        }

        public void Init()
        {
            _mapConf = (MapConfig)UnityGameApp.Inst.Conf.getConfig("maps");
        }

        public void registerMapLevelCreator(string type, Func<IMapLevel> creator)
        {
            if (_mapLevelCreator.ContainsKey(type))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"registerMapLevelCreator ({type}) already exist");
                return;
            }
            _mapLevelCreator[type] = creator;
        }

        public IMapLevel createMapLevel(string type)
        {
            if (_mapLevelCreator.ContainsKey(type))
            {
                return _mapLevelCreator[type]();
            }

            Debug.DebugOutput(DebugTraceType.DTT_Error, $"createMapLevel ({type}) not exist");

            return null;
        }
    }
}
