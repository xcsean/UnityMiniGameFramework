using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UILevelEntryPanel : UIPanel
    {
        override public string type => "UILevelEntryPanel";
        public static UILevelEntryPanel create()
        {
            return new UILevelEntryPanel();
        }

        protected Button _startLevelBtn;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _startLevelBtn = this._uiObjects["StartLevel"].unityVisualElement as Button;
            _startLevelBtn.RegisterCallback<MouseUpEvent>(onStartLevelClick);
        }


        public void onStartLevelClick(MouseUpEvent e)
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            if(cmGame.uiLevelMainPanel == null)
            {
                cmGame.InitUILevelMainPanel("LevelMainUI");
            }

            cmGame.uiMainPanel.hideUI();
            this.hideUI();
            cmGame.uiLevelMainPanel.showUI();

            if(cmGame.levelCenterObject != null)
            {
                UnityGameApp.Inst.MainScene.camera.follow(cmGame.levelCenterObject);
            }

            _initCMNPCHeros();
        }

        protected void _initCMNPCHeros()
        {
            // for Debug ...
            // give 4 hero
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            if (cmGame.cmNPCHeros.Count <= 0)
            {
                cmGame.AddDefenseHero("Alice");
                cmGame.AddDefenseHero("Bob");
                cmGame.AddDefenseHero("Charlie");
                cmGame.AddDefenseHero("Don");
            }
        }
    }
}
