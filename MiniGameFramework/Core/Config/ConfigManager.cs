using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

using System.Collections.Concurrent;

namespace MiniGameFramework
{
    public class ConfigFileField
    {
        public string filename { get; set; }
        public string type { get; set; }
    }

    public class AppConfigs
    {
        public Dictionary<string, ConfigFileField> configFiles { get; set; }
    }

    public class ConfigManager
    {
        protected ConcurrentDictionary<string, Func<IConfig>> _configCreator = new ConcurrentDictionary<string, Func<IConfig>>();
        protected ConcurrentDictionary<string, IConfig> _configs = new ConcurrentDictionary<string, IConfig>();

        public void regConfigCreator(string configType, Func<IConfig> creator)
        {
            Debug.DebugOutput(DebugTraceType.DTT_System, $"register config creator ({configType})");

            _configCreator[configType] = creator;
        }

        public IConfig createConfig(string configType)
        {
            if(!_configCreator.ContainsKey(configType))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"createConfig ({configType}) not exist");

                return null;
            }

            return _configCreator[configType]();
        }

        public void InitAppConfig(string appConfigName)
        {
            Debug.DebugOutput(DebugTraceType.DTT_System, $"Init App Config from ({appConfigName})");
            string confStr = GameApp.Inst.File.readStringFrom(appConfigName);

            AppConfigs appConfs = JsonSerializer.Deserialize<AppConfigs>(confStr);

            foreach(var conf in appConfs.configFiles)
            {
                IConfig c = this.createConfig(conf.Value.type);
                if(c ==null)
                {
                    continue;
                }

                c.Init(conf.Value.filename, conf.Key);
                _configs[conf.Key] = c;
            }
        }

        public IConfig getConfig(string configName)
        {
            if(!_configs.ContainsKey(configName))
            {
                return null;
            }
            return _configs[configName];
        }
    }
}
