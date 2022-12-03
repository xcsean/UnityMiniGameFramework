using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class NetPackageData : Data
    {
        public static NetPackageData create()
        {
            return new NetPackageData();
        }

        public NetPackageData()
        {

        }
    }
}
