using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

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

        protected UnityEngine.GameObject _unityGameObject;
        public UnityEngine.GameObject unityGameObject => _unityGameObject;

        public UIObject()
        {

        }

        virtual public void hideUI()
        {
            _unityGameObject.SetActive(false);
        }

        virtual public void showUI()
        {
            _unityGameObject.SetActive(true);
        }

        virtual public void onInit(UIControlConf c, UnityEngine.GameObject o)
        {
            _name = c.name;
            _unityGameObject = o;
            // TO DO : 
        }
    }
}
