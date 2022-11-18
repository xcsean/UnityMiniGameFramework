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

        void bindEventAction(uint eventCode, Action<object> eventAction);
        void bindEventAction<T>(uint eventCode, Action<T> eventAction);

        void showUI();
        void hideUI();
        // TO DO : add interfaces
    }
}
