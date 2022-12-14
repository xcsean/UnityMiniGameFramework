using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class AIStateManager
    {
        protected Dictionary<string, Func<ActorObject, AIState>> _aiStateObjectCreators;

        public AIStateManager()
        {
            _aiStateObjectCreators = new Dictionary<string, Func<ActorObject, AIState>>();
        }

        public void registerAIStateObjectCreator(string type, Func<ActorObject, AIState> creator)
        {
            if (_aiStateObjectCreators.ContainsKey(type))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"registerAIStateObjectCreator ({type}) already exist");
                return;
            }
            _aiStateObjectCreators[type] = creator;
        }

        public AIState createAIStateObject(string type, ActorObject actor)
        {
            if (_aiStateObjectCreators.ContainsKey(type))
            {
                return _aiStateObjectCreators[type](actor);
            }

            Debug.DebugOutput(DebugTraceType.DTT_Error, $"createAIStateObject ({type}) not exist");

            return null;
        }
    }
}
