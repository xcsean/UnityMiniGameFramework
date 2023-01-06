﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class UISkillUpgradePanel: UIPanel
    {
        override public string type => "UISkillUpgradePanel";

        protected Button _closeBtn;
        protected VisualElement content;
        public static UISkillUpgradePanel create()
        {
            return new UISkillUpgradePanel();
        }
        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _closeBtn = this._uiObjects["CloseButton"].unityVisualElement as Button;
            _closeBtn.clicked += onClickClose;
            content = this._uiObjects["unity-content-container"].unityVisualElement;
        }

        public override void showUI()
        {
            base.showUI();
            showSkills();
        }

        private void onClickClose()
        {
            hideUI();
        }

        private void showSkills()
        {
            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;

            for (int index = 1; index < content.childCount + 1; index++)
            {
                var grid = content.Q<VisualElement>($"grid{index}");
                grid.visible = false;
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
                grid.Q<VisualElement>("armed").visible = false;
                //grid.Q<Button>("ChooseButton").clicked += onClickChoose;
                //var func = new Func<int, int>(funct);
                grid.visible = true;
            }
        }

        private List<Func<int>> funcList = new List<Func<int>>();
        private void onClickChoose()
        {

        }

        private int funct(int a)
        {
            return 1;
        }

    }
}
