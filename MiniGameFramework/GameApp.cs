using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public class GameApp
    {
        private static INetwork _net;

        public static INetwork Network => _net;

        public static bool Init()
        {
            _net = new Network();

            return true;
        }
    }
}
