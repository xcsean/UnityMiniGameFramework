using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{

    public class CMMutexPopPanels
    {

        private List<UIPopupPanel> panels = new List<UIPopupPanel>();
        public bool haveExPanel = false;
        public void addUI(UIPopupPanel ui)
        {
            if (!panels.Contains(ui))
            {
                panels.Add(ui);
            }

            hideAllUI();
        }

        public void removeUI(UIPopupPanel ui)
        {
            if (panels.Contains(ui))
            {
                panels.Remove(ui);
            }

            reshowAllUI();
        }

        public void hideAllUI()
        {
            int exIndex = -1;
            for (int i = panels.Count - 1; i >= 0; i--)
            {
                var ui = panels[i];
                if (exIndex == -1 && ui.mutex)
                {
                    exIndex = i;
                }
            }

            haveExPanel = exIndex != -1;
            if (exIndex != -1)
            {
                for (int i = 0; i < panels.Count; i++)
                {
                    var ui = panels[i];
                    ui.display(i == exIndex);
                }
            }
            else
            {
                foreach (var ui in panels)
                {
                    ui.display(true);
                }
            }
        }

        public void reshowAllUI()
        {
            int exIndex = -1;
            for (int i = panels.Count - 1; i >= 0; i--)
            {
                var ui = panels[i];

                if (exIndex == -1 && ui.mutex)
                {
                    exIndex = i;
                }
            }

            haveExPanel = exIndex != -1;
            if (exIndex != -1)
            {
                panels[exIndex].display(true);
            }
            else
            {
                foreach (var ui in panels)
                {
                    ui.display(true);
                }
            }
        }
    }
}
