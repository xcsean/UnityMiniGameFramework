using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    class UIObject : IUIObject
    {
        protected string _name;
        public string name => _name;

        virtual public string type => "UIObject";

        virtual public void bindEventAction(uint eventCode, Action<object> eventAction)
        {
            throw new NotImplementedException();
        }

        virtual public void bindEventAction<T>(uint eventCode, Action<T> eventAction)
        {
            throw new NotImplementedException();
        }

        virtual public void hideUI()
        {
            throw new NotImplementedException();
        }

        virtual public void showUI()
        {
            throw new NotImplementedException();
        }

        virtual public void onInit(UIControlConf c)
        {
            _name = c.name;
            // TO DO : 
        }
    }
}
