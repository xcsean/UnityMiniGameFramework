﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public interface IGame
    {
        Task InitAsync();

        void OnStartSceneLoaded();

        void OnMainSceneLoaded();

        void OnUpdate();
    }
}
