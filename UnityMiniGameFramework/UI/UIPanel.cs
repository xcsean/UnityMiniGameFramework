using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class UIPanel : IUIPanel
    {
        protected string _name;
        public string name => _name;

        virtual public string type => "UIPanel";

        virtual public void bindEventAction(uint eventCode, Action<object> eventAction)
        {
            throw new NotImplementedException();
        }

        virtual public void bindEventAction<T>(uint eventCode, Action<T> eventAction)
        {
            throw new NotImplementedException();
        }

        virtual public void Dispose()
        {
            throw new NotImplementedException();
        }

        virtual public IUIObject getUIObject(string name)
        {
            throw new NotImplementedException();
        }

        virtual public void hideUI()
        {
            throw new NotImplementedException();
        }

        virtual public void Init(UIPanelConf conf)
        {
            _name = conf.name;
            // TO DO : 
        }

        virtual public void showUI()
        {
            throw new NotImplementedException();
        }
    }
}
