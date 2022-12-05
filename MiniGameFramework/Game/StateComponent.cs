using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public class StateComponent : GameObjectComponent
    {
        override public string type => "StateComponent";
        public static StateComponent create()
        {
            return new StateComponent();
        }

        protected Dictionary<int, int> _states; // state name => state

        public void setState(int stateKey, int stateValue)
        {
            _states[stateKey] = stateValue;
        }
        public void unsetState(int stateKey)
        {
            _states.Remove(stateKey);
        }
        public int getState(int stateKey)
        {
            if (_states.ContainsKey(stateKey))
            {
                return _states[stateKey];
            }

            return 0;
        }
        public bool hasState(int stateKey)
        {
            return _states.ContainsKey(stateKey);
        }

        override public void Init(object config)
        {
            base.Init(config);

            _states = new Dictionary<int, int>();
        }
        override public void Dispose()
        {
            base.Dispose();
        }
    }
}
