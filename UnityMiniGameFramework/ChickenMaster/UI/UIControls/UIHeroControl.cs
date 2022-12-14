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

        protected VisualElement _HeadPic;
        public VisualElement HeadPic => _HeadPic;

        protected CMNPCHeros _hero;
        protected CMHeroConf _heroConf;

        override public void onInit(UIControlConf c, VisualElement o)
        {
            base.onInit(c, o);

            _HeadPic = this._subControls["HeadPic"].unityVisualElement;
            _HeroName = this._subControls["HeroName"].unityVisualElement as Label;
            _Info = this._subControls["Info"].unityVisualElement as Label;
            _Level = this._subControls["Level"].unityVisualElement as Label;
            _ActBtn = this._subControls["ActBtn"].unityVisualElement as Button;

            _ActBtn.RegisterCallback<MouseUpEvent>(onActBtnClick);
        }

        public void onActBtnClick(MouseUpEvent e)
        {
            if (_hero == null)
            {
                // not activate
                ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                if (cmGame.Self.TrySubGold(_heroConf.activateGoldCost))
                {
                    // active defense hero
                    _hero = cmGame.AddDefenseHero(_heroConf.mapHeroName);
                    //cmGame.baseInfo.markDirty();

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

            // TO DO : set hero pic
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
                _ActBtn.text = "Upgrade";
                _Level.text = $"Lv:{_hero.heroInfo.level}";
                _Info.text = $"Upgrade Gold: {_hero.getUpgradeGoldCost()}";
            }
        }
    }
}
