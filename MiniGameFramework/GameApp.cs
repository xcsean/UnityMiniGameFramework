using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public class GameApp
    {
        protected static GameApp _inst;
        public static GameApp Inst => _inst;

        public static void setInst(GameApp inst)
        {
            _inst = inst;
        }

        protected INetwork _net;
        public INetwork Network => _net;

        protected IFileSystem _file;
        public IFileSystem file => _file;

        protected ConfigManager _conf;
        public ConfigManager conf;

        virtual public bool Init(string appConfigFileName)
        {
            _net = new Network();
            _conf = new ConfigManager();

            GameObjectManager.registerGameObjectComponentCreator("StateComponent", StateComponent.create);

            return true;
        }
    }
}
