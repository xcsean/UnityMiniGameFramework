using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityMiniGameFramework
{
    public class BattleNumberEmitter : MonoBehaviour
    {
        public Canvas parentCanvas;
        private static UnityAction<GameObject, int, bool> _createNumAction;

        public static UnityAction<GameObject, int, bool> CreateNumAction
        {
            get => _createNumAction;
        }

        private void OnEnable()
        {
            _createNumAction += CreateBattleNum;
        }

        private void OnDisable()
        {
            _createNumAction -= CreateBattleNum;
        }


        private void CreateBattleNum(GameObject targetGo, int dmg, bool critical)
        {
            var go = UnityGameApp.Inst.UnityResource.LoadUnityPrefabObject(
                "Battle/BattleEffect/BattleDamageNumber/Prefabs/BattleNumber");
            go = Instantiate(go, gameObject.transform, true);
            var numText = go.GetComponentInChildren<UIImageNumText>();
            numText.text = dmg.ToString();
            numText.SetUIPos(targetGo);
            Font font;
            if (critical)
                font = ((UnityResourceManager) UnityGameApp.Inst.Resource).LoadFont("Fonts/FontNumPurple");
            else
                font = ((UnityResourceManager) UnityGameApp.Inst.Resource).LoadFont("Fonts/FontNumRed");

            if (font != null)
                numText.font = font;
        }
    }
}