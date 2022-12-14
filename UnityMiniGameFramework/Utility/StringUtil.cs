using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    static class StringUtil
    {
        private const int unitDigits = 3;

        private static readonly List<char> units = new List<char>() {'K', 'M'};
        
        public static string StringNumFormat(string str)
        {
            int len = str.Length;
            int index = Math.Min(units.Count, (len - 1) / unitDigits);
            if (index <= 0)
                return str;
            return str.Substring(0, len - index * unitDigits) + units[index - 1];
        }
    }
}
