using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class ActBuf
    {
        protected ActorObject _actor;
        public ActorObject actor => _actor;

        protected float _timeLeft;
        public float timeLeft => _timeLeft;

        public ActBuf(ActorObject actor)
        {
            _actor = actor;
        }

        // TO DO : implement ActBuf

        protected virtual bool _checkTimeFinish()
        {
            _timeLeft -= UnityEngine.Time.deltaTime;
            if(_timeLeft > 0)
            {
                return false;
            }

            _timeLeft = 0;

            _actor.actionComponent.OnBufRemove(this);

            return true;
        }

        public virtual void OnAdd()
        {

        }

        public virtual void OnRemove()
        {

        }

        public virtual void OnUpdate()
        {

        }

        public virtual void OnPostUpdate()
        {
            _checkTimeFinish();
        }
    }
}
