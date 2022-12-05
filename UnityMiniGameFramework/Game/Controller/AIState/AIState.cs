using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class AIState
    {
        protected ActorObject _actor;

        public AIState(ActorObject actor)
        {
            _actor = actor;
        }

        virtual public void OnUpdate()
        {

        }
    }
}
