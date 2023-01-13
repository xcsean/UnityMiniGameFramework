using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

using UnityEngine;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    /// <summary>
    /// 战斗主界面
    /// </summary>
    public class UIGameMainPanel : UIPanel
    {
        protected class NotifyMessage
        {
            public CMGNotifyType t;
            public string msg;
            public float timeLeft;
        }
        public class FlyNumParam
        {
            public int index;
            public int num;
            public string type;
            public VisualElement obj;
        }

        override public string type => "UIGameMainPanel";
        public static UIGameMainPanel create()
        {
            return new UIGameMainPanel();
        }

        protected UIJoyStickPanel _joystick;
        public UIJoyStickPanel Joystick => _joystick;

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
        public VisualElement _NotifyBg;

        protected Button _btnUseSkill;
        protected Button _btnDoubleExp;
        protected Button _btnDoubleAtt;
        protected Button _btnSetting;
        protected VisualElement _levelsNodes;
        protected VisualElement _bossInfo;
        protected VisualElement _battleStartInfo;
        protected VisualElement _expBar;

        protected float _expBarWidth = 72f;

        protected VisualElement _clickableArea;
        protected VisualTreeAsset vts;

        protected List<NotifyMessage> _notifyMessages;
        // TO DO 做成一个通用组件

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            // 摇杆单独拆为一个界面
            _joystick = UnityGameApp.Inst.UI.createUIPanel("JoyStickUI") as UIJoyStickPanel;
            _joystick.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.uiRootObject).unityGameObject.transform);
            _joystick.showUI();
            //_joystick = this._uiObjects["JoyStick"] as UIJoyStickControl;

            UIMaskPanel mask = UnityGameApp.Inst.UI.getMaskUI() as UIMaskPanel;
            mask.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.uiRootObject).unityGameObject.transform);
            mask.hideUI();

            _meatNum = this._uiObjects["MeatNum"].unityVisualElement as Label;
            _goldNum = this._uiObjects["GoldNum"].unityVisualElement as Label;
            _level = this._uiObjects["Level"].unityVisualElement as Label;
            _exp = this._uiObjects["Exp"].unityVisualElement as Label;
            _CurrentLevel = this._uiObjects["CurrentLevel"].unityVisualElement as Label;
            _LevelInfo = this._uiObjects["LevelInfo"].unityVisualElement as Label;
            _NotifyText = this._uiObjects["NotifyText"].unityVisualElement as Label;
            _TrainTime = this._uiObjects["TrainTime"].unityVisualElement as Label;
            _clickableArea = this._uiObjects["Clickable"].unityVisualElement;
            _expBar = this._uiObjects["ExpBar"].unityVisualElement;
            _NotifyBg = this._uiObjects["NotifyBg"].unityVisualElement;
            _btnUseSkill = this._uiObjects["BtnUseSkill"].unityVisualElement as Button;
            _btnDoubleExp = this._uiObjects["BtnDoubleExp"].unityVisualElement as Button;
            _btnDoubleAtt = this._uiObjects["BtnDoubleAtt"].unityVisualElement as Button;
            _btnSetting = this._uiObjects["BtnSetting"].unityVisualElement as Button;
            _levelsNodes = this._uiObjects["LevelsNodes"].unityVisualElement;
            _bossInfo = this._uiObjects["BossInfo"].unityVisualElement;
            _battleStartInfo = this._uiObjects["BattleStartInfo"].unityVisualElement;

            _btnUseSkill.clicked += OnUseSkillBtnClick;
            _btnDoubleExp.clicked += OnDoubleExpBtnClick;
            _btnDoubleAtt.clicked += OnDoubleAttBtnClick;
            _btnSetting.clicked += OnSettiingBtnClick;

            // TODO 动态获取一直是0
            //_expBarWidth = _expBar.style.width.value.value;


            _LevelInfo.text = "Not Start";
            _NotifyText.text = "";
            _NotifyBg.style.display = DisplayStyle.None;

            _notifyMessages = new List<NotifyMessage>();
            vts = Resources.Load<VisualTreeAsset>("UI/Controls/FlyNumIcon");

            ShowBattleStartInfo(false);
        }

        /// <summary>
        /// 使用技能
        /// </summary>
        protected void OnUseSkillBtnClick()
        {
            // 此处需要判断技能次数： 有则释放技能，无则打开视频获取技能界面
            UIGetSkillPanel _ui = UnityGameApp.Inst.UI.createUIPanel("GetSkillUI") as UIGetSkillPanel;
            _ui.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.uiRootObject).unityGameObject.transform);
            _ui.showUI();
        }

        /// <summary>
        /// 双倍经验
        /// </summary>
        protected void OnDoubleExpBtnClick()
        {
            UIDoubleExpPanel _ui = UnityGameApp.Inst.UI.createUIPanel("DoubleExpUI") as UIDoubleExpPanel;
            _ui.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.uiRootObject).unityGameObject.transform);
            _ui.showUI();
        }

        /// <summary>
        /// 双倍攻击
        /// </summary>
        protected void OnDoubleAttBtnClick()
        {
            UIDoubleAttackPanel _ui = UnityGameApp.Inst.UI.createUIPanel("DoubleAttackUI") as UIDoubleAttackPanel;
            _ui.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.uiRootObject).unityGameObject.transform);
            _ui.showUI();
        }

        /// <summary>
        /// 设置
        /// </summary>
        protected void OnSettiingBtnClick()
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.Self.AddGold(10);
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
            _goldNum.text = StringUtil.StringNumFormat($"{gold}");
        }
        public void refreshLevel(int level)
        {
            _level.text = $"Lv.{level}";
        }
        public void refreshExp(int exp, int nextLevelExp)
        {
            //_exp.text = $"Exp:{exp}/{nextLevelExp}";
            //_exp_ProgressBar.title = $"{exp}/{nextLevelExp}";
            _expBar.style.width = new StyleLength(new Length(_expBarWidth * exp / nextLevelExp));
        }

        private List<FlyNumParam> flyAnis = new List<FlyNumParam>() { };
        public void addGold(int n)
        {
            if (n != 0)
            {
                showAni("gold", n);
            }
        }

        public void addMeat(int n)
        {
            if (n != 0)
            {
                showAni("meat", n);
            }
        }

        public void showAni(string type, int n)
        {
            if (flyAnis.Count == 0)
            {
                var tx1 = ((UnityResourceManager)UnityGameApp.Inst.Resource).LoadTexture($"icons/common/icon_jinbi");
                for (var i = 0; i < 5; i++)
                {
                    TemplateContainer temp = vts.CloneTree();
                    _goldNum.Add(temp);
                    if (tx1 != null)
                    {
                        temp.Q<VisualElement>("icon").style.backgroundImage = tx1;
                    }
                    temp.style.display = DisplayStyle.None;
                    flyAnis.Add(new FlyNumParam()
                    {
                        index = i,
                        num = 0,
                        obj = temp,
                        type = "gold"
                    });
                }

                var tx2 = ((UnityResourceManager)UnityGameApp.Inst.Resource).LoadTexture($"icons/products/icon_meat");
                for (var i = 5; i < 10; i++)
                {
                    TemplateContainer temp = vts.CloneTree();
                    _meatNum.Add(temp);
                    if (tx2 != null)
                    {
                        temp.Q<VisualElement>("icon").style.backgroundImage = tx2;
                    }
                    temp.style.display = DisplayStyle.None;
                    flyAnis.Add(new FlyNumParam()
                    {
                        index = i,
                        num = 0,
                        obj = temp,
                        type = "meat"
                    });
                }
            }

            if (n != 0)
            {
                for (var i = 0; i < flyAnis.Count; i++)
                {
                    var ani = flyAnis[i];
                    if (ani.type == type && ani.num == 0)
                    {
                        string sign = n > 0 ? "+" : "";
                        var _add = ani.obj.Q<Label>("numLabel");
                        var color = n > 0 ? new Color(141f / 255, 229f / 255,70f / 255) : new Color(235f / 255, 76f / 255, 14f / 255);
                        _add.style.color = new StyleColor(color);
                        //var color = n > 0 ? "#8DE847FF" : "#E34A18FF";
                        //_add.text = $"<color={color}>{sign}{StringUtil.StringNumFormat(n.ToString())}</color><br>";
                        _add.text = $"{sign}{StringUtil.StringNumFormat(n.ToString())}";
                        ani.obj.transform.position = new Vector3(0, 0);
                        ani.obj.style.display = DisplayStyle.Flex;
                        flyAnis[i].num = n;

                        break;
                    }
                }
            }
        }
        /// <summary>
        /// 当前关卡
        /// </summary>
        public void refreshCurrentLevel(int currentLevel)
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            int maxLevel = cmGame.gameConf.maxDefenseLevelCount;
            int firstNum = Math.Max(0, currentLevel - 1);
            int bossLevel = 10000;
            string bossIcon = "Mob_boss_1";
            foreach (var lvlConf in cmGame.gameConf.gameConfs.defenseLevels)
            {
                if (lvlConf.mapLevelName == "testLevelBigBoss")
                {
                    bossLevel = (currentLevel / lvlConf.levelDivide + 1) * lvlConf.levelDivide;
                    int num = bossLevel / lvlConf.levelDivide;
                    bossIcon = $"Mob_boss_{num}";
                }
            }
            for (int i = 0; i < 3; i++)
            {
                var item = _levelsNodes.Q<VisualElement>($"Item{i + 1}");
                var sprIcon = item.Q<VisualElement>("sprIcon");
                var labLevel = item.Q<Label>("labLevel");

                labLevel.text = $"{firstNum++}";

                if (i == 2)
                {
                    if (currentLevel == maxLevel)
                    {
                        labLevel.text = "Max";
                    }
                    sprIcon.style.display = currentLevel == bossLevel ? DisplayStyle.Flex : DisplayStyle.None;
                    labLevel.style.display = currentLevel == bossLevel ? DisplayStyle.None : DisplayStyle.Flex;
                }
                else
                {
                    sprIcon.style.display = DisplayStyle.Flex;
                    labLevel.style.display = DisplayStyle.Flex;
                }
            }
            var labBoss = _bossInfo.Q<Label>("labLevel");
            if (labBoss != null)
            {
                labBoss.text = $"{bossLevel - currentLevel} level";
            }
            VisualElement sprBoss = _bossInfo.Q<VisualElement>("bossHead");
            var tx = ((UnityResourceManager)UnityGameApp.Inst.Resource).LoadTexture($"icons/boss/{bossIcon}");
            if (tx != null)
            {
                sprBoss.style.backgroundImage = tx;
                sprBoss.style.width = tx.width;
                sprBoss.style.height = tx.height;
                sprBoss.style.left = 48 - tx.width / 2;
            }
        }
        public void refreshLevelInfo(CMShootingLevel lvl)
        {
            //DateTime t = new DateTime((long)(lvl.timeLeft * 1000));
            var t = new TimeSpan((long)(lvl.timeLeft * 10000000));
            string info = $"Time: {t.Minutes}:{t.Seconds}:{t.Milliseconds}";
            foreach (var kmPair in lvl.kmCount)
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
                _meatNum.text = StringUtil.StringNumFormat($"{meatInfo.count}");
            }
        }
        public void refreshTrainTime(long time)
        {
            TimeSpan t = new TimeSpan(time * 10000);
            _TrainTime.text = $"{t.Minutes}:{t.Seconds}";
        }

        public void ShowBattleStartInfo(bool isShow = true)
        {
            if (isShow)
            {
                // TODO use animation 
                _battleStartInfo.transform.position = new UnityEngine.Vector3(0, 0, 0);
            }
            else
            {
                _battleStartInfo.transform.position = new UnityEngine.Vector3(-240, 0, 0);
            }
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
            if (_notifyMessages.Count > 3)
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
            _NotifyBg.style.display = _notifyMessages.Count == 0 ? DisplayStyle.None : DisplayStyle.Flex;
            if (_notifyMessages.Count > 0)
            {
                _NotifyBg.style.height = new StyleLength(50f + 20f * _notifyMessages.Count);
                _NotifyBg.style.top = new StyleLength(275f - 10f * _notifyMessages.Count);
            }
        }

        private void onUpdateAni()
        {
            foreach (var ani in flyAnis)
            {
                if (ani.num != 0)
                {
                    ani.obj.transform.position -= new Vector3(0, Time.deltaTime * 50);
                    if (ani.obj.transform.position.y <= -50f)
                    {
                        ani.obj.style.display = DisplayStyle.None;
                        flyAnis[ani.index].num = 0;
                    }
                }
            }
        }
        public void OnUpdate()
        {
            onUpdateAni();

            if (_notifyMessages == null)
            {
                return;
            }

            bool changed = false;
            for (int i = 0; i < _notifyMessages.Count; ++i)
            {
                var notify = _notifyMessages[i];
                notify.timeLeft -= UnityEngine.Time.deltaTime;

                if (notify.timeLeft <= 0)
                {
                    _notifyMessages.RemoveAt(i);
                    --i;
                    changed = true;
                }
            }

            if (changed)
            {
                _refreshNotifyMessages();
            }
        }
    }
}
