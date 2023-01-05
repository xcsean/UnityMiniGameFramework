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

            try
            {
                string confStr = GameApp.Inst.File.readStringFromStreamPath(filename);

                _conf = _JsonDeserialize(confStr);
            }
            catch(Exception e)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Init JsonConfig {n} from ({filename}) error msg: {e.Message}");
                Debug.DebugOutput(DebugTraceType.DTT_Error, e.StackTrace);
            }
        }

        virtual protected object _JsonDeserialize(string confStr)
        {
            throw new NotImplementedException();
        }
    }
}
