using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class AIActorControllerComp : ActorControllerComponent
    {
        override public string type => "AIActorControllerComp";
        new public static AIActorControllerComp create()
        {
            return new AIActorControllerComp();
        }

        protected HashSet<AIState> _aiStates;

        public AIActorControllerComp()
        {
            _aiStates = new HashSet<AIState>();
        }

        override public void Init(object config)
        {
            base.Init(config);


        }

        public void AddAIState(AIState aiState)
        {
            _aiStates.Add(aiState);
        }

        public void RemoveAIState(AIState aiState)
        {
            _aiStates.Remove(aiState);
        }

        override public void OnUpdate(float timeElasped)
        {
            base.OnUpdate(timeElasped);

            foreach(var state in _aiStates)
            {
                state.OnUpdate();
            }
        }
    }
}
