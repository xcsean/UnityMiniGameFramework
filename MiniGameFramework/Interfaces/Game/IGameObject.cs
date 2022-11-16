using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public interface IGameObject
    {
        string name { get; }
        string type { get; }

        IAttribute attribute { get; }

        IGameObjectComponent getComponent(string compName);

        void Init(string confname);
        void Dispose();
        bool AddComponent(IGameObjectComponent comp);

        void OnUpdate(uint timeElasped);
        void OnPostUpdate(uint timeElasped);
    }
}
