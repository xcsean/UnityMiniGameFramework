using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public struct AsyncOpStatus
    {
        public bool progressing;
        public bool done;
        public uint percentage;

        //public event Action<AsyncOpStatus> completed;
    }
}
