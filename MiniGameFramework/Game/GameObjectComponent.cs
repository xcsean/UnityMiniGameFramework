using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public abstract class GameObjectComponent : IGameObjectComponent
    {
        protected IGameObject _gameObject;

        virtual public string type => "GameObjectComponent";
        public IGameObject gameObject => _gameObject;

        virtual public void Init(object config)
        {
        }
        virtual public void Dispose()
        {
            _gameObject = null;
        }
        
        virtual public void OnAddToGameObject(IGameObject obj)
        {
            _gameObject = obj;
        }

        virtual public void OnUpdate(float timeElasped)
        {

        }
        virtual public void OnPostUpdate(float timeElasped)
        {

        }
    }
}
