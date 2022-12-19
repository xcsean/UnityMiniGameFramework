using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{

    public class UILevelMainPanel : UIPanel
    {

        override public string type => "UILevelMainPanel";
        public static UILevelMainPanel create()
        {
            return new UILevelMainPanel();
        }

        protected Button _nextLevelBtn;
        protected Button _quitBtn;

        protected UILevelStateControl _levelStateControl;
        public UILevelStateControl levelStateControl => _levelStateControl;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _nextLevelBtn = this._uiObjects["NextLevelBtn"].unityVisualElement as Button;
            _nextLevelBtn.RegisterCallback<MouseUpEvent>(onNextLevelClick);

            _quitBtn = this._uiObjects["QuitBtn"].unityVisualElement as Button;
            _quitBtn.RegisterCallback<MouseUpEvent>(onQuitLevelClick);

            _levelStateControl = this._uiObjects["LevelStates"] as UILevelStateControl;
        }

        public void onNextLevelClick(MouseUpEvent e)
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var bi = (cmGame.baseInfo.getData() as LocalBaseInfo);

            var lvlConf = cmGame.GetCurrentDefenseLevelConf();
            if (lvlConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"GetCurrentDefenseLevelConf [{bi.currentLevel}] not exist");
                return;
            }

            if (UnityGameApp.Inst.MainScene.map.currentLevel == null)
            {
                var level = UnityGameApp.Inst.MainScene.map.CreateLevel(lvlConf.mapLevelName);
                if (level != null)
                {
                    (level as CMShootingLevel).SetDefenseLevelConf(lvlConf, bi.currentLevel);

                    level.Start();
                }
            }
            else if (!UnityGameApp.Inst.MainScene.map.currentLevel.isStarted)
            {
                var level = UnityGameApp.Inst.MainScene.map.CreateLevel(lvlConf.mapLevelName);
                if (level != null)
                {
                    (level as CMShootingLevel).SetDefenseLevelConf(lvlConf, bi.currentLevel);

                    level.Start();
                }
            }
            else
            {
                // level is ongoing
            }
        }
        public void onQuitLevelClick(MouseUpEvent e)
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;

            cmGame.uiLevelMainPanel.hideUI();
            cmGame.uiMainPanel.showUI();

            UnityGameApp.Inst.MainScene.camera.follow(cmGame.Self.mapHero);
        }

    }
}
