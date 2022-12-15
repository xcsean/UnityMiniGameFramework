using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class CMEgg
    {
        protected LocalEggInfo _eggInfo;


        public void Init(LocalEggInfo eggInfo)
        {
            _eggInfo = eggInfo;

        }
        public void OnUpdate()
        {
        }
    }
}
