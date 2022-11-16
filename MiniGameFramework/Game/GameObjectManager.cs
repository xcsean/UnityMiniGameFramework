using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace MiniGameFramework
{
    public class GameObjectManager
    {
        protected static ConcurrentDictionary<string, Func<IGameObject>> _gameObjectCreators = new ConcurrentDictionary<string, Func<IGameObject>>();
        protected static ConcurrentDictionary<string, Func<IGameObjectComponent>> _gameObjectComponentCreators = new ConcurrentDictionary<string, Func<IGameObjectComponent>>();

        public static void registerGameObjectCreator(string type, Func<IGameObject> creator)
        {
            if(_gameObjectCreators.ContainsKey(type))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"registerGameObjectCreator ({type}) already exist");
                return;
            }
            _gameObjectCreators[type] = creator;
        }
        
        public static void registerGameObjectComponentCreator(string type, Func<IGameObjectComponent> creator)
        {
            if (_gameObjectComponentCreators.ContainsKey(type))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"registerGameObjectComponentCreator ({type}) already exist");
                return;
            }
            _gameObjectComponentCreators[type] = creator;
        }

        public static IGameObject createGameObject(string type)
        {
            if (_gameObjectCreators.ContainsKey(type))
            {
                return _gameObjectCreators[type]();
            }

            Debug.DebugOutput(DebugTraceType.DTT_Error, $"createGameObject ({type}) not exist");

            return null;
        }

        public static IGameObjectComponent createGameObjectComponent(string type)
        {
            if (_gameObjectComponentCreators.ContainsKey(type))
            {
                return _gameObjectComponentCreators[type]();
            }

            Debug.DebugOutput(DebugTraceType.DTT_Error, $"createGameObjectComponent ({type}) not exist");

            return null;
        }
    }
}
