using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.UIElements;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class UIWeaponAscendPanel : UIPopupPanel
    {
        override public string type => "UIWeaponAscendPanel";

        public static UIWeaponAscendPanel create()
        {
            return new UIWeaponAscendPanel();
        }

        protected VisualElement _sprGunIcon;
        protected Label _labGunName;
        protected Label _labGunStar;
        protected Label _labDamage;
        protected Label _labSpeed;
        protected Label _labRange;

        public override void Init(UIPanelConf conf)
        {
            base.Init(conf);

            FindUI();
        }

        protected void FindUI()
        {
            _labDamage = this._uiObjects["labDamage"].unityVisualElement as Label;
            _labSpeed = this._uiObjects["labSpeed"].unityVisualElement as Label;
            _labRange = this._uiObjects["labRange"].unityVisualElement as Label;
            _labGunName = this._uiObjects["labGunName"].unityVisualElement as Label;
            _labGunStar = this._uiObjects["labGunStar"].unityVisualElement as Label;
            _sprGunIcon = this._uiObjects["sprGunIcon"].unityVisualElement;
        }

        /// <summary>
        /// 武器信息
        /// </summary>
        public void SetWeaponInfo(LocalWeaponInfo gunInfo)
        {
            if (gunInfo == null)
            {
                _labDamage.text = "0";
                _labSpeed.text = "0";
                _labRange.text = "0";

                _labGunName.text = "";
                _labGunStar.text = "0";
            }
            else
            {
                ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                var gunConf = cmGame.gameConf.getCMGunConf(gunInfo.id);

                _labDamage.text = "0";
                _labSpeed.text = "0";
                _labRange.text = "0";

                _labGunName.text = $"{gunConf.id}";
                _labGunStar.text = $"{gunInfo.level}";
            }

            showUI();
        }

        public override void showUI()
        {
            base.showUI();
            unityUIDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        }

        public override void hideUI()
        {
            base.hideUI();
            unityUIDocument.rootVisualElement.style.display = DisplayStyle.None;
        }
    }
}
