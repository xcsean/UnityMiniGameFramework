using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public abstract class GameObject : IGameObject
    {
        protected string _name;
        protected IAttribute _attribute;
        protected Dictionary<string, IGameObjectComponent> _components;

        public string name => _name;
        virtual public string type => "GameObject";

        public IAttribute attribute => _attribute;

        public event Action<GameObject> OnDispose;

        public GameObject()
        {
            _components = new Dictionary<string, IGameObjectComponent>();
        }

        virtual public void Init(string confname)
        {
        }
        virtual public void Dispose()
        {
            if(OnDispose != null)
            {
                OnDispose(this);
                OnDispose = null;
            }

            if(_components != null)
            {
                foreach (var pair in _components)
                {
                    pair.Value.Dispose();
                }
                _components = null;
            }
        }

        public IGameObjectComponent getComponent(string compName)
        {
            if (_components.ContainsKey(compName))
            {
                return _components[compName];
            }

            return null;
        }

        virtual public bool AddComponent(IGameObjectComponent comp)
        {
            if(_components.ContainsKey(comp.type))
            {
                // err
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"AddComponent GameObject:{_name} type:{type}, component type:{comp.type} already exist");
                return false;
            }

            comp.OnAddToGameObject(this);
            _components[comp.type] = comp;

            _onAddComponent(comp);

            return true;
        }
        virtual protected void _onAddComponent(IGameObjectComponent comp)
        {

        }

        virtual public void OnUpdate(float timeElasped)
        {
            if(_components != null)
            {
                foreach (var pair in _components)
                {
                    pair.Value.OnUpdate(timeElasped);
                }
            }
        }
        virtual public void OnPostUpdate(float timeElasped)
        {
            if (_components != null)
            {
                foreach (var pair in _components)
                {
                    pair.Value.OnPostUpdate(timeElasped);
                }
            }
        }

        virtual public void Hide()
        {
        }

        virtual public void Show()
        {
        }

    }
}
