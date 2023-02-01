using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityMiniGameFramework
{
    public class BattleNumberEmitter : MonoBehaviour
    {
        public Canvas parentCanvas;
        private static UnityAction<GameObject, int, DamageTypeEnum> _createNumAction;

        public static UnityAction<GameObject, int, DamageTypeEnum> CreateNumAction
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


        private void CreateBattleNum(GameObject targetGo, int dmg, DamageTypeEnum damageType)
        {
            var go = UnityGameApp.Inst.UnityResource.LoadUnityPrefabObject(
                "Battle/BattleEffect/BattleDamageNumber/Prefabs/BattleNumber");
            go = Instantiate(go, gameObject.transform, true);
            var numText = go.GetComponentInChildren<UIImageNumText>();
            numText.text = dmg.ToString();
            numText.SetUIPos(targetGo);
            Font font = null;
            switch (damageType)
            {
                case DamageTypeEnum.Attack:
                    font = ((UnityResourceManager) UnityGameApp.Inst.Resource).LoadFont("Fonts/FontNumRed");
                    break;
                case DamageTypeEnum.Critical:
                    font = ((UnityResourceManager) UnityGameApp.Inst.Resource).LoadFont("Fonts/FontNumPurple");
                    break;
                case DamageTypeEnum.Dot:
                    font = ((UnityResourceManager) UnityGameApp.Inst.Resource).LoadFont("Fonts/FontNumYellow");
                    break;
            }

            if (font != null)
                numText.font = font;
        }
    }
}