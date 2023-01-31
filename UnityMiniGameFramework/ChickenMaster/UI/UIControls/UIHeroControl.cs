using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UIHeroControl : UIObject
    {
        override public string type => "UIHeroControl";
        new public static UIHeroControl create()
        {
            return new UIHeroControl();
        }

        protected Label _HeroName;
        public Label HeroName => _HeroName;

        protected Label _Level;
        public Label Level => _Level;

        protected Label _Info;
        public Label Info => _Info;

        protected Button _ActBtn;
        public Button ActBtn => _ActBtn;

        protected Label _Gun1;
        public Label Gun1 => _Gun1;

        protected Label _Gun2;
        public Label Gun2 => _Gun2;

        protected Label _Gun3;
        public Label Gun3 => _Gun3;

        protected VisualElement _Gun1Pic;
        public VisualElement Gun1Pic => _Gun1Pic;

        protected VisualElement _Gun2Pic;
        public VisualElement Gun2Pic => _Gun2Pic;

        protected VisualElement _Gun3Pic;
        public VisualElement Gun3Pic => _Gun3Pic;

        protected VisualElement _HeadPic;
        public VisualElement HeadPic => _HeadPic;

        protected ProgressBar _Gun1Prog;
        public ProgressBar Gun1Prog => _Gun1Prog;

        protected ProgressBar _Gun2Prog;
        public ProgressBar Gun2Prog => _Gun2Prog;

        protected ProgressBar _Gun3Prog;
        public ProgressBar Gun3Prog => _Gun3Prog;

        protected CMNPCHeros _hero;
        protected CMHeroConf _heroConf;

        protected LocalWeaponInfo _gun1Info;
        protected LocalWeaponInfo _gun2Info;
        protected LocalWeaponInfo _gun3Info;

        override public void onInit(UIControlConf c, VisualElement o)
        {
            base.onInit(c, o);

            _HeadPic = this._subControls["HeadPic"].unityVisualElement;
            _HeroName = this._subControls["HeroName"].unityVisualElement as Label;
            _Info = this._subControls["Info"].unityVisualElement as Label;
            _Level = this._subControls["Level"].unityVisualElement as Label;
            _ActBtn = this._subControls["ActBtn"].unityVisualElement as Button;

            _Gun1 = this._subControls["Gun1Text"].unityVisualElement as Label;
            _Gun2 = this._subControls["Gun2Text"].unityVisualElement as Label;
            _Gun3 = this._subControls["Gun3Text"].unityVisualElement as Label;
            _Gun1Pic = this._subControls["Gun1Pic"].unityVisualElement;
            _Gun2Pic = this._subControls["Gun2Pic"].unityVisualElement;
            _Gun3Pic = this._subControls["Gun3Pic"].unityVisualElement;

            _Gun1Prog = this._subControls["Gun1Prog"].unityVisualElement as ProgressBar;
            _Gun2Prog = this._subControls["Gun2Prog"].unityVisualElement as ProgressBar;
            _Gun3Prog = this._subControls["Gun3Prog"].unityVisualElement as ProgressBar;

            _ActBtn.RegisterCallback<MouseUpEvent>(onActBtnClick);

            _Gun1Pic.RegisterCallback<MouseUpEvent>(onGun1Click);
            _Gun2Pic.RegisterCallback<MouseUpEvent>(onGun2Click);
            _Gun3Pic.RegisterCallback<MouseUpEvent>(onGun3Click);
        }

        public void onActBtnClick(MouseUpEvent e)
        {
            if (_hero == null)
            {
                // not activate
                ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                if (_heroConf.userLevelRequire > 0 && (cmGame.baseInfo.getData() as LocalBaseInfo).level < _heroConf.userLevelRequire)
                {
                    cmGame.ShowTips(CMGNotifyType.CMG_ERROR, "User Level not reach !");
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
                    cmGame.ShowTips(CMGNotifyType.CMG_ERROR, "insuffcient gold !");
                }
            }
            else
            {
                // upgrade
                if(_hero.TryUpgrade())
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
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"UIHeroControl setHero [{name}] config not exist");
                return;
            }

            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.cmNPCHeros.TryGetValue(name, out _hero);

            _HeroName.text = _heroConf.mapHeroName;

            if(_hero != null)
            {
                _gun1Info = cmGame.GetWeaponInfo(_heroConf.guns[0]);
                _gun2Info = cmGame.GetWeaponInfo(_heroConf.guns[1]);
                _gun3Info = cmGame.GetWeaponInfo(_heroConf.guns[2]);
            }

            refreshInfo();

            refreshGunUpgradeProgress();
        }

        public void refreshInfo()
        {
            if(_hero == null)
            {
                // not active
                _ActBtn.text = "Activate";
                _Level.text = "Lv:0";
                _Info.text = $"Activate Gold: {_heroConf.activateGoldCost}";
            }
            else
            {
                ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;

                // get attack info
                int attack = 0;
                var conf = _hero.getCurrentHeroLevelConf();
                if(conf != null)
                {
                    attack = conf.combatConf.attackBase;
                }

                _ActBtn.text = "Upgrade";
                _Level.text = $"Lv:{_hero.heroInfo.level}";
                _Info.text = $"Upgrade Gold: {_hero.getUpgradeGoldCost()}\r\nAttack: {attack}";

                // get gun info

                _Gun1.text = $"Gun1 Lv: {_gun1Info.level}";

                if(_gun2Info == null)
                {
                    _Gun2.text = "NotActivate";
                }
                else
                {
                    _Gun2.text = $"Gun2 Lv: {_gun2Info.level}";
                }

                if (_gun3Info == null)
                {
                    _Gun3.text = "NotActivate";
                }
                else
                {
                    _Gun3.text = $"Gun3 Lv: {_gun3Info.level}";
                }
            }
        }

        protected void _refreshGunUpgrade(LocalWeaponInfo gunInfo, Label gunText, ProgressBar gunProg)
        {
            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var cmGunConf = cmGame.gameConf.getCMGunConf(gunInfo.id);
            var gunLevelConf = cmGunConf.gunLevelConf[gunInfo.level];

            var itemInfo = cmGame.Self.GetBackpackItemInfo(cmGunConf.upgradeItemName);
            if(itemInfo == null)
            {
                gunProg.value = 0;
            }
            else
            {
                if(itemInfo.count >= gunLevelConf.upgrageCostItemCost)
                {
                    gunProg.value = 100;
                    gunText.text = "ReadyUpgrade";
                }
                else
                {
                    gunProg.value = itemInfo.count * 100 / gunLevelConf.upgrageCostItemCost;
                }
            }
        }

        protected void _refreshGunActivate(int gunId, Label gunText, ProgressBar gunProg)
        {
            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            var cmGunConf = cmGame.gameConf.getCMGunConf(gunId);

            var itemInfo = cmGame.Self.GetBackpackItemInfo(cmGunConf.upgradeItemName);
            if (itemInfo == null)
            {
                gunProg.value = 0;
            }
            else
            {
                if (itemInfo.count >= cmGunConf.activateItemCost)
                {
                    gunProg.value = 100;
                    gunText.text = "ReadyActivte";
                }
                else
                {
                    gunProg.value = itemInfo.count * 100 / cmGunConf.activateItemCost;
                }
            }
        }

        public void refreshGunUpgradeProgress()
        {
            if(_gun1Info != null)
            {
                _refreshGunUpgrade(_gun1Info, _Gun1, _Gun1Prog);
            }

            if (_gun2Info != null)
            {
                _refreshGunUpgrade(_gun2Info, _Gun2, _Gun2Prog);
            }
            else
            {
                _refreshGunActivate(_heroConf.guns[1], _Gun2, _Gun2Prog);
            }

            if (_gun3Info != null)
            {
                _refreshGunUpgrade(_gun3Info, _Gun3, _Gun3Prog);
            }
            else
            {
                _refreshGunActivate(_heroConf.guns[2], _Gun3, _Gun3Prog);
            }
        }

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
                if(!_hero.TryUpgradeWeapon(gunId))
                {
                    // active failed
                    _hero.ChangeWeapon(gunId);

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
            if(changed)
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
    }
}
