using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;
using UnityEngine.UIElements;
using UnityEngine;

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

        public int x => (int)_unityUIDocument.rootVisualElement.transform.position.x;

        public int y => (int)_unityUIDocument.rootVisualElement.transform.position.y;

        public int width => (int)_unityUIDocument.rootVisualElement.layout.width;

        public int height => (int)_unityUIDocument.rootVisualElement.layout.height;

        // 界面是否显示中
        protected bool _isShow = false;
        public bool isShow => _isShow || _unityUIDocument.rootVisualElement.style.display == DisplayStyle.Flex;

        public Action onShowStartHandle { get; set; }
        public Action onHideEndHandle { get; set; }

        private VisualElement _showActionVE;

        private Action _callBack = null;

        public UIPanel()
        {
            _uiObjects = new Dictionary<string, UIObject>();
        }

        virtual public void Init(UIPanelConf conf)
        {
            // TO DO : 
            _name = conf.name;
            _unityGameObject = new UnityEngine.GameObject($"UIPanel_{_name}");
            _unityUIDocument = _unityGameObject.AddComponent<UIDocument>();
            _unityUIDocument.sortingOrder = conf.sortOrder;

            _isShow = true;

            //var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/MyCustomEditor.uxml");
            var visualTree = Resources.Load<VisualTreeAsset>(conf.uiFile); // TO DO : use resource manager
            _unityUIDocument.visualTreeAsset = visualTree;
            _unityUIDocument.panelSettings = UnityGameApp.Inst.unityUIPanelSettings;

            foreach (var ctrlConf in conf.controls)
            {
                var tr = _unityUIDocument.rootVisualElement.Q(ctrlConf.name);
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

                _uiObjects[ctrlConf.name] = uiObj;
            }
     
        }

        virtual public void SetSortOrder(int order)
        {
            _unityUIDocument.sortingOrder = order;
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

        protected void removeUpdate()
        {
            if (_callBack != null)
            {
                UnityGameApp.Inst.removeUpdateCall(_callBack);
                _callBack = null;
            }
        }
        
        protected void addUpdate(Action callBack)
        {
            if (_callBack != null)
                removeUpdate();
            _callBack = callBack;
            UnityGameApp.Inst.addUpdateCall(callBack);
        }

        virtual public void hideUI()
        {
            //_unityGameObject.SetActive(false);
            //_unityUIDocument.rootVisualElement.SetEnabled(false);
            //_unityUIDocument.rootVisualElement.style.visibility = Visibility.Hidden;
            //_unityUIDocument.rootVisualElement.visible = false;
            //_unityUIDocument.rootVisualElement.style.display = DisplayStyle.None;
            //_unityUIDocument.rootVisualElement.style.opacity = 0f;

            if (onHideEndHandle != null)
            {
                onHideEndHandle();
            }
            HideAction();

            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.removeUI(this);
            removeUpdate();
        }

        virtual public void showUI()
        {
            //_unityGameObject.SetActive(true);
            //_unityUIDocument.rootVisualElement.SetEnabled(true);
            //_unityUIDocument.rootVisualElement.style.visibility = Visibility.Visible;
            //_unityUIDocument.rootVisualElement.visible = true;
            //_unityUIDocument.rootVisualElement.style.display = DisplayStyle.Flex;
            //_unityUIDocument.rootVisualElement.style.opacity = 1f;

            if (onShowStartHandle != null)
            {
                onShowStartHandle();
            }

            ShowAction();

            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.addUI(this);
        }
        public void setPoisition(int x, int y)
        {
            _unityUIDocument.rootVisualElement.transform.position = new Vector2(x, y);
        }

        virtual public void display(bool b)
        {
            if (b)
            {
                ShowAction();
            }
            else
            {
                HideAction();
            }
        }

        /// <summary>
        /// 绑定弹窗缩放对象，在ShowAction和HideAction之前执行
        /// </summary>
        public void BindShowActionVE(VisualElement showVE)
        {
            _showActionVE = showVE;
            if (_showActionVE != null)
            {
                _showActionVE.style.scale = new StyleScale(new Scale(new Vector3(0.3f, 0.3f, 1f)));
                _showActionVE.style.opacity = 0f;
            }
        }

        /// <summary>
        /// 界面显示时缩放动画
        /// </summary>
        virtual public void ShowAction()
        {
            if (_showActionVE != null && !_showActionVE.ClassListContains("unity-scale-show"))
            {
                var uss = ((UnityResourceManager)UnityGameApp.Inst.Resource).LoadStyleSheet($"unity-scale-show");
                if (uss == null)
                {
                    MiniGameFramework.Debug.DebugOutput(DebugTraceType.DTT_Error, $"UIPanel {_name}, uss[unity-scale-show] not exist");
                    return;
                }
                _unityUIDocument.rootVisualElement.styleSheets.Add(uss);
                _showActionVE.AddToClassList("unity-scale-show");
            }
            if (_showActionVE != null && _showActionVE.ClassListContains("unity-scale-show"))
            {
                _showActionVE.style.scale = new StyleScale(new Scale(new Vector3(1f, 1f, 1f)));
                _showActionVE.style.opacity = 1f;
            }
            else
            {
                _unityUIDocument.rootVisualElement.style.display = DisplayStyle.Flex;
            }
            _isShow = true;
        }

        /// <summary>
        /// 界面隐藏时缩放动画
        /// </summary>
        virtual public void HideAction()
        {
            if (_showActionVE != null && _showActionVE.ClassListContains("unity-scale-show"))
            {
                _showActionVE.style.scale = new StyleScale(new Scale(new Vector3(0.3f, 0.3f, 1f)));
                _showActionVE.style.opacity = 0f;
            }
            else
            {
                _unityUIDocument.rootVisualElement.style.display = DisplayStyle.None;
            }
            _isShow = false;
        }
    }
}
