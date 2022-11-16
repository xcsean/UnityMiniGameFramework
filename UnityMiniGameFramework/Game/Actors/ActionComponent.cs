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

        public List<Act> penddingActions => _penddingActions;
        public List<Act> currActivateActions => _currActivateActions.ToList();

        public ActionComponent()
        {
            _penddingActions = new List<Act>();
            _currActivateActions = new HashSet<Act>();
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

        override public void OnUpdate(uint timeElasped)
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
        }
        override public void OnPostUpdate(uint timeElasped)
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
        }
    }
}
