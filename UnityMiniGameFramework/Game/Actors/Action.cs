using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace UnityMiniGameFramework
{
    abstract public class Act
    {
        protected ActorObject _actor;
        public ActorObject actor => _actor;

        protected bool _actived = false;

        public Act(ActorObject actor)
        {
            _actor = actor;
        }

        public bool isActive
        {
            get
            {
                return _actived;
            }
        }

        virtual public bool isFinished
        {
            get
            {
                return true;
            }
        }

        virtual public bool discardWhenFinish
        {
            get
            {
                return true;
            }
        }

        virtual public bool queueWhenNotStartable
        {
            get
            {
                return false;
            }
        }

        virtual public bool checkStartCondition()
        {
            return true;
        }

        virtual public void Start()
        {
            _actived = true;
        }

        virtual public void Update(float timeElasped)
        {

        }

        virtual public void PostUpdate(float timeElasped)
        {

        }
    }
}
