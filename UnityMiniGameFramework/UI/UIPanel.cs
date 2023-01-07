﻿using System;
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

        public bool isShow => _unityUIDocument.rootVisualElement.style.display == DisplayStyle.Flex;

        private List<Action> _updateCallList = new List<Action>();
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
            //_unityGameObject.SetActive(false);
            //_unityUIDocument.rootVisualElement.SetEnabled(false);
            //_unityUIDocument.rootVisualElement.style.visibility = Visibility.Hidden;
            //_unityUIDocument.rootVisualElement.visible = false;
            _unityUIDocument.rootVisualElement.style.display = DisplayStyle.None;
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.removeUI(this);
            removeUpdate();
        }

        protected void removeUpdate()
        {
            foreach (var callBack in _updateCallList)
            {
                UnityGameApp.Inst.addUpdateCall(callBack);    
            }
            _updateCallList.Clear();
        }
        
        protected void addUpdate(Action callBack)
        {
            _updateCallList.Add(callBack);
            UnityGameApp.Inst.addUpdateCall(callBack);
        }
        
        virtual public void showUI()
        {
            //_unityGameObject.SetActive(true);
            //_unityUIDocument.rootVisualElement.SetEnabled(true);
            //_unityUIDocument.rootVisualElement.style.visibility = Visibility.Visible;
            //_unityUIDocument.rootVisualElement.visible = true;
            _unityUIDocument.rootVisualElement.style.display = DisplayStyle.Flex;
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            cmGame.addUI(this);
        }
        public void setPoisition(int x, int y)
        {
            _unityUIDocument.rootVisualElement.transform.position = new UnityEngine.Vector2(x, y);
        }

        virtual public void display(bool b)
        {
            _unityUIDocument.rootVisualElement.style.display = b ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
