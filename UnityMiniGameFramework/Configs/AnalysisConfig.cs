using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class AnalysicPointConf
    {
        public string point { get; set; }
    }

    [Serializable]
    public class AnalysisConfs
    {
        public Dictionary<string, AnalysicPointConf> points { get; set; }
    }

    public class AnalysisConfig: JsonConfig
    {
        override public string type => "AnalysisConfig";
        public static AnalysisConfig create()
        {
            return new AnalysisConfig();
        }

        public AnalysisConfs analysisConf => (AnalysisConfs)_conf;

        override protected object _JsonDeserialize(string confStr)
        {
            return JsonUtil.FromJson<AnalysisConfs>(confStr);
        }

        public AnalysicPointConf getPointsConfig(string name)
        {
            if (analysisConf.points == null || !analysisConf.points.ContainsKey(name))
            {
                return null;
            }
            return analysisConf.points[name];
        }
    }
}
