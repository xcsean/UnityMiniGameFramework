using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public interface IGameObjectComponent
    {
        string type { get; }

        IGameObject gameObject { get; }

        void Init(object config);
        void Dispose();

        void OnAddToGameObject(IGameObject obj);

        void OnUpdate(float timeElasped);
        void OnPostUpdate(float timeElasped);
    }
}
