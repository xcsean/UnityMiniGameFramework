using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class CMShootingLevel : MapLevel
    {
        UILevelMainPanel _levelUI;

        public static CMShootingLevel create()
        {
            return new CMShootingLevel();
        }

        public override bool Init(string confName)
        {
            bool ret = base.Init(confName);

            _levelUI = UnityGameApp.Inst.UI.getUIPanel("LevelMainUI") as UILevelMainPanel;

            return ret;
        }

        public override void OnUpdate(float timeElasped)
        {
            base.OnUpdate(timeElasped);

            _levelUI.levelStateControl.timeLeftText.text = this.timeLeft.ToString();
        }
    }
}
