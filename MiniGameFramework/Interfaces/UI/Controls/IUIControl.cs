using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public interface IUIControl : IUIObject
    {

        void setPosition(int x, int y);
        void setSize(int width, int height);

        // TO DO : add interfaces
    }
}
