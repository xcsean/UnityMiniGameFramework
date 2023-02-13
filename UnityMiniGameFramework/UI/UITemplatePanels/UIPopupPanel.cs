using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    abstract public class UIPopupPanel : UIPanel
    {
        protected Button _closeBtn;

        public bool mutex = false;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _closeBtn = this._uiObjects["CloseButton"].unityVisualElement as Button;

            _closeBtn.clicked += onCloseClick;
            mutex = conf.mutex;

        }
        //override public void hideUI()
        //{
        //    if(!this.isShow)
        //    {
        //        return;
        //    }

        //    _closeBtn.clicked -= onCloseClick;
        //    base.hideUI();
        //}

        //override public void showUI()
        //{
        //    if(this.isShow)
        //    {
        //        return;
        //    }

        //    _closeBtn.clicked += onCloseClick;
        //    base.showUI();
        //}

        virtual public void onCloseClick()
        {
            this.hideUI();
        }

        override public void showUI()
        {
            base.showUI();

            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.addUI(this);
        }

        override public void hideUI()
        {
            base.hideUI();

            ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.removeUI(this);
        }
    }
}
