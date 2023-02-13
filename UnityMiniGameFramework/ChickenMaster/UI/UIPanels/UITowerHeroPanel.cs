using System;
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
        protected Label _labAttacked;
        protected Label _labUpgradeCoin;
        protected Label _labHeroActCoin;
        protected Label _labAttackCur;
        protected Label _labAttackNext;
        protected Button _btnHeroAct;
        protected Button _btnHeroUpgrade;
        protected VisualElement _heroLockBg;
        protected VisualElement _sprHeroIcon;
        protected VisualElement _advanced;
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

            FindUI();
        }

        protected void FindUI()
        {
            BindMoveActionVE(this._uiObjects["Content"].unityVisualElement);

            _sprHeroIcon = this._uiObjects["sprHeroIcon"].unityVisualElement;
            _heroLockBg = this._uiObjects["heroLockBg"].unityVisualElement;
            _labHeroName = this._uiObjects["labHeroName"].unityVisualElement as Label;
            _labUpgradeCoin = this._uiObjects["labUpgradeCoin"].unityVisualElement as Label;
            _labHeroActCoin = this._uiObjects["labHeroActCoin"].unityVisualElement as Label;
            _labAttacked = this._uiObjects["labAttacked"].unityVisualElement as Label;
            _labAttackCur = this._uiObjects["labAttackCur"].unityVisualElement as Label;
            _labAttackNext = this._uiObjects["labAttackNext"].unityVisualElement as Label;
            _advanced = this._uiObjects["advanced"].unityVisualElement;
            _btnHeroAct = this._uiObjects["btnHeroAct"].unityVisualElement as Button;
            _btnHeroUpgrade = this._uiObjects["btnHeroUpgrade"].unityVisualElement as Button;

            _gunItem1 = this._uiObjects["gunItem1"].unityVisualElement;
            _gunItem2 = this._uiObjects["gunItem2"].unityVisualElement;
            _gunItem3 = this._uiObjects["gunItem3"].unityVisualElement;

            _gunItemArr[0] = _gunItem1;
            _gunItemArr[1] = _gunItem2;
            _gunItemArr[2] = _gunItem3;

            _gunItem1.Q<Button>("btnActive").clicked += onGun1Click;
            _gunItem2.Q<Button>("btnActive").clicked += onGun2Click;
            _gunItem3.Q<Button>("btnActive").clicked += onGun3Click;
            _gunItem1.Q<Button>("btnItem").clicked += OnUpgradeGunBtnClick1;
            _gunItem2.Q<Button>("btnItem").clicked += OnUpgradeGunBtnClick2;
            _gunItem3.Q<Button>("btnItem").clicked += OnUpgradeGunBtnClick3;
            _gunItem1.Q<Button>("btnArmed").clicked += OnChangeGunBtnClick1;
            _gunItem2.Q<Button>("btnArmed").clicked += OnChangeGunBtnClick2;
            _gunItem3.Q<Button>("btnArmed").clicked += OnChangeGunBtnClick3;

            _btnHeroAct.clicked += OnActBtnClick;
            _btnHeroUpgrade.clicked += OnUpgradeBtnClick;
        }

        /// <summary>
        /// 英雄激活
        /// </summary>
        public void OnActBtnClick()
        {
            if (_hero == null)
            {
                // not activate
                ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                if (_heroConf.userLevelRequire > 0 && (cmGame.baseInfo.getData() as LocalBaseInfo).currentLevel < _heroConf.userLevelRequire)
                {
                    cmGame.ShowTips(CMGNotifyType.CMG_ERROR, "Game Level not reach !");
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
                    refreshGunUpgradeProgress();
                }
                else
                {
                    cmGame.ShowTips(CMGNotifyType.CMG_ERROR, "insuffcient gold !");
                }
            }
        }

        /// <summary>
        /// 英雄升级
        /// </summary>
        public void OnUpgradeBtnClick()
        {
            if (_hero != null)
            {
                // upgrade
                if (_hero.TryUpgrade())
                {
                    refreshInfo();
                    refreshGunUpgradeProgress();
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

            _gun1Info = cmGame.GetWeaponInfo(_heroConf.guns[0]);
            _gun2Info = cmGame.GetWeaponInfo(_heroConf.guns[1]);
            _gun3Info = cmGame.GetWeaponInfo(_heroConf.guns[2]);

            refreshInfo();
            refreshGunUpgradeProgress();
        }

        public void refreshInfo()
        {
            var tx = ((UnityResourceManager)UnityGameApp.Inst.Resource).LoadTexture($"icons/heros/{_heroConf.halfHead}");

            if(tx != null)
            {
                _sprHeroIcon.style.backgroundImage = tx;
                _sprHeroIcon.style.width = tx.width;
                _sprHeroIcon.style.height = tx.height;
            }
            if (_hero == null)
            {
                // not active
                _heroLockBg.style.display = DisplayStyle.Flex;

                _labHeroName.text = $"{_heroConf.mapHeroName}";
                _labHeroActCoin.text = StringUtil.StringNumFormat($"{_heroConf.activateGoldCost}");
                _labUpgradeCoin.text = $"{0}";

                _labAttacked.text = $"0";
                _labAttackCur.text = $"{0}";
                _labAttackNext.text = $"{0}";

                refreshHeroAdvanced(0);
            }
            else
            {
                _heroLockBg.style.display = DisplayStyle.None;
                // get attack info
                int attack = 0;
                var conf = _hero.getCurrentHeroLevelConf();
                if (conf != null)
                {
                    attack = conf.combatConf.attackBase;
                }
                var nextConf = _hero.getNextHeroLevelConf();
                if (nextConf == null)
                {
                    nextConf = conf;
                }
                int upgradeCost = _hero.getUpgradeGoldCost();

                _labHeroName.text = $"{_heroConf.mapHeroName}";
                _labUpgradeCoin.text = StringUtil.StringNumFormat($"{upgradeCost}");

                _labAttacked.text = $"{_hero.heroInfo.level}";  // 改为等级
                _labAttackCur.text = StringUtil.StringNumFormat($"{attack}");
                _labAttackNext.text = StringUtil.StringNumFormat($"{nextConf.combatConf.attackBase}");

                refreshHeroAdvanced(_hero.heroInfo.level);
            }
        
            // set gun info
            RefreshGunInfo(_gun1Info, 0);
            RefreshGunInfo(_gun2Info, 1);
            RefreshGunInfo(_gun3Info, 2);
        }

        protected void RefreshGunInfo(LocalWeaponInfo gunInfo, int gunIndex)
        {
            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            int gunId = _heroConf.guns[gunIndex];
            var cmGunConf = cmGame.gameConf.getCMGunConf(gunId);

            VisualElement gunItem = _gunItemArr[gunIndex];
            gunItem.style.display = DisplayStyle.Flex;
            var btnArmed = gunItem.Q<Button>("btnArmed");

            var cmGameConf = UnityGameApp.Inst.Conf.getConfig("cmgame") as CMGameConfig;
            gunItem.Q<Label>("labGunName").text = $"{cmGameConf.gameConfs.gunConfs[cmGunConf.id].name}";

            // 切换武器
            if (_hero != null && _hero.heroInfo.holdWeaponId == gunId)
            {
                btnArmed.text = "ARMED";
            }
            else
            {
                btnArmed.text = "UNARMED";
            }

            // set gun icon
            var tx = ((UnityResourceManager)UnityGameApp.Inst.Resource).LoadTexture($"icons/weapons/{cmGunConf.weaponIcon}");
            gunItem.Q("sprGunIcon").style.backgroundImage = tx;

            if (gunInfo == null)
            {
                gunItem.Q<Label>("labGunStar").text = $"{0}";
                btnArmed.style.display = DisplayStyle.None;
            }
            else
            {
                gunItem.Q<Label>("labGunStar").text = $"{gunInfo.level}";
                btnArmed.style.display = DisplayStyle.Flex;
            }
        }

        protected void _refreshGunUpgrade(LocalWeaponInfo gunInfo, int gunIndex)
        {
            VisualElement gunItem = _gunItemArr[gunIndex];
            var frameBar = gunItem.Q<VisualElement>("frame-bar");
            var barValue = gunItem.Q<Label>("bar-value");
            float barWidth = 94;

            var upgradeTip = gunItem.Q<VisualElement>("sprStarUpgrade");
            var btnActive = gunItem.Q<Button>("btnActive");

            btnActive.style.display = DisplayStyle.None;

            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var cmGunConf = cmGame.gameConf.getCMGunConf(gunInfo.id);
            var gunLevelConf = cmGunConf.gunLevelConf[gunInfo.level];

            int hasCnt;
            int needCnt;
            var itemInfo = cmGame.Self.GetBackpackItemInfo(cmGunConf.upgradeItemName);
            if (itemInfo == null)
            {
                hasCnt = 0;
                needCnt = gunLevelConf.upgrageCostItemCost;
            }
            else
            {
                hasCnt = itemInfo.count;
                needCnt = gunLevelConf.upgrageCostItemCost;
            }
            upgradeTip.style.display = hasCnt >= needCnt ? DisplayStyle.Flex : DisplayStyle.None;

            frameBar.style.width = new StyleLength(new Length(Math.Min(barWidth, barWidth * hasCnt / needCnt)));
            barValue.text = $"{hasCnt}/{needCnt}";
        }

        protected void _refreshGunActivate(int gunIndex)
        {
            VisualElement gunItem = _gunItemArr[gunIndex];
            var frameBar = gunItem.Q<VisualElement>("frame-bar");
            var barValue = gunItem.Q<Label>("bar-value");
            float barWidth = 94;

            var upgradeTip = gunItem.Q<VisualElement>("sprStarUpgrade");
            var btnActive = gunItem.Q<Button>("btnActive");
            var btnArmed = gunItem.Q<Button>("btnArmed");

            btnActive.style.display = DisplayStyle.Flex;
            upgradeTip.style.display = DisplayStyle.None;
            btnArmed.style.display = DisplayStyle.None;

            int gunId = _heroConf.guns[gunIndex];
            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var cmGunConf = cmGame.gameConf.getCMGunConf(gunId);

            int hasCnt;
            int needCnt;
            var itemInfo = cmGame.Self.GetBackpackItemInfo(cmGunConf.upgradeItemName);
            if (itemInfo == null)
            {
                hasCnt = 0;
                needCnt = cmGunConf.activateItemCost;
            }
            else
            {
                hasCnt = itemInfo.count;
                needCnt = cmGunConf.activateItemCost;
            }
            frameBar.style.width = new StyleLength(new Length(Math.Min(barWidth, barWidth * hasCnt / needCnt)));
            barValue.text = $"{hasCnt}/{needCnt}";
        }

        /// <summary>
        /// 武器升星碎片进度
        /// </summary>
        protected void refreshGunUpgradeProgress()
        {
            if (_gun1Info != null)
            {
                _refreshGunUpgrade(_gun1Info, 0);
            }
            else
            {
                _refreshGunActivate(0);
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
        /// 英雄等级进阶（暂无）
        /// </summary>
        private void refreshHeroAdvanced(int lv)
        {
            // 5级一次进阶 读配置
            float n = ((float)lv / 5 - 1);
            int len = _advanced.childCount;
            for (int i = 0; i < len; i++)
            {
                _advanced.ElementAt(i).style.display = n >= i ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        /// <summary>
        /// 切换武器
        /// </summary>
        protected void OnChangeGun(int gunIndex)
        {
            int gunId = _heroConf.guns[gunIndex];
            if (_hero == null)
            {
                return;
            }

            if (_hero.heroInfo.holdWeaponId == gunId)
            {
                return;
            }
            _hero.ChangeWeapon(gunId);

            refreshInfo();
        }

        protected void OnChangeGunBtnClick1()
        {
            OnChangeGun(0);
        }

        protected void OnChangeGunBtnClick2()
        {
            OnChangeGun(1);
        }

        protected void OnChangeGunBtnClick3()
        {
            OnChangeGun(2);
        }

        /// <summary>
        /// 升级武器
        /// </summary>
        /// <param name="gunIndex"></param>
        protected void OnUpgradeGun(int gunIndex)
        {
            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            int gunId = _heroConf.guns[gunIndex];

            if (_hero == null)
            {
                return;
            }
            LocalWeaponInfo weaponInfo = cmGame.GetWeaponInfo(gunId);
            if (weaponInfo == null)
            {
                // 未激活
                return;
            }
            if (!_hero.TryUpgradeWeapon(gunId))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Debug, "无法升级武器");
                return;
            }

            LocalWeaponInfo _gunInfo = null;
            if (gunIndex == 0)
            {
                _gun1Info = cmGame.GetWeaponInfo(_heroConf.guns[gunIndex]);
                _gunInfo = _gun1Info;
            }
            else if (gunIndex == 1)
            {
                _gun2Info = cmGame.GetWeaponInfo(_heroConf.guns[gunIndex]);
                _gunInfo = _gun2Info;
            }
            else if (gunIndex == 2)
            {
                _gun3Info = cmGame.GetWeaponInfo(_heroConf.guns[gunIndex]);
                _gunInfo = _gun3Info;
            }
            if (_gunInfo == null)
            {
                return;
            }
            refreshInfo();
            refreshGunUpgradeProgress();
            GunAscendSuccess(_gunInfo);
        }

        protected void OnUpgradeGunBtnClick1()
        {
            OnUpgradeGun(0);
        }
        protected void OnUpgradeGunBtnClick2()
        {
            OnUpgradeGun(1);
        }

        protected void OnUpgradeGunBtnClick3()
        {
            OnUpgradeGun(2);
        }

        /// <summary>
        /// 解锁
        /// </summary>
        protected bool OnUnlockGunClick(int gunIndex)
        {
            if (_hero == null)
            {
                return false;
            }
            int gunId = _heroConf.guns[gunIndex];

            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var gunInfo = cmGame.GetWeaponInfo(gunId);

            if (gunInfo == null)
            {
                // not active
                Debug.DebugOutput(DebugTraceType.DTT_Debug, "检查武器碎片是否足够");
                if (_hero.TryActiveWeapon(gunId))
                {
                    OnChangeGun(gunIndex);
                    return true;
                }

                return false;
            }
            else
            {
                return false;
            }
        }

        public void onGun1Click()
        {
            var upgrade = OnUnlockGunClick(0);
            if (upgrade)
            {
                ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                _gun1Info = cmGame.GetWeaponInfo(_heroConf.guns[0]);
                refreshInfo();
                refreshGunUpgradeProgress();
                GunAscendSuccess(_gun1Info);
            }
        }
        public void onGun2Click()
        {
            var upgrade = OnUnlockGunClick(1);
            if (upgrade)
            {
                ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                _gun2Info = cmGame.GetWeaponInfo(_heroConf.guns[1]);
                refreshInfo();
                refreshGunUpgradeProgress();
                GunAscendSuccess(_gun2Info);
            }
        }
        public void onGun3Click()
        {
            var upgrade = OnUnlockGunClick(2);
            if (upgrade)
            {
                ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                _gun3Info = cmGame.GetWeaponInfo(_heroConf.guns[2]);
                refreshInfo();
                refreshGunUpgradeProgress();
                GunAscendSuccess(_gun3Info);
            }
        }

        /// <summary>
        /// 武器升星成功
        /// </summary>
        private void GunAscendSuccess(LocalWeaponInfo gunInfo)
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var cmGunConf = cmGame.gameConf.getCMGunConf(gunInfo.id);
            UnityGameApp.Inst.RESTFulClient.Report(UnityGameApp.Inst.AnalysisMgr.GetPointData7($"武器[{cmGunConf.name}]，星级[{gunInfo.level}]"));

            UIWeaponAscendPanel _ui = UnityGameApp.Inst.UI.createUIPanel("WeaponAscendUI") as UIWeaponAscendPanel;
            _ui.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.uiRootObject).unityGameObject.transform);
            _ui.SetWeaponInfo(gunInfo);
        }

        public void ShowHero(string heroName)
        {
            setHero(heroName);

            showUI();
        }

    }
}
