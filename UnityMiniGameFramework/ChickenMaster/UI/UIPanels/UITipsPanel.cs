using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

using UnityEngine;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class UITipsPanel : UIPanel
    {
        override public string type => "UITipsPanel";
        public static UITipsPanel create()
        {
            return new UITipsPanel();
        }
        protected class NotifyMessage
        {
            public CMGNotifyType t;
            public string msg;
            public float timeLeft;
            public TemplateContainer tipObj;
        }

        public VisualElement _NotifyBg;
        protected VisualTreeAsset vts_tips;
        protected List<NotifyMessage> _notifyMessages;
        private List<TemplateContainer> tipObjs = new List<TemplateContainer>() { };


        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _NotifyBg = this._uiObjects["NotifyBg"].unityVisualElement;
            vts_tips = ((UnityResourceManager)UnityGameApp.Inst.Resource).LoadUXML("UI/Controls/Tips");
            _notifyMessages = new List<NotifyMessage>();
        }

        public override void showUI()
        {
            base.showUI();

            UnityGameApp.Inst.addUpdateCall(OnUpdate);
        }

        public override void hideUI()
        {
            base.hideUI();

            UnityGameApp.Inst.removeUpdateCall(OnUpdate);
        }

        private TemplateContainer getTipsObj(string tip)
        {
            TemplateContainer tipsObj;
            if (tipObjs.Count == 0)
            {
                tipsObj = vts_tips.CloneTree();
                _NotifyBg.Add(tipsObj);
            }
            else
            {
                tipsObj = tipObjs[0];
                tipObjs.RemoveAt(0);
            }
            tipsObj.transform.position = new Vector3(0, 0);
            tipsObj.style.display = DisplayStyle.Flex;
            tipsObj.Q<Label>("tipLabel").text = $"{tip}";
            return tipsObj;
        }
        public void NofityMessage(CMGNotifyType t, string msg)
        {
            var notify = new NotifyMessage()
            {
                t = t,
                msg = msg,
                tipObj = getTipsObj(msg),
                timeLeft = 1.0f // 3 seconds
            };

            _notifyMessages.Add(notify);
            if (_notifyMessages.Count > 7)
            {
                TemplateContainer obj = _notifyMessages[0].tipObj;
                obj.style.display = DisplayStyle.None;
                tipObjs.Add(obj);
                _notifyMessages.RemoveAt(0);
            }
        }
        public void OnUpdate()
        {
            if (_notifyMessages == null)
            {
                return;
            }

            for (int i = 0; i < _notifyMessages.Count; ++i)
            {
                var notify = _notifyMessages[i];
                notify.timeLeft -= UnityEngine.Time.deltaTime;

                if (notify.timeLeft <= 0)
                {
                    notify.tipObj.style.display = DisplayStyle.None;
                    tipObjs.Add(notify.tipObj);
                    _notifyMessages.RemoveAt(i);
                    --i;
                }
                else if (notify.timeLeft >= 0.8f)
                {
                    notify.tipObj.transform.position -= new Vector3(0, Time.deltaTime * 200);
                }
            }
        }
    }
}
