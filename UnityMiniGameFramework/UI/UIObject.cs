using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UIObject : IUIObject
    {
        virtual public string type => "UIObject";
        public static UIObject create()
        {
            return new UIObject();
        }

        protected string _name;
        public string name => _name;

        protected VisualElement _unityVE;
        public VisualElement unityVisualElement => _unityVE;

        public UIObject()
        {

        }

        virtual public void hideUI()
        {
            _unityVE.visible = false;
        }

        virtual public void showUI()
        {
            _unityVE.visible = true;
        }

        virtual public void onInit(UIControlConf c, VisualElement o)
        {
            _name = c.name;
            _unityVE = o;
            // TO DO : 
        }
    }
}
