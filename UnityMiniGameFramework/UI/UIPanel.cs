using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class UIPanel : IUIPanel
    {
        protected string _name;
        public string name => _name;

        virtual public string type => "UIPanel";

        protected UnityEngine.GameObject _unityGameObject;
        public UnityEngine.GameObject unityGameObject => _unityGameObject;

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

            foreach(var ctrlConf in conf.controls)
            {
                var tr = _unityGameObject.transform.Find(ctrlConf.name);
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

                uiObj.onInit(ctrlConf, tr.gameObject);

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
