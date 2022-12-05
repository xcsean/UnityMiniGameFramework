﻿using System;
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
            _mapBuilding.OnTriggerEnter(other);
        }
        private void OnTriggerExit(UnityEngine.Collider other)
        {
            _mapBuilding.OnTriggerExit(other);
        }
    }

    class MapBuildingObject : MapActorObject
    {
        protected MapBuildObjectConf _mapBuildingConf;
        protected UIPanel _buildingUIPanel;

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

            if (UnityGameApp.Inst.CharacterManager.CharacterConfs == null)
            {
                return null;
            }
            return UnityGameApp.Inst.CharacterManager.CharacterConfs.getActorConf(_mapBuildingConf.actorConfName);
        }

        override public void Init(string confname)
        {
            base.Init(confname);

            // TO DO : init building 

            var tr = this.unityGameObject.transform.Find(_mapBuildingConf.triggerObjectName);
            if(tr == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Map Building [{_name}] without trigger object [{_mapBuildingConf.triggerObjectName}]");
                return;
            }
        }

        public void OnTriggerEnter(UnityEngine.Collider other)
        {
            var comp = other.gameObject.GetComponent<UnityGameObjectBehaviour>();
            if(comp != null && comp.mgGameObject.type == "MapHeroObject")
            {
                // player

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

                _buildingUIPanel.showUI();
            }
        }
        public void OnTriggerExit(UnityEngine.Collider other)
        {
            var comp = other.gameObject.GetComponent<UnityGameObjectBehaviour>();
            if (comp != null && comp.mgGameObject.type == "MapHeroObject")
            {
                // player

                _buildingUIPanel.hideUI();
            }
        }
    }
}