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

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _closeBtn = this._uiObjects["CloseButton"].unityVisualElement as Button;

            _closeBtn.RegisterCallback<MouseUpEvent>(onCloseClick);

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

        public void onCloseClick(MouseUpEvent e)
        {
            this.hideUI();
        }
    }
}
