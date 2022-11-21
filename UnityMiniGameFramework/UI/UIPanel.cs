using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UIPanel : IUIPanel
    {
        protected string _name;
        public string name => _name;

        virtual public string type => "UIPanel";

        protected UnityEngine.GameObject _unityGameObject;
        public UnityEngine.GameObject unityGameObject => _unityGameObject;

        protected UIDocument _unityUIDocument;
        public UIDocument unityUIDocument => _unityUIDocument;

        protected Dictionary<string, UIObject> _uiObjects;

        public UIPanel()
        {
            _uiObjects = new Dictionary<string, UIObject>();
        }

        virtual public void Init(UIPanelConf conf)
        {
            _name = conf.name;
            _unityGameObject = UnityGameApp.Inst.UnityResource.CreateUnityPrefabObject(conf.uiFile);
            if(_unityGameObject == null)
            {
                return;
            }

            // The UXML is already instantiated by the UIDocument component
            _unityUIDocument = _unityGameObject.GetComponent<UIDocument>();

            foreach (var ctrlConf in conf.controls)
            {
                var tr = _unityUIDocument.rootVisualElement.Q(ctrlConf.name);
                if(tr == null)
                {
                    Debug.DebugOutput(DebugTraceType.DTT_Error, $"UIPanel {_name} control [{ctrlConf.name}] not exist");
                    continue;
                }

                UIObject uiObj = (UIObject)UnityGameApp.Inst.UI.createUIObject(ctrlConf.type);
                if(uiObj == null)
                {
                    Debug.DebugOutput(DebugTraceType.DTT_Error, $"UIPanel {_name} control {ctrlConf.name} type [{ctrlConf.type}] not exist");
                    continue;
                }

                uiObj.onInit(ctrlConf, tr);

                _uiObjects[ctrlConf.name] = uiObj;
            }
            // TO DO : 
        }

        virtual public void Dispose()
        {
            throw new NotImplementedException();
        }

        virtual public IUIObject getUIObject(string name)
        {
            if (!_uiObjects.ContainsKey(name))
            {
                return null;
            }

            return _uiObjects[name];
        }

        virtual public void hideUI()
        {
            _unityGameObject.SetActive(false);
        }

        virtual public void showUI()
        {
            _unityGameObject.SetActive(true);
        }
    }
}
