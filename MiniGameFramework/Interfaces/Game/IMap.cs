using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public interface IMap
    {
        void Init(string confname);
        void OnEnter();
        void OnLeave();
        void Dispose();
        void OnUpdate(float timeElasped);
        void OnPostUpdate(float timeElasped);

        IMapLevel CreateLevel(string levelName);
        IMapLevel currentLevel { get; }

    }
}
