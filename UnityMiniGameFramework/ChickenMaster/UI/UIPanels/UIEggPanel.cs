using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UIEggPanel : UIPanel
    {
        override public string type => "UIEggPanel";
        public static UIEggPanel create()
        {
            return new UIEggPanel();
        }

        protected Label _recoveryTime;
        public Label RecoveryTime => _recoveryTime;


        protected Button _startBtn;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            //this.unityUIDocument.sortingOrder = -1;

            _recoveryTime = this._uiObjects["RecoverTime"].unityVisualElement as Label;

            _startBtn = this._uiObjects["StartBtn"].unityVisualElement as Button;
            _startBtn.RegisterCallback<MouseUpEvent>(onStartLevelClick);

            _recoveryTime.text = "ready";
        }

        public void onStartLevelClick(MouseUpEvent e)
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var bi = (cmGame.baseInfo.getData() as LocalBaseInfo);

            if(bi.egg.hp <= 0)
            {
                // can't start level
                return;
            }

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
                    _recoveryTime.text = "";
                }
            }
            else if (!UnityGameApp.Inst.MainScene.map.currentLevel.isStarted)
            {
                var level = UnityGameApp.Inst.MainScene.map.CreateLevel(lvlConf.mapLevelName);
                if (level != null)
                {
                    (level as CMShootingLevel).SetDefenseLevelConf(lvlConf, bi.currentLevel);

                    level.Start();
                    _recoveryTime.text = "";
                }
            }
            else
            {
                // level is ongoing
            }
        }

        public void onEggDie()
        {
            // TO DO : disable start button, show quick recover button (ad. button)
        }
        public void onEggRecover()
        {
            _recoveryTime.text = "ready";
            // TO DO : show start button
        }

        public void refreshRecoveryTime(long time)
        {
            var t = new TimeSpan((long)(time * 10000));

            _recoveryTime.text = $"Time: {t.Minutes}:{t.Seconds}";
        }
    }
}
