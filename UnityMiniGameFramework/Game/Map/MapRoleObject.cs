using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    abstract public class MapRoleObject : MapActorObject
    {
        protected RigibodyMoveAct _rigiMovAct;
        public RigibodyMoveAct moveAct => _rigiMovAct;

        override public void Init(string confname)
        {
            base.Init(confname);

            _rigiMovAct = new RigibodyMoveAct(this);
            this.actionComponent.AddAction(_rigiMovAct);
        }
    }
}
