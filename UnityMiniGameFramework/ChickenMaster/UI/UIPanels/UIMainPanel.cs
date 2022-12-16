using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UIMainPanel : UIPanel
    {
        override public string type => "UIMainPanel";
        public static UIMainPanel create()
        {
            return new UIMainPanel();
        }

        protected VisualElement _joystickArea;
        protected UIJoyStickControl _joystick;
        public UIJoyStickControl Joystick => _joystick;

        protected Label _meatNum;
        public Label meatNum => _meatNum;

        protected Label _goldNum;
        public Label goldNum => _goldNum;

        protected Label _level;
        public Label level => _level;

        protected Label _exp;
        public Label exp => _exp;

        protected Label _CurrentLevel;
        public Label CurrentLevel => _CurrentLevel;

        protected Label _LevelInfo;
        public Label LevelInfo => _LevelInfo;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _joystick = this._uiObjects["JoyStick"] as UIJoyStickControl;
            _meatNum = this._uiObjects["MeatNum"].unityVisualElement as Label;
            _goldNum = this._uiObjects["GoldNum"].unityVisualElement as Label;
            _level = this._uiObjects["Level"].unityVisualElement as Label;
            _exp = this._uiObjects["Exp"].unityVisualElement as Label;
            _CurrentLevel = this._uiObjects["CurrentLevel"].unityVisualElement as Label;
            _LevelInfo = this._uiObjects["LevelInfo"].unityVisualElement as Label;

            _LevelInfo.text = "Not Start";
        }

        public void refreshAll()
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var baseInfo = cmGame.baseInfo.getData() as LocalBaseInfo;
            refreshGold(baseInfo.gold);
            refreshLevel(baseInfo.level);
            refreshExp(baseInfo.exp, cmGame.gameConf.getLevelUpExpRequire(baseInfo.level));
            refreshCurrentLevel(baseInfo.currentLevel);
            refreshMeat();
        }

        public void refreshGold(int gold)
        {
            _goldNum.text = $"Gold:{gold}";
        }
        public void refreshLevel(int level)
        {
            _level.text = $"Lv:{level}";
        }
        public void refreshExp(int exp, int nextLevelExp)
        {
            _exp.text = $"Exp:{exp}/{nextLevelExp}";
        }
        public void refreshCurrentLevel(int currentLevel)
        {
            _CurrentLevel.text = $"- {currentLevel} -";
        }
        public void refreshLevelInfo(CMShootingLevel lvl)
        {
            //DateTime t = new DateTime((long)(lvl.timeLeft * 1000));
            var t = new TimeSpan((long)(lvl.timeLeft * 10000000));
            string info = $"Time: {t.Minutes}:{t.Seconds}:{t.Milliseconds}";
            foreach(var kmPair in lvl.kmCount)
            {
                info += $"\r\n{kmPair.Key} : {kmPair.Value}";
            }

            _LevelInfo.text = info;
        }

        public void refreshMeat()
        {
            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            var meatInfo = cmGame.Self.GetBackpackProductInfo("meat");
            if (meatInfo != null)
            {
                _meatNum.text = $"Meat: {meatInfo.count}";
            }
        }
    }
}
