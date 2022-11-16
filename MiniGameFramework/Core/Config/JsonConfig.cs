using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public abstract class JsonConfig : IConfig
    {
        protected string _name;
        protected string _file;
        protected object _conf;

        virtual public string type => "JsonConfig";
        public string name => _name;
        public string file => _file;
        public object config => _conf;

        virtual public void Init(string filename, string n)
        {
            _name = n;
            _file = filename;

            Debug.DebugOutput(DebugTraceType.DTT_System, $"Init JsonConfig {n} from ({filename})");
            string confStr = GameApp.Inst.file.readStringFrom(filename);

            _conf = _JsonDeserialize(confStr);
        }

        virtual protected object _JsonDeserialize(string confStr)
        {
            throw new NotImplementedException();
        }
    }
}
