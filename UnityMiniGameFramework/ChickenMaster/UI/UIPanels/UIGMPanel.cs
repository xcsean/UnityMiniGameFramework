using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.UIElements;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    /// <summary>
    /// 自定义类型 临时
    /// </summary>
    public enum EResID
    {
        GOLD = 0,
        MEAT,
        GAME_LEVEL
    }

    public class GMItem
    {
        public string name;
        public string resID;
        public int count;
    }

    /// <summary>
    /// GM工具
    /// </summary>
    public class UIGMPanel : UIPopupPanel
    {
        override public string type => "UIGMPanel";
        public static UIGMPanel create()
        {
            return new UIGMPanel();
        }

        protected List<GMItem> gmItemList;
        protected TextField _countTextField;
        protected VisualElement _btnList;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            gmItemList = new List<GMItem>()
            {
                new GMItem(){ name = "金币", resID = "", count = 1000 },
                new GMItem(){ name = "鸡肉", resID = "", count = 1000 },
                new GMItem(){ name = "等级", resID = "", count = 1 },
                new GMItem(){ name = "关卡", resID = "", count = 1 },
             };

            FindUI();

            InitInfo();
        }

        protected void FindUI()
        {
            BindShowActionVE(this._uiObjects["Content"].unityVisualElement);

            _btnList = this._uiObjects["btnList"].unityVisualElement;
            _countTextField = this._uiObjects["countTextField"].unityVisualElement as TextField;

            // 不能代码修改，导致输入框无法使用？
            //_countTextField.label = "100";
        }

        public override void showUI()
        {
            base.showUI();

            RefreshInfo();
        }

        protected void InitInfo()
        {
            for (var i = 0; i < _btnList.childCount; i++)
            {
                var btn = _btnList.ElementAt(i);
                btn.style.display = DisplayStyle.None;
            }

            int idx = 0;
            foreach (var item in gmItemList)
            {
                if (!(_btnList.ElementAt(idx) is Button btn))
                {
                    break;
                }
                btn.style.display = DisplayStyle.Flex;
                (btn.Q("labBtnGm") as Label).text = $"{item.name}";

                btn.clicked += () =>
                {
                    onBtnGm(item.name);
                };
                idx++;
            }
        }

        protected void RefreshInfo()
        {
            _countTextField.value = "0";
        }

        protected void onBtnGm(string gmName)
        {
            GMItem gmItem = null;
            foreach (var item in gmItemList)
            {
                if (item.name == gmName)
                {
                    gmItem = item;
                    break;
                }
            }
            if (gmItem == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"UIGMPanel {gmName} button not exist");
                return;
            }
            int input = gmItem.count;
            string inputStr = _countTextField.value;
            if (inputStr != null && inputStr != "")
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(inputStr, @"^\d+$"))
                {
                    input = int.Parse(inputStr);
                }
            }

            string logStr = $"GM工具，增加{gmName}数量{input}";
            bool isPrint = true;

            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            switch (gmName)
            {
                case "金币":
                    cmGame.Self.AddGold(input);
                    break;
                case "鸡肉":
                    cmGame.Self.AddBackpackProduct("meat", input);
                    break;
                case "等级":
                    cmGame.Self.AddTestLevel(input);
                    break;
                case "关卡":
                    onStartLevel(input);
                    break;
                default:
                    isPrint = false;
                    break;
            }
            if (isPrint)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Debug, logStr);
            }
        }

        public void onStartLevel(int _level)
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var bi = (cmGame.baseInfo.getData() as LocalBaseInfo);

            if (bi.egg.hp <= 0)
            {
                // can't start level
                return;
            }

            var lvlConf = cmGame.GetDefenseLevelConf(_level);
            if (lvlConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"GetDefenseLevelConf [{_level}] not exist");
                return;
            }

            if (UnityGameApp.Inst.MainScene.map.currentLevel == null)
            {
                var level = UnityGameApp.Inst.MainScene.map.CreateLevel(lvlConf.mapLevelName);
                if (level != null)
                {
                    (level as CMShootingLevel).SetDefenseLevelConf(lvlConf, _level);

                    level.Start();
                }
            }
            else if (!UnityGameApp.Inst.MainScene.map.currentLevel.isStarted)
            {
                var level = UnityGameApp.Inst.MainScene.map.CreateLevel(lvlConf.mapLevelName);
                if (level != null)
                {
                    (level as CMShootingLevel).SetDefenseLevelConf(lvlConf, _level);

                    level.Start();
                }
            }
        }
    }
}
