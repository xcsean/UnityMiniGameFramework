﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using UnityEngine.UIElements;
using MiniGameFramework;

namespace UnityMiniGameFramework
{
    /// <summary>
    /// 英雄头顶未解锁提示
    /// </summary>
    public class UITowerHeroLockHudPanel: UIPanel
    {
        override public string type => "UITowerHeroLockHudPanel";
        public static UITowerHeroLockHudPanel create()
        {
            return new UITowerHeroLockHudPanel();
        }

        protected Label _labLockTip;

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            _labLockTip = this._uiObjects["labLockTip"].unityVisualElement as Label;
        }

        private Transform followTrans;
        private Vector2 screenPos;
        public void SetFollowTarget(Transform trans)
        {
            followTrans = trans;
        }

        public void RefreshInfo(int unlockLevel)
        {
            if (unlockLevel > 0)
            {
                _labLockTip.text = $"Unlock at\r\nbattle level {unlockLevel}";
            }
            else
            {
                _labLockTip.text = "";
            }
        }

        protected void onUpdate()
        {
            if (followTrans != null)
            {
                screenPos = UnityGameApp.Inst.ScreenToUIPos((UnityGameApp.Inst.MainScene.camera as UnityGameCamera)
                    .worldToScreenPos(followTrans.position));
                setPoisition(screenPos.x, screenPos.y - 150);
            }
        }

        public override void showUI()
        {
            base.showUI();

            addUpdate(onUpdate);
        }
    }
}
