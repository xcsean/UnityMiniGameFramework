using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public interface IConfig
    {
        string type { get; }
        string name { get; }
        string file { get; }
        object config { get; }
        
        void Init(string filename, string name);
    }

    public interface IConfigReader
    {
        object getConfig(string key);
    }
}
