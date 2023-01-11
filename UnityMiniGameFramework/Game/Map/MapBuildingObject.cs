using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    class UnityBuildingColliderTrigger : UnityEngine.MonoBehaviour
    {
        protected MapBuildingObject _mapBuilding;

        private void Start()
        {
            var comp = this.gameObject.transform.parent.gameObject.GetComponent<UnityGameObjectBehaviour>();

            _mapBuilding = comp.mgGameObject as MapBuildingObject;
        }

        private void OnTriggerEnter(UnityEngine.Collider other)
        {
            if (UnityGameApp.Inst.currInitStep != MiniGameFramework.GameAppInitStep.EnterMainScene)
            {
                return;
            }

            _mapBuilding.OnTriggerEnter(this.gameObject.name, other);
        }
        private void OnTriggerStay(UnityEngine.Collider other)
        {
            if (UnityGameApp.Inst.currInitStep != MiniGameFramework.GameAppInitStep.EnterMainScene)
            {
                return;
            }

            _mapBuilding.OnTriggerStay(this.gameObject.name, other);
        }
        private void OnTriggerExit(UnityEngine.Collider other)
        {
            if (UnityGameApp.Inst.currInitStep != MiniGameFramework.GameAppInitStep.EnterMainScene)
            {
                return;
            }

            _mapBuilding.OnTriggerExit(this.gameObject.name, other);
        }
    }

    public class MapBuildingObject : MapActorObject
    {
        protected MapBuildObjectConf _mapBuildingConf;
        protected UIPanel _buildingUIPanel;
        protected Dictionary<string, int> _buildingNearState = new Dictionary<string, int>();

        override public string type => "MapBuildingObject";
        new public static MapBuildingObject create()
        {
            return new MapBuildingObject();
        }
        override protected ActorObjectConfig _getActorConf(string confname)
        {
            if (UnityGameApp.Inst.MapManager.MapConf == null)
            {
                return null;
            }
            _mapBuildingConf = UnityGameApp.Inst.MapManager.MapConf.getMapBuildingConf(confname);
            if(_mapBuildingConf == null)
            {
                return null;
            }

            if (UnityGameApp.Inst.CharacterManager.CharacterConfs == null)
            {
                return null;
            }
            return UnityGameApp.Inst.CharacterManager.CharacterConfs.getActorConf(_mapBuildingConf.actorConfName);
        }

        override public void Init(string confname)
        {
            base.Init(confname);

            this._name = confname;

            // TO DO : init building 

            var tr = this.unityGameObject.transform.Find(_mapBuildingConf.triggerObjectName);
            if(tr == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Map Building [{_name}] without trigger object [{_mapBuildingConf.triggerObjectName}]");
                return;
            }
        }

        public void OnTriggerEnter(string triggerObjectName, UnityEngine.Collider other)
        {
            OnMapBuildingTriggerEnter(triggerObjectName, other);

            (UnityGameApp.Inst.MainScene.map as Map).OnMapBuildingTriggerEnter(triggerObjectName, this, other);
        }
        public void OnTriggerStay(string triggerObjectName, UnityEngine.Collider other)
        {
            OnMapBuildingTriggerStay(triggerObjectName, other);

            //(UnityGameApp.Inst.MainScene.map as Map).OnMapBuildingTriggerEnter(triggerObjectName, this, other);
        }
        public void OnTriggerExit(string triggerObjectName, UnityEngine.Collider other)
        {
            OnMapBuildingTriggerExit(triggerObjectName, other);

            (UnityGameApp.Inst.MainScene.map as Map).OnMapBuildingTriggerExit(triggerObjectName, this, other);
        }
        public void OnMapBuildingTriggerEnter(string triggerObjectName, UnityEngine.Collider other)
        {
            var comp = other.gameObject.GetComponent<UnityGameObjectBehaviour>();
            if(comp != null && comp.mgGameObject.type == "MapHeroObject" && (comp.mgGameObject as MapHeroObject).isSelf)
            {
                // player

                //if (_mapBuildingConf.uiPanelName != null)
                //{
                //    if (_buildingUIPanel == null)
                //    {
                //        // create UI
                //        _buildingUIPanel = UnityGameApp.Inst.UI.createUIPanel(_mapBuildingConf.uiPanelName) as UIPanel;
                //        if (_buildingUIPanel == null)
                //        {
                //            Debug.DebugOutput(DebugTraceType.DTT_Error, $"Map Building [{_name}] without ui panel [{_mapBuildingConf.uiPanelName}]");
                //            return;
                //        }
                //        _buildingUIPanel.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.uiRootObject).unityGameObject.transform);
                //    }
                //    if (_buildingUIPanel is UICommonFactoryPanel _factoryUI)
                //    {
                //        _factoryUI.InitFactoryInfo(unityGameObject.GetComponent<UnityGameObjectBehaviour>().mgGameObjectConfigName);
                //    }
                //    _buildingUIPanel.showUI();
                //}
            }
        }
        public void OnMapBuildingTriggerStay(string triggerObjectName, UnityEngine.Collider other)
        {
            var comp = other.gameObject.GetComponent<UnityGameObjectBehaviour>();
            if(comp != null && comp.mgGameObject.type == "MapHeroObject" && (comp.mgGameObject as MapHeroObject).isSelf)
            {
                // 主角移动中不触发打开界面
                if ((comp.mgGameObject as MapHeroObject).moveAct.isMoving)
                {
                    return;
                }
                int nearState = 0;
                _buildingNearState.TryGetValue(comp.mgGameObject.name, out nearState);
                if (nearState == 0)
                {
                    // last frame is nearby, exit nearby
                    _buildingNearState[comp.mgGameObject.name] = 1;

                    // player
                    if (_mapBuildingConf.uiPanelName != null)
                    {
                        if (_buildingUIPanel == null)
                        {
                            // create UI
                            _buildingUIPanel = UnityGameApp.Inst.UI.createUIPanel(_mapBuildingConf.uiPanelName) as UIPanel;
                            if (_buildingUIPanel == null)
                            {
                                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Map Building [{_name}] without ui panel [{_mapBuildingConf.uiPanelName}]");
                                return;
                            }
                            _buildingUIPanel.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.uiRootObject).unityGameObject.transform);
                        }
                        if (_buildingUIPanel is UICommonFactoryPanel _factoryUI)
                        {
                            _factoryUI.InitFactoryInfo(unityGameObject.GetComponent<UnityGameObjectBehaviour>().mgGameObjectConfigName);
                        }
                        _buildingUIPanel.showUI();
                    }
                }
            }

        }
        public void OnMapBuildingTriggerExit(string triggerObjectName, UnityEngine.Collider other)
        {
            if (_buildingUIPanel != null)
            {
                var comp = other.gameObject.GetComponent<UnityGameObjectBehaviour>();
                if (comp != null && comp.mgGameObject.type == "MapHeroObject" && (comp.mgGameObject as MapHeroObject).isSelf)
                {
                    int nearState = 0;
                    _buildingNearState.TryGetValue(comp.mgGameObject.name, out nearState);
                    if (nearState != 0)
                    {
                        // last frame is nearby, exit nearby
                        _buildingNearState[comp.mgGameObject.name] = 0;
                    }

                    // player

                    _buildingUIPanel.hideUI();
                }
            }
        }
    }
}
