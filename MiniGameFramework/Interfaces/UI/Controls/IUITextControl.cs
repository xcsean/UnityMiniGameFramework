using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework.Interfaces.UI.Controls
{
    public interface IUITextControl : IUIControl
    {
        string textContent { get; set; }

        // TO DO : add interfaces
    }
}
