using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public enum CMGNotifyType
    {
        CMG_Notify,
        CMG_ERROR
    }

    public class UIMainPanel : UIPanel
    {
        protected class NotifyMessage
        {
            public CMGNotifyType t;
            public string msg;
            public float timeLeft;
        }

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

        protected Label _TrainTime;
        public Label TrainTime => _TrainTime;

        protected Label _NotifyText;
        public Label NotifyText => _NotifyText;

        protected List<NotifyMessage> _notifyMessages;

        protected VisualElement _clickableArea;

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
            _NotifyText = this._uiObjects["NotifyText"].unityVisualElement as Label;
            _TrainTime = this._uiObjects["TrainTime"].unityVisualElement as Label;
            _clickableArea = this._uiObjects["Clickable"].unityVisualElement;
            //_clickableArea.RegisterCallback<MouseDownEvent>(onMouseDownCA);
            //_clickableArea.RegisterCallback<MouseUpEvent>(onMouseUpCA);

            _LevelInfo.text = "Not Start";
            _NotifyText.text = "";

            _notifyMessages = new List<NotifyMessage>();
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

        public void refreshGold(long gold)
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
        public void refreshTrainTime(long time)
        {
            TimeSpan t = new TimeSpan(time * 10000);
            _TrainTime.text = $"Train -{t.Minutes}:{t.Seconds}";
        }

        public void NofityMessage(CMGNotifyType t, string msg)
        {
            var notify = new NotifyMessage()
            {
                t = t,
                msg = msg,
                timeLeft = 3.0f // 3 seconds
            };

            _notifyMessages.Add(notify);
            if(_notifyMessages.Count > 3)
            {
                _notifyMessages.RemoveAt(0);
            }

            _refreshNotifyMessages();
        }

        protected void _refreshNotifyMessages()
        {
            string msg = "";
            foreach (var n in _notifyMessages)
            {
                switch (n.t)
                {
                    case CMGNotifyType.CMG_Notify:
                        msg += $"<color=#ffffffff>{n.msg}</color><br>"; // TO DO : change color
                        break;
                    case CMGNotifyType.CMG_ERROR:
                        msg += $"<color=#ff0000ff>{n.msg}</color><br>"; // TO DO : change color
                        break;
                }
            }

            _NotifyText.text = msg;
        }


        public void OnUpdate()
        {
            if(_notifyMessages == null)
            {
                return;
            }

            bool changed = false;
            for(int i=0; i< _notifyMessages.Count; ++i)
            {
                var notify = _notifyMessages[i];
                notify.timeLeft -= UnityEngine.Time.deltaTime;

                if(notify.timeLeft <= 0)
                {
                    _notifyMessages.RemoveAt(i);
                    --i;
                    changed = true;
                }
            }

            if(changed)
            {
                _refreshNotifyMessages();
            }
        }
    }
}
