using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public interface IAttribute
    {
        object data { get; }

        string toJson();
        void fromJson(string jsonStr);

        void cloneTo<U>(U toObj);
        void cloneFrom<U>(U fromObj);
    }
}
