using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public interface IScene
    {
        string name { get; }

        IGameObject sceneRootObj { get; }
        IGameObject uiRootObject { get; }

        ICamera camera { get; }

        AsyncOpStatus loadStatus { get; }
        AsyncOpStatus unloadStatus { get; }

        void LoadAsync();
        void UnloadAsync();

        void OnShow();
        void OnHide();

        void OnUpdate();
    }
}
