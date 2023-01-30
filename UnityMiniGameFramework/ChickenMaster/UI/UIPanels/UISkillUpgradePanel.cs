using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class UISkillUpgradePanel: UIPopupPanel
    {
        override public string type => "UISkillUpgradePanel";

        protected VisualElement content;
        public static UISkillUpgradePanel create()
        {
            return new UISkillUpgradePanel();
        }
        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            BindShowActionVE(this._uiObjects["Content"].unityVisualElement);

            content = this._uiObjects["unity-content-container"].unityVisualElement;
            for (int index = 1; index < content.childCount + 1; index++)
            {
                var grid = content.Q<VisualElement>($"grid{index}");
                switch (index)
                {
                    case 1:
                        grid.Q<Button>("ChooseButton").clicked += onChose1;
                        break;
                    case 2:
                        grid.Q<Button>("ChooseButton").clicked += onChose2;
                        break;
                    case 3:
                        grid.Q<Button>("ChooseButton").clicked += onChose3;
                        break;
                    case 4:
                        grid.Q<Button>("ChooseButton").clicked += onChose4;
                        break;
                    case 5:
                        grid.Q<Button>("ChooseButton").clicked += onChose5;
                        break;
                    case 6:
                        grid.Q<Button>("ChooseButton").clicked += onChose6;
                        break;
                }
                grid.style.display = DisplayStyle.None;
            }
        }

        public override void showUI()
        {
            base.hideUI();
            return;
            base.showUI();
            showSkills();
        }

        private void showSkills()
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;

            for (int index = 1; index < content.childCount + 1; index++)
            {
                var grid = content.Q<VisualElement>($"grid{index}");
                grid.style.display = DisplayStyle.None;
            }
            
            for (int index = 1; index <= cmGame.gameConf.gameConfs.skillsConf.Count; index++)
            {
                CMSkillConf cfg = cmGame.gameConf.gameConfs.skillsConf[index];
                int skillLv = 1;
                int nextLv = skillLv >= cfg.levelsConf.Count ? skillLv : skillLv + 1;
                var grid = content.Q<VisualElement>($"grid{index}");
                grid.Q<Label>("title").text = $"{cfg.title}";
                grid.Q<Label>("Skill").text = $"{cfg.desc}:";
                grid.Q<Label>("lv").text = $"LV.{skillLv}";
                grid.Q<Label>("curBuff").text = $"{cfg.levelsConf[skillLv].buff}";
                grid.Q<Label>("nextBuff").text = $"{cfg.levelsConf[nextLv].buff}";
                grid.Q<Label>("price").text = $"{StringUtil.StringNumFormat(cfg.levelsConf[skillLv].upgradeGold.ToString())}";
                grid.Q<VisualElement>("armed").style.display = DisplayStyle.None;
                grid.style.display = DisplayStyle.Flex;
            }
        }

        private List<Func<int>> funcList = new List<Func<int>>();
        private void onClickChoose(int id)
        {
            for (int index = 1; index < content.childCount + 1; index++)
            {
                var grid = content.Q<VisualElement>($"grid{index}");
                grid.Q<VisualElement>("armed").style.display = index == id ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        private void onChose1()
        {
            onClickChoose(1);
        }

        private void onChose2()
        {
            onClickChoose(2);
        }
        private void onChose3()
        {
            onClickChoose(3);
        }

        private void onChose4()
        {
            onClickChoose(4);
        }

        private void onChose5()
        {
            onClickChoose(5);
        }

        private void onChose6()
        {
            onClickChoose(6);
        }

        private void onClickVideo()
        {
            SDKManager.showAutoAd(onVideoCb, "upgrade_skill");
        }

        private void onVideoCb()
        {

        }
    }
}
