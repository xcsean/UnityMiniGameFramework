﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.UIElements;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class UITowerHeroPanel : UIPopupPanel
    {
        override public string type => "UITowerHeroPanel";
        public static UITowerHeroPanel create()
        {
            return new UITowerHeroPanel();
        }

        protected Label _labHeroName;
        protected Label _labHeroLv;
        protected Label _labUpgradeCoin;
        protected Label _labAttackLv;
        protected Label _labAttackCur;
        protected Label _labAttackNext;
        protected Button _btnAct;
        protected VisualElement _gunItem1;
        protected VisualElement _gunItem2;
        protected VisualElement _gunItem3;
        protected VisualElement[] _gunItemArr = new VisualElement[3];

        protected CMNPCHeros _hero;
        protected CMHeroConf _heroConf;

        protected LocalWeaponInfo _gun1Info;
        protected LocalWeaponInfo _gun2Info;
        protected LocalWeaponInfo _gun3Info;


        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            FindUi();
        }

        protected void FindUi()
        {
            _labHeroName = this._uiObjects["labHeroName"].unityVisualElement as Label;
            _labHeroLv = this._uiObjects["labHeroLv"].unityVisualElement as Label;
            _labUpgradeCoin = this._uiObjects["labUpgradeCoin"].unityVisualElement as Label;
            _labAttackLv = this._uiObjects["labAttackLv"].unityVisualElement as Label;
            _labAttackCur = this._uiObjects["labAttackCur"].unityVisualElement as Label;
            _labAttackNext = this._uiObjects["labAttackNext"].unityVisualElement as Label;
            _btnAct = this._uiObjects["btnAct"].unityVisualElement as Button;

            _gunItem1 = this._uiObjects["gunItem1"].unityVisualElement;
            _gunItem2 = this._uiObjects["gunItem2"].unityVisualElement;
            _gunItem3 = this._uiObjects["gunItem3"].unityVisualElement;

            _gunItemArr[0] = _gunItem1;
            _gunItemArr[1] = _gunItem2;
            _gunItemArr[2] = _gunItem3;

            _gunItem1.Q<Button>("btnActive").RegisterCallback<MouseUpEvent>(onGun1Click);
            _gunItem1.Q<Button>("btnActive").RegisterCallback<MouseUpEvent>(onGun2Click);
            _gunItem1.Q<Button>("btnActive").RegisterCallback<MouseUpEvent>(onGun3Click);
            _gunItem1.Q<Button>("btnItem").RegisterCallback<MouseUpEvent>(OnChangeGunBtnClick1);
            _gunItem2.Q<Button>("btnItem").RegisterCallback<MouseUpEvent>(OnChangeGunBtnClick2);
            _gunItem3.Q<Button>("btnItem").RegisterCallback<MouseUpEvent>(OnChangeGunBtnClick3);
            _btnAct.RegisterCallback<MouseUpEvent>(OnActBtnClick);
        }

        public void OnActBtnClick(MouseUpEvent e)
        {
            if (_hero == null)
            {
                // not activate
                ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                if (_heroConf.userLevelRequire > 0 && (cmGame.baseInfo.getData() as LocalBaseInfo).level < _heroConf.userLevelRequire)
                {
                    // TO DO : level require

                    // for Debug ...
                    cmGame.uiMainPanel.NofityMessage(CMGNotifyType.CMG_ERROR, "User Level not reach !");
                }
                else if (cmGame.Self.TrySubGold(_heroConf.activateGoldCost))
                {
                    // active defense hero
                    _hero = cmGame.AddDefenseHero(_heroConf.mapHeroName);
                    //cmGame.baseInfo.markDirty();

                    _gun1Info = cmGame.GetWeaponInfo(_heroConf.guns[0]);
                    _gun2Info = cmGame.GetWeaponInfo(_heroConf.guns[1]);
                    _gun3Info = cmGame.GetWeaponInfo(_heroConf.guns[2]);

                    refreshInfo();
                }
                else
                {
                    // TO DO : not enough gold

                    // for Debug ...
                    cmGame.uiMainPanel.NofityMessage(CMGNotifyType.CMG_ERROR, "insuffcient gold !");
                }
            }
            else
            {
                // upgrade
                if (_hero.TryUpgrade())
                {
                    refreshInfo();
                }
            }

        }

        public void setHero(string name)
        {
            var cmGameConf = UnityGameApp.Inst.Conf.getConfig("cmgame") as CMGameConfig;
            _heroConf = cmGameConf.getCMHeroConf(name);
            if (_heroConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"UITowerHeroPanel setHero [{name}] config not exist");
                return;
            }

            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.cmNPCHeros.TryGetValue(name, out _hero);

            _labHeroName.text = _heroConf.mapHeroName;

            if (_hero != null)
            {
                _gun1Info = cmGame.GetWeaponInfo(_heroConf.guns[0]);
                _gun2Info = cmGame.GetWeaponInfo(_heroConf.guns[1]);
                _gun3Info = cmGame.GetWeaponInfo(_heroConf.guns[2]);
            }

            // TO DO : set hero pic and gun pic
            //_HeadPic.style.backgroundImage = new StyleBackground(texture2d);

            refreshInfo();

            refreshGunUpgradeProgress();
        }

        public void refreshInfo()
        {
            if (_hero == null)
            {
                // not active
                _btnAct.text = "Activate";
                _labHeroName.text = "";
                _labHeroLv.text = "Lv:0";
                _labUpgradeCoin.text = $"Gold: {_heroConf.activateGoldCost}";
            }
            else
            {
                ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;

                // get attack info
                int attack = 0;
                var conf = _hero.getCurrentHeroLevelConf();
                if (conf != null)
                {
                    attack = conf.combatConf.attackBase;
                }

                _btnAct.text = "Upgrade";
                _labHeroName.text = $"{_hero.heroInfo.mapHeroName}";
                _labHeroLv.text = $"Lv:{_hero.heroInfo.level}";
                _labUpgradeCoin.text = $"Gold: {_hero.getUpgradeGoldCost()}";
                _labAttackLv.text = $"Attack Lv：{attack}";
                _labAttackCur.text = $"{attack}";
                _labAttackNext.text = $"{attack + 1}";

                // set gun info
                RefreshGunInfo(_gun1Info, 0);
                RefreshGunInfo(_gun2Info, 1);
                RefreshGunInfo(_gun3Info, 2);
            }
        }

        protected void RefreshGunInfo(LocalWeaponInfo gunInfo, int gunIndex)
        {
            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            int gunId = _heroConf.guns[gunIndex];
            var cmGunConf = cmGame.gameConf.getCMGunConf(gunId);
            
            _gunItemArr[gunIndex].Q<Label>("labGunName").text = $"Gun{cmGunConf.id}";
            
            // to do: set gun icon

            if (gunInfo == null)
            {
                _gunItemArr[gunIndex].Q<Label>("labGunStar").text = $"{0}";
            }
            else
            {
                _gunItemArr[gunIndex].Q<Label>("labGunStar").text = $"{_gun1Info.level}";
            }
        }

        protected void _refreshGunUpgrade(LocalWeaponInfo gunInfo, int gunIndex)
        {
            VisualElement gunItem = _gunItemArr[gunIndex];
            var gunProg = gunItem.Q<ProgressBar>("progGunFrame");
            var bgGray = gunItem.Q<VisualElement>("bgGray");
            var btnActive = gunItem.Q<Button>("btnActive");

            bgGray.style.visibility = Visibility.Hidden;
            btnActive.text = "Upgrade";

            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var cmGunConf = cmGame.gameConf.getCMGunConf(gunInfo.id);
            var gunLevelConf = cmGunConf.gunLevelConf[gunInfo.level];

            var itemInfo = cmGame.Self.GetBackpackItemInfo(cmGunConf.upgradeItemName);
            if (itemInfo == null)
            {
                gunProg.value = 0;
            }
            else
            {
                if (itemInfo.count >= gunLevelConf.upgrageCostItemCost)
                {
                    gunProg.value = 100;
                    gunProg.title = $"{itemInfo.count}/{cmGunConf.activateItemCost}";
                }
                else
                {
                    gunProg.value = itemInfo.count * 100 / gunLevelConf.upgrageCostItemCost;
                    gunProg.title = $"{itemInfo.count}/{cmGunConf.activateItemCost}";
                }
            }
        }

        protected void _refreshGunActivate(int gunIndex)
        {
            VisualElement gunItem = _gunItemArr[gunIndex];
            var gunProg = gunItem.Q<ProgressBar>("progGunFrame");
            var bgGray = gunItem.Q<VisualElement>("bgGray");
            var btnActive = gunItem.Q<Button>("btnActive");

            bgGray.style.visibility = Visibility.Visible;
            btnActive.text = "Active";

            int gunId = _heroConf.guns[gunIndex];
            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var cmGunConf = cmGame.gameConf.getCMGunConf(gunId);

            var itemInfo = cmGame.Self.GetBackpackItemInfo(cmGunConf.upgradeItemName);
            if (itemInfo == null)
            {
                gunProg.value = 0;
                gunProg.title = $"0/0";
            }
            else
            {
                if (itemInfo.count >= cmGunConf.activateItemCost)
                {
                    gunProg.value = 100;
                    gunProg.title = $"{itemInfo.count}/{cmGunConf.activateItemCost}";
                }
                else
                {
                    gunProg.value = itemInfo.count * 100 / cmGunConf.activateItemCost;
                    gunProg.title = $"{itemInfo.count}/{cmGunConf.activateItemCost}";
                }
            }
        }

        public void refreshGunUpgradeProgress()
        {
            if (_gun1Info != null)
            {
                _refreshGunUpgrade(_gun1Info, 0);
            }

            if (_gun2Info != null)
            {
                _refreshGunUpgrade(_gun2Info, 1);
            }
            else
            {
                _refreshGunActivate(1);
            }

            if (_gun3Info != null)
            {
                _refreshGunUpgrade(_gun3Info, 2);
            }
            else
            {
                _refreshGunActivate(2);
            }
        }

        /// <summary>
        /// 切换武器
        /// </summary>
        protected void OnChangeGun(int gunId)
        {
            if (_hero == null)
            {
                return;
            }
            _hero.ChangeWeapon(gunId);
            refreshInfo();
        }

        protected void OnChangeGunBtnClick1(MouseUpEvent e)
        {
            OnChangeGun(0);
        }

        protected void OnChangeGunBtnClick2(MouseUpEvent e)
        {
            OnChangeGun(1);
        }

        protected void OnChangeGunBtnClick3(MouseUpEvent e)
        {
            OnChangeGun(2);
        }

        /// <summary>
        /// 解锁、升星
        /// </summary>
        protected bool _onGunClick(int gunId)
        {
            if (_hero == null)
            {
                return false;
            }

            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var gunInfo = cmGame.GetWeaponInfo(gunId);

            if (gunInfo == null)
            {
                // not active
                return _hero.TryActiveWeapon(gunId);
            }
            else
            {
                // actived
                if (!_hero.TryUpgradeWeapon(gunId))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public void onGun1Click(MouseUpEvent e)
        {
            var changed = _onGunClick(_heroConf.guns[0]);
            if (changed)
            {
                ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                _gun1Info = cmGame.GetWeaponInfo(_heroConf.guns[0]);
                refreshInfo();
                refreshGunUpgradeProgress();
            }
        }
        public void onGun2Click(MouseUpEvent e)
        {
            var changed = _onGunClick(_heroConf.guns[1]);
            if (changed)
            {
                ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                _gun2Info = cmGame.GetWeaponInfo(_heroConf.guns[1]);
                refreshInfo();
                refreshGunUpgradeProgress();
            }
        }
        public void onGun3Click(MouseUpEvent e)
        {
            var changed = _onGunClick(_heroConf.guns[2]);
            if (changed)
            {
                ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                _gun3Info = cmGame.GetWeaponInfo(_heroConf.guns[2]);
                refreshInfo();
                refreshGunUpgradeProgress();
            }
        }

        public void ShowHero(string heroName)
        {
            setHero(heroName);
            showUI();
        }

    }
}