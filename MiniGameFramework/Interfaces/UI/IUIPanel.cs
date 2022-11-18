using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public interface IUIPanel : IUIObject
    {
        IUIObject getUIObject(string name);

        void Init(UIPanelConf conf);
        void Dispose();
    }
}
