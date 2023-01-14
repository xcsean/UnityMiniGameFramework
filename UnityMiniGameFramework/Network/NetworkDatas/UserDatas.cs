using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class TempInfo
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class BaseUserInfo
    {
        public string uid { get; set; }

        public int level { get; set; }

        public int gold { get; set; }

        public int meat { get; set; }

        public int gameLevel { get; set; }

        public TempInfo heroLevels { get; set; }

        public TempInfo buildingLevels { get; set; }

        public TempInfo weaponLevels { get; set; }

        public TempInfo weaponPieces { get; set; }

    }
}
