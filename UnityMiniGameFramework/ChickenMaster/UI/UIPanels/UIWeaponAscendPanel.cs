﻿using System;
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
        protected Label _labDamageDesc;
        protected Label _labSpeedDesc;
        protected Label _labRangeDesc;

        public override void Init(UIPanelConf conf)
        {
            base.Init(conf);

            FindUI();
        }

        protected void FindUI()
        {
            BindShowActionVE(this._uiObjects["Content"].unityVisualElement);

            _labDamage = this._uiObjects["labDamage"].unityVisualElement as Label;
            _labSpeed = this._uiObjects["labSpeed"].unityVisualElement as Label;
            _labRange = this._uiObjects["labRange"].unityVisualElement as Label;
            _labDamageDesc = this._uiObjects["labDamageDesc"].unityVisualElement as Label;
            _labSpeedDesc = this._uiObjects["labSpeedDesc"].unityVisualElement as Label;
            _labRangeDesc = this._uiObjects["labRangeDesc"].unityVisualElement as Label;
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
                _sprGunIcon.style.backgroundImage = null;

                _labDamage.style.display = DisplayStyle.None;
                _labSpeed.style.display = DisplayStyle.None;
                _labRange.style.display = DisplayStyle.None;
                _labDamageDesc.style.display = DisplayStyle.None;
                _labSpeedDesc.style.display = DisplayStyle.None;
                _labRangeDesc.style.display = DisplayStyle.None;
            }
            else
            {
                ChickenMasterGame cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
                var gunConf = cmGame.gameConf.getCMGunConf(gunInfo.id);

                _labDamage.text = $"{gunConf.gunLevelConf[gunInfo.level].attack.attackMin}";
                _labSpeed.text = $"{gunConf.gunLevelConf[gunInfo.level].IncreasedAttackSpeed}";
                _labRange.text = "0";

                _labGunName.text = $"{gunConf.name}";
                _labGunStar.text = $"{gunInfo.level}";

                var tx = ((UnityResourceManager)UnityGameApp.Inst.Resource).LoadTexture($"icons/weapons/{gunConf.weaponIcon}");
                if (tx != null)
                {
                    _sprGunIcon.style.backgroundImage = tx;
                    _sprGunIcon.style.width = tx.width;
                    _sprGunIcon.style.height = tx.height;
                }

                _labDamage.style.display = gunConf.gunLevelConf[gunInfo.level].attack.attackMin > 0 ? DisplayStyle.Flex : DisplayStyle.None;
                _labSpeed.style.display = gunConf.gunLevelConf[gunInfo.level].IncreasedAttackSpeed > 0 ? DisplayStyle.Flex : DisplayStyle.None;
                _labRange.style.display = DisplayStyle.None;
                _labDamageDesc.style.display = gunConf.gunLevelConf[gunInfo.level].attack.attackMin > 0 ? DisplayStyle.Flex : DisplayStyle.None;
                _labSpeedDesc.style.display = gunConf.gunLevelConf[gunInfo.level].IncreasedAttackSpeed > 0 ? DisplayStyle.Flex : DisplayStyle.None;
                _labRangeDesc.style.display = DisplayStyle.None;
            }

            showUI();
        }

        public override void hideUI()
        {
            base.hideUI();
        }

    }
}
