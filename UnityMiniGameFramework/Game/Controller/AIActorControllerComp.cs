using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

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

            var aiStatesConf = config as List<MapConfAIState>;
            foreach(var aic in aiStatesConf)
            {
                var state = UnityGameApp.Inst.AIStates.createAIStateObject(aic.aiType, _gameObject as ActorObject);
                if(state == null)
                {
                    Debug.DebugOutput(DebugTraceType.DTT_Error, $"AI State type [{aic.aiType}] not exist");
                    continue;
                }

                state.Init(aic);
                AddAIState(state);
            }
        }

        public void AddAIState(AIState aiState)
        {
            _aiStates.Add(aiState);
        }

        public void RemoveAIState(AIState aiState)
        {
            _aiStates.Remove(aiState);
        }

        public T GetAIState<T>() where T:AIState
        {
            foreach(var aiS in _aiStates)
            {
                if(aiS.GetType() == typeof(T) || aiS.GetType().IsSubclassOf(typeof(T)))
                {
                    return (T)aiS;
                }
            }

            return null;
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
