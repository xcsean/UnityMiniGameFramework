using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public interface IMapLevel
    {
        IMap map { get; }

        bool isStarted { get; }

        bool Init(string confName);

        void Start();
        void Finish();

        void OnUpdate(float timeElasped);
        void OnPostUpdate(float timeElasped);
    }
}
