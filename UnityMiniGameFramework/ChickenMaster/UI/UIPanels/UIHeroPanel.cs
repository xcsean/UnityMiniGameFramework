using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class UIHeroPanel : UIPopupPanel
    {
        override public string type => "UIHeroPanel";
        public static UIHeroPanel create()
        {
            return new UIHeroPanel();
        }

        protected UIHeroControl _hero;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _hero = this._uiObjects["HeroControl"] as UIHeroControl;
        }

        public void ShowHero(string heroName)
        {
            _hero.setHero(heroName);

            showUI();
        }
    }
}
