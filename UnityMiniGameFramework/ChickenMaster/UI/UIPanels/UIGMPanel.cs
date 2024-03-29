﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UIElements;

using MiniGameFramework;
using Debug = MiniGameFramework.Debug;

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
        protected Label _labStats;
        protected Label _labFPS;
        protected Toggle _toggleClearData;
        protected VisualElement _heroPanel;

        ProfilerRecorder drawcall;
        ProfilerRecorder batches;
        ProfilerRecorder vertices;

        private long _drawcallCnt;
        protected long drawcallCnt
        {
            get { return _drawcallCnt; }
            set
            {
                if (_drawcallCnt != value && value != 0)
                {
                    _drawcallCnt = value;
                    UpdateStats();
                }
            }
        }
        private long _batchesCnt;
        protected long batchesCnt
        {
            get { return _batchesCnt; }
            set
            {
                if (_batchesCnt != value && value != 0)
                {
                    _batchesCnt = value;
                    UpdateStats();
                }
            }
        }
        private long _verticesCnt;
        protected long verticesCnt
        {
            get { return _verticesCnt; }
            set
            {
                if (_verticesCnt != value && value != 0)
                {
                    _verticesCnt = value;
                    UpdateStats();
                }
            }
        }

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            gmItemList = new List<GMItem>()
            {
                new GMItem(){ name = "金币", resID = "", count = 1000 },
                new GMItem(){ name = "鸡肉", resID = "", count = 1000 },
                new GMItem(){ name = "等级", resID = "", count = 10 },
                new GMItem(){ name = "挑战关卡", resID = "", count = 100 },
                new GMItem(){ name = "武器等级", resID = "", count = 5 },
                new GMItem(){ name = "武器攻速", resID = "", count = 400 },
                new GMItem(){ name = "武器范围", resID = "", count = 10 },
                new GMItem(){ name = "Log", resID = "", count = 10 },
                new GMItem(){ name = "FPS", resID = "", count = 10 },
                new GMItem(){ name = "添加道具", resID = "", count = 100 },
                new GMItem(){ name = "直接通关", resID = "", count = 100 },
                new GMItem(){ name = "设置英雄", resID = "", count = 100 }
             };

            FindUI();

            InitInfo();
        }

        protected void FindUI()
        {
            BindShowActionVE(this._uiObjects["Content"].unityVisualElement);

            _labFPS = this._uiObjects["labFPS"].unityVisualElement as Label;
            _labStats = this._uiObjects["labStats"].unityVisualElement as Label;
            _btnList = this._uiObjects["btnList"].unityVisualElement;
            _countTextField = this._uiObjects["countTextField"].unityVisualElement as TextField;
            _idTextField = this._uiObjects["idTextField"].unityVisualElement as TextField;
            _toggleClearData = this._uiObjects["ToggleClearData"].unityVisualElement as Toggle;
            _heroPanel = this._uiObjects["heroPanel"].unityVisualElement;

            _heroPanel.style.display = DisplayStyle.None;

            _labFPS.text = "";
            _labStats.text = "";
            // 不能代码修改，导致输入框无法使用？
            //_countTextField.label = "";

            // 退出游戏清档
            _toggleClearData.RegisterValueChangedCallback(evt =>
            {
                UnityGameApp.Inst.isClearUserData = evt.newValue;
            });

            batches = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Batches Count");
            drawcall = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");
            vertices = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Vertices Count");
        }

        public override void showUI()
        {
            base.showUI();

            RefreshInfo();

            closeHeroPanel();

            addUpdate(onUpdate);
        }
        protected void onUpdate()
        {
            if (showFPS)
            {
                onUpdateGameStats();
            }
        }

        protected bool showFPS = true;
        protected float fpsMs = 0f;
        protected float fpsInterval = 0;
        protected void onUpdateGameStats()
        {
            fpsMs = Time.unscaledDeltaTime;

            fpsInterval += 1;
            if (fpsInterval > 5f)
            {
                fpsInterval = 0;
                UpdateFPS();
            }

            batchesCnt = batches.LastValue;
            drawcallCnt = drawcall.LastValue;
            verticesCnt = vertices.LastValue;
        }

        protected void UpdateFPS()
        {
            _labFPS.text = $"{1.0f / fpsMs:f1}FPS({1000f *fpsMs:f1}ms)";
        }

        protected void UpdateStats()
        {
            _labStats.text = $"Batches: {_batchesCnt}\r\nDrawCall: {_drawcallCnt}\r\nVertices: {_verticesCnt}";
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
                btn.text = $"{item.name}";

                btn.clicked += () =>
                {
                    onBtnGm(item.name);
                };
                idx++;
            }
        }

        protected void RefreshInfo()
        {
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
                    if (inputStr.Length > Int32.MaxValue.ToString().Length - 1)
                    {
                        input = 10000 * 10000;
                    }
                    else
                    {
                        input = Int32.Parse(inputStr);
                    }
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
                case "Log":
                    var _ui = UnityGameApp.Inst.UI.createUIPanel("GMLogUI") as UIGMLogPanel;
                    _ui.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.uiRootObject).unityGameObject.transform);
                    _ui.showUI();
                    break;
                case "FPS":
                    showFPS = !showFPS;
                    if (!showFPS)
                    {
                        _labFPS.text = "";
                        _labStats.text = "";
                    }
                    break;
                case "添加道具":
                    string itemName = GetItemID();
                    cmGame.Self.AddBackpackItem(itemName, amount);
                    break;
                case "直接通关":
                    onEndLevel();
                    break;
                case "设置英雄":
                    logStr = $"GM工具，设置{gmName}";
                    if (_heroPanel.style.display.Equals(DisplayStyle.None))
                    {

                        openHeroPanel();
                    }
                    else
                    {
                        closeHeroPanel();
                    }
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
                    cmGame.Egg.eggUI.clearRecoverTime();
                }
            }
            else if (!UnityGameApp.Inst.MainScene.map.currentLevel.isStarted)
            {
                var level = UnityGameApp.Inst.MainScene.map.CreateLevel(lvlConf.mapLevelName);
                if (level != null)
                {
                    (level as CMShootingLevel).SetDefenseLevelConf(lvlConf, _level);

                    level.Start();
                    cmGame.Egg.eggUI.clearRecoverTime();
                }
            }
        }

        protected void onEndLevel()
        {
            var level = UnityGameApp.Inst.MainScene.map.currentLevel;
            if (level != null && level.isStarted)
            {
                (level as CMShootingLevel).GM_EndLevel();
            }
        }

        private List<string> disableHeroGuns;
        /// <summary>
        /// 英雄禁用武器
        /// </summary>
        protected void openHeroPanel()
        {
            _heroPanel.style.display = DisplayStyle.Flex;

            var heroEF = _heroPanel.Q<DropdownField>("heroEnumField");
            var heroInfoList = _heroPanel.Q<VisualElement>("heroInfoList");
            var closeBtn = _heroPanel.Q<Button>("btnCloseHeroPanel");
            var _btnSelectAll = _heroPanel.Q<Button>("btnSelectAll");

            if (disableHeroGuns == null)
            {
                disableHeroGuns = new List<string>();

                closeBtn.clicked += () =>
                {
                    closeHeroPanel();
                };

                _btnSelectAll.clicked += () =>
                {
                    bool selectAll = false;
                    for (int i = 0; i < heroInfoList.childCount; i++)
                    {
                        if (heroInfoList.ElementAt(i).style.display.Equals(DisplayStyle.None))
                        {
                            continue;
                        }
                        if (!(heroInfoList.ElementAt(i) as Toggle).value)
                        {
                            selectAll = true;
                            break;
                        }
                    }
                    for (int i = 0; i < heroInfoList.childCount; i++)
                    {
                        if (heroInfoList.ElementAt(i).style.display.Equals(DisplayStyle.None))
                        {
                            continue;
                        }
                        (heroInfoList.ElementAt(i) as Toggle).value = selectAll;
                    }
                };
            }
            // 可操作选项
            var heroOptList = new List<string>();
            heroOptList.Add("禁止开枪");
            heroEF.choices = heroOptList;
            heroEF.index = 0;

            for (int i = 0; i < heroInfoList.childCount; i++)
            {
                heroInfoList.ElementAt(i).style.display = DisplayStyle.None;
            }

            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var defHeros = cmGame.GetDefAreaHeros().Values.ToArray();
            for (int i = 0; i < defHeros.Length; i++)
            {
                heroInfoList.ElementAt(i).style.display = DisplayStyle.Flex;
                string name = defHeros[i].heroInfo.mapHeroName;
                (heroInfoList.ElementAt(i) as Toggle).label = $"{name}";
                (heroInfoList.ElementAt(i) as Toggle).value = disableHeroGuns.Contains(name);
                (heroInfoList.ElementAt(i) as Toggle).RegisterValueChangedCallback(evt => {
                    if (evt.newValue)
                    {
                        if (!disableHeroGuns.Contains(name))
                        {
                            disableHeroGuns.Add(name);
                        }
                    }
                    else
                    {
                        if (disableHeroGuns.Contains(name))
                        {
                            disableHeroGuns.Remove(name);
                        }
                    }
                });
            }
        }

        public bool IsDisableHero(string name)
        {
            return (disableHeroGuns != null && disableHeroGuns.Contains(name));
        }

        protected void closeHeroPanel()
        {
            _heroPanel.style.display = DisplayStyle.None;
        }

    }
}
