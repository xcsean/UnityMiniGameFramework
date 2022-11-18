using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public interface IUIPreloaderPanel : IUIPanel
    {
        void AddInitStep(string stepName);

        void OnInitStep(string stepName);

        void ProgressCurrentInitStep(int percentage, string message);
    }
}
