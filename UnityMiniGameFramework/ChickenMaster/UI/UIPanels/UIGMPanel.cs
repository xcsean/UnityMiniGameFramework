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
        protected TextField _idTextField;
        protected VisualElement _btnList;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            gmItemList = new List<GMItem>()
            {
                new GMItem(){ name = "金币", resID = "", count = 1000 },
                new GMItem(){ name = "鸡肉", resID = "", count = 1000 },
                new GMItem(){ name = "等级", resID = "", count = 1 },
                new GMItem(){ name = "挑战关卡", resID = "", count = 1 },
                new GMItem(){ name = "武器等级", resID = "", count = 1 },
                new GMItem(){ name = "武器攻速", resID = "", count = 100 },
                new GMItem(){ name = "武器范围", resID = "", count = 5 }
             };

            FindUI();

            InitInfo();
        }

        protected void FindUI()
        {
            BindShowActionVE(this._uiObjects["Content"].unityVisualElement);

            _btnList = this._uiObjects["btnList"].unityVisualElement;
            _countTextField = this._uiObjects["countTextField"].unityVisualElement as TextField;
            _idTextField = this._uiObjects["idTextField"].unityVisualElement as TextField;

            // 不能代码修改，导致输入框无法使用？
            //_countTextField.label = "";
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
            // 
            _countTextField.value = "";
            _idTextField.value = "1";
        }

        protected int GetItemCount(string gmName)
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
                return 0;
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
            return input;
        }

        protected string GetItemID(bool isParse = false)
        {
            string inputStr = _idTextField.value;
            // 需要int数据 校验下
            if (isParse)
            {
                if (inputStr != null && inputStr != "")
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(inputStr, @"^\d+$"))
                    {
                        return inputStr;
                    }
                    else
                    {
                        return "0";
                    }
                }
                else
                {
                    return "0";
                }
            }
            return inputStr;
        }

        protected void onBtnGm(string gmName)
        {
            int amount = GetItemCount(gmName);
            bool isPrint = true;
            string logStr = $"GM工具，增加{gmName}数量{amount}";

            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            switch (gmName)
            {
                case "金币":
                    cmGame.Self.AddGold(amount);
                    break;
                case "鸡肉":
                    cmGame.Self.AddBackpackProduct("meat", amount);
                    break;
                case "等级":
                    logStr = $"GM工具，设置{gmName}为{amount}";
                    cmGame.Self.GM_SetPlayerLevel(amount);
                    break;
                case "挑战关卡":
                    logStr = $"GM工具，开始{gmName}{amount}";
                    onStartLevel(amount);
                    break;
                case "武器等级":
                    int weaponId = int.Parse(GetItemID());
                    logStr = $"GM工具，设置{gmName},ID:{weaponId},数量:{amount}";
                    modifyWeaponLevel(weaponId, amount);
                    break;
                case "武器攻速":
                    int weaponId2 = int.Parse(GetItemID());
                    logStr = $"GM工具，设置{gmName},ID:{weaponId2},数量:{amount}";
                    modifyWeaponAttackSpeed(weaponId2, amount);
                    break;
                case "武器范围":
                    int weaponId3 = int.Parse(GetItemID());
                    logStr = $"GM工具，设置{gmName},ID:{weaponId3},数量:{amount}";
                    modifyWeaponRange(weaponId3, amount);
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

        /// <summary>
        /// 修改武器星级
        /// </summary>
        public void modifyWeaponLevel(int weaponId, int level)
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var heros = cmGame.GetDefAreaHeros();
            bool has = false;
            foreach (var hero in heros)
            {
                if (hero.Value.conf.guns.Contains(weaponId))
                {
                    has = true;
                    hero.Value.GM_TryUpgradeWeapon(weaponId, level);
                    return;
                }
            }
            if (!has)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"UIGMPanel modifyWeaponLevel {weaponId} not exist or not active");
                return;
            }
        }
        /// <summary>
        /// 修改武器攻击范围
        /// </summary>
        public void modifyWeaponRange(int weaponId, int range)
        {
            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            var heros = cmGame.GetDefAreaHeros();
            bool has = false;
            foreach (var hero in heros)
            {
                if (hero.Value.conf.guns.Contains(weaponId))
                {
                    has = true;
                    hero.Value.gun.SetAttackRange(range);
                    return;
                }
            }
            if (!has)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"UIGMPanel modifyWeaponRange {weaponId} not exist or not active");
                return;
            }
        }
        /// <summary>
        /// 修改武器攻速
        /// </summary>
        public void modifyWeaponAttackSpeed(int weaponId, int speed)
        {
            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            var heros = cmGame.GetDefAreaHeros();
            bool has = false;
            foreach (var hero in heros)
            {
                if (hero.Value.conf.guns.Contains(weaponId))
                {
                    has = true;
                    hero.Value.gun.GM_UpdateFireCd(speed);
                    return;
                }
            }
            if (!has)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"UIGMPanel modifyWeaponAttackSpeed {weaponId} not exist or not active");
                return;
            }
        }

        /// <summary>
        /// 挑战关卡
        /// </summary>
        protected void onStartLevel(int _level)
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
