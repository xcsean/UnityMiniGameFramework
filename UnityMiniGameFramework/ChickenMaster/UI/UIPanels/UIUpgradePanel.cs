using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UIUpgradePanel : UIPopupPanel
    {
        override public string type => "UIUpgradePanel";
        public static UIUpgradePanel create()
        {
            return new UIUpgradePanel();
        }

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            var FlameGunBtn = this._uiObjects["FlameGun"].unityVisualElement as Button;
            FlameGunBtn.RegisterCallback<MouseUpEvent>(onFlameGunClick);
            var MachineGunBtn = this._uiObjects["MachineGun"].unityVisualElement as Button;
            MachineGunBtn.RegisterCallback<MouseUpEvent>(onMachineGunClick);
            var ElectricGunBtn = this._uiObjects["ElectricGun"].unityVisualElement as Button;
            ElectricGunBtn.RegisterCallback<MouseUpEvent>(onElectricGunClick);
            var BlastGunBtn = this._uiObjects["BlastGun"].unityVisualElement as Button;
            BlastGunBtn.RegisterCallback<MouseUpEvent>(onBlastGunClick);
        }

        protected void _changeGun(int gunID)
        {
            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            var baseInfo = cmGame.baseInfo.getData() as LocalBaseInfo;
            baseInfo.hero.holdWeapon.id = gunID;

            cmGame.baseInfo.markDirty();

            cmGame.Self.onChangeGun();
        }

        public void onFlameGunClick(MouseUpEvent e)
        {
            _changeGun(3);
        }
        public void onMachineGunClick(MouseUpEvent e)
        {
            _changeGun(1);
        }
        public void onElectricGunClick(MouseUpEvent e)
        {
            _changeGun(4);
        }
        public void onBlastGunClick(MouseUpEvent e)
        {
            _changeGun(2);
        }
    }
}
