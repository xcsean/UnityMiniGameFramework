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

            _ActBtn.RegisterCallback<MouseUpEvent>(onActBtnClick);

            // TO DO : add gun active/upgrade btn
        }

        public void onActBtnClick(MouseUpEvent e)
        {
            if (_hero == null)
            {
                // not activate
                ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                if (_heroConf.userLevelRequire > 0 && (cmGame.baseInfo.getData() as LocalBaseInfo).level < _heroConf.userLevelRequire)
                {
                    // TO DO : level require
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

            // TO DO : set hero pic and gun pic
            //_HeadPic.style.backgroundImage = new StyleBackground(texture2d);

            refreshInfo();
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

        public void refreshGunUpgradeProgress()
        {
            // TO DO : 
        }
    }
}
