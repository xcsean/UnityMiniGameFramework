using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class Randomness : IRandom
    {
        protected Random _rand;

        public Randomness()
        {
            _rand = new Random();
        }

        public int RandomBetween(int begin, int end)
        {
            return _rand.Next(begin, end);
        }
    }
}
