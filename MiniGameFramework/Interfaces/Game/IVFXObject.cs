using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public interface IVFXObject
    {
        string name { get; }
        string type { get; }

        void Play();
    }
}
