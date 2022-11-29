using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class LocalUserData : Data
    {
        public LocalUserData(string n, LocalStorageProvider p) : base(n, p)
        {

        }
    }
}
