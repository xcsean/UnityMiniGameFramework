using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class FootPrintComponent : UnityEngine.MonoBehaviour
    {
        public UnityEngine.GameObject ActorObject;
        public string RunDustVFX = "runDust";
        public string FootPrintVFX = "footPrint";

        private void Start()
        {
            //var comp = this.gameObject.transform.parent.gameObject.GetComponent<UnityGameObjectBehaviour>();

            //_mapBuilding = comp.mgGameObject as MapBuildingObject;
        }

        private void OnTriggerEnter(UnityEngine.Collider other)
        {
            if(UnityGameApp.Inst.currInitStep != MiniGameFramework.GameAppInitStep.EnterMainScene)
            {
                return;
            }

            if (other.gameObject.layer != UnityEngine.LayerMask.NameToLayer("Ground"))
            {
                return;
            }

            // create dust
            var dust = UnityGameApp.Inst.VFXManager.createVFXObject(RunDustVFX);
            if (dust != null)
            {
                dust.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);
                dust.unityGameObject.transform.position = this.gameObject.transform.position;
                dust.unityGameObject.transform.forward = ActorObject.transform.forward;
            }

            // create foot print
            var footPrint = UnityGameApp.Inst.VFXManager.createVFXObject(FootPrintVFX);
            if (footPrint != null)
            {
                footPrint.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.MainScene.sceneRootObj).unityGameObject.transform);
                footPrint.unityGameObject.transform.position = this.gameObject.transform.position;
                footPrint.unityGameObject.transform.forward = ActorObject.transform.forward;
            }
        }
        //private void OnTriggerExit(UnityEngine.Collider other)
        //{
        //    _mapBuilding.OnTriggerExit(other);
        //}
    }

}
