using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public interface IUIObject
    {
        string name { get; }
        string type { get; }

        int x { get; }
        int y { get; }

        int width { get; }
        int height { get; }

        void setPoisition(float x, float y);

        void showUI();
        void hideUI();
        void display(bool b);

        // TO DO : add interfaces
    }
}
