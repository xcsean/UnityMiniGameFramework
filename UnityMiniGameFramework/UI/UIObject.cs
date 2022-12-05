using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UIObject : IUIObject
    {
        virtual public string type => "UIObject";
        public static UIObject create()
        {
            return new UIObject();
        }

        protected string _name;
        public string name => _name;

        protected VisualElement _unityVE;
        public VisualElement unityVisualElement => _unityVE;
        protected Dictionary<string, UIObject> _subControls;

        public int x => (int)_unityVE.transform.position.x;

        public int y => (int)_unityVE.transform.position.y;

        public int width => (int)_unityVE.layout.width;

        public int height => (int)_unityVE.layout.height;

        public UIObject()
        {
            _subControls = new Dictionary<string, UIObject>();
        }

        virtual public void hideUI()
        {
            _unityVE.style.visibility = Visibility.Hidden;
        }

        virtual public void showUI()
        {
            _unityVE.style.visibility = Visibility.Visible;
        }

        virtual public void onInit(UIControlConf c, VisualElement o)
        {
            _name = c.name;
            _unityVE = o;

            if(c.subControls != null)
            {
                foreach (var ctrlConf in c.subControls)
                {
                    var tr = _unityVE.Q(ctrlConf.name);
                    if (tr == null)
                    {
                        MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"UIPanel {_name} control [{ctrlConf.name}] not exist");
                        continue;
                    }

                    UIObject uiObj = (UIObject)UnityGameApp.Inst.UI.createUIObject(ctrlConf.type);
                    if (uiObj == null)
                    {
                        MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"UIPanel {_name} control {ctrlConf.name} type [{ctrlConf.type}] not exist");
                        continue;
                    }

                    uiObj.onInit(ctrlConf, tr);

                    _subControls[ctrlConf.name] = uiObj;
                }
            }
            // TO DO : 
        }


        public void setPoisition(int x, int y)
        {
            _unityVE.transform.position = new UnityEngine.Vector2(x, y);
        }
    }
}
