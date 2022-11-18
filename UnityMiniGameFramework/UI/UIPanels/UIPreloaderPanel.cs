using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class UIPreloaderPanel : UIPanel, IUIPreloaderPanel
    {
        override public string type => "UIPreloaderPanel";
        public static UIPreloaderPanel create()
        {
            return new UIPreloaderPanel();
        }

        virtual public void AddInitStep(string stepName)
        {
            throw new NotImplementedException();
        }

        virtual public void OnInitStep(string stepName)
        {
            throw new NotImplementedException();
        }

        virtual public void ProgressCurrentInitStep(int percentage, string message)
        {
            throw new NotImplementedException();
        }
    }
}
