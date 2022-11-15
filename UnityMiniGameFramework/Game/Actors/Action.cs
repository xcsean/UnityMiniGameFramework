using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace UnityMiniGameFramework.Game.Character
{
    public class Action
    {
        protected ActorComponent _actor;

        public ActorComponent actor => _actor;

        public bool isFinished
        {
            get
            {
                return true;
            }
        }

        public void Update()
        {

        }
    }
}
