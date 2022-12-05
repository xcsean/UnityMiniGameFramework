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

        protected UILevelStateControl _levelStateControl;
        public UILevelStateControl levelStateControl => _levelStateControl;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _startLevelBtn = this._uiObjects["StartLevel"].unityVisualElement as Button;
            _startLevelBtn.RegisterCallback<MouseUpEvent>(onStartLevelClick);

            _levelStateControl = this._uiObjects["LevelStates"] as UILevelStateControl;
        }


        public void onStartLevelClick(MouseUpEvent e)
        {
            if(UnityGameApp.Inst.MainScene.map.currentLevel == null)
            {
                var level = UnityGameApp.Inst.MainScene.map.CreateLevel("testLevel");
                if(level != null)
                {
                    level.Start();
                    _showInLevelStatus(true);
                }
            }
            else if(!UnityGameApp.Inst.MainScene.map.currentLevel.isStarted)
            {
                UnityGameApp.Inst.MainScene.map.currentLevel.Start();
                _showInLevelStatus(true);
            }

        }

        protected void _showInLevelStatus(bool bshow)
        {
            if(bshow)
            {
                _levelStateControl.unityVisualElement.style.visibility = Visibility.Visible;
                _startLevelBtn.style.visibility = Visibility.Hidden;
            }
            else
            {
                _levelStateControl.unityVisualElement.style.visibility = Visibility.Hidden;
                _startLevelBtn.style.visibility = Visibility.Visible;
            }
        }

        public override void showUI()
        {
            base.showUI();

            _showInLevelStatus(UnityGameApp.Inst.MainScene.map.currentLevel != null && UnityGameApp.Inst.MainScene.map.currentLevel.isStarted);
        }
    }
}
