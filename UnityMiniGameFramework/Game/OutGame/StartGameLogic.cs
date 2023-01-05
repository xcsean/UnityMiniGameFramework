using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class StartGameLogic
    {
        protected UIPanelStartMain _startMainUI;

        public void Init()
        {
            _startMainUI = UnityGameApp.Inst.UI.getUIPanel("startMainUI") as UIPanelStartMain;
        }
    }
}
