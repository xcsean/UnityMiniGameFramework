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

        public bool IsRandomHit(float probability)
        {
            var rate = RandomBetween(0, 10000);
            return rate < probability * 10000;
        }
    }
}
