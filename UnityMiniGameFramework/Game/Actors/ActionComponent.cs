using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class ActionComponentConfig
    {
        public string componentType { get; set; }

    }

    public class ActionComponent : StateComponent
    {
        override public string type => "ActionComponent";
        new public static ActionComponent create()
        {
            return new ActionComponent();
        }

        protected List<Act> _penddingActions;
        protected HashSet<Act> _currActivateActions;

        protected HashSet<ActBuf> _bufs;

        public List<Act> penddingActions => _penddingActions;
        public List<Act> currActivateActions => _currActivateActions.ToList();

        public ActionComponent()
        {
            _penddingActions = new List<Act>();
            _currActivateActions = new HashSet<Act>();

            _bufs = new HashSet<ActBuf>();
        }

        public void AddBuf(ActBuf buf)
        {
            _bufs.Add(buf);

            buf.OnAdd();
        }

        public void OnBufRemove(ActBuf buf)
        {
            buf.OnRemove();

            _bufs.Remove(buf);
        }

        public void AddAction(Act act)
        {
            if(act.checkStartCondition())
            {
                _currActivateActions.Add(act);

                act.Start();
            }
            else if(act.queueWhenNotStartable)
            {
                _penddingActions.Add(act);
            }
        }

        public void ClearActions()
        {
            _currActivateActions.Clear();
            _penddingActions.Clear();
        }

        override public void Init(object config)
        {
            base.Init(config);
            
        }
        override public void Dispose()
        {
            base.Dispose();

            _currActivateActions = null;
            _penddingActions = null;
        }

        override public void OnUpdate(float timeElasped)
        {
            if(_currActivateActions != null)
            {
                foreach (var act in _currActivateActions)
                {
                    act.Update((uint)(Time.deltaTime * 1000));
                }
            }

            if(_penddingActions != null)
            {
                List<Act> pendAct = _penddingActions;
                _penddingActions = new List<Act>();
                foreach (var act in pendAct)
                {
                    if(act.checkStartCondition())
                    {
                        act.Start();

                        _currActivateActions.Add(act);
                    }
                    else
                    {
                        _penddingActions.Add(act);
                    }
                }
            }

            if(_bufs != null)
            {
                foreach (var buf in _bufs)
                {
                    buf.OnUpdate();
                }
            }
        }
        override public void OnPostUpdate(float timeElasped)
        {
            if (_currActivateActions != null)
            {
                List<Act> toDelAct = new List<Act>();
                foreach (var act in _currActivateActions)
                {
                    act.PostUpdate((uint)(Time.deltaTime * 1000));

                    if (act.isFinished && act.discardWhenFinish)
                    {
                        toDelAct.Add(act);
                    }
                }
                foreach (var act in toDelAct)
                {
                    _currActivateActions.Remove(act);
                }
            }

            if (_bufs != null)
            {
                ActBuf[] bufArray = _bufs.ToArray();
                foreach (var buf in bufArray)
                {
                    buf.OnPostUpdate();
                }
            }
        }
    }
}
