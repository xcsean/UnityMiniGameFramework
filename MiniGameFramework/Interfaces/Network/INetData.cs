using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public interface INetData
    {
        object getData();

        void OnUpdate(object data);
    }
}
