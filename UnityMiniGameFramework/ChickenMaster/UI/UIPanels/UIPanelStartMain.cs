﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniGameFramework;
using UnityEngine.UIElements;
using UnityMiniGameFramework.RESTFulAPI;

namespace UnityMiniGameFramework
{
    public class UIPanelStartMain : UIPanel
    {
        override public string type => "UIPanelStartMain";

        public VisualElement bar;
        public VisualElement barbg;
        public Label barLabel;
        public Button btnStart;
        private long time = 0;
        public static UIPanelStartMain create()
        {
            return new UIPanelStartMain();
        }

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);
            UnityGameApp.Inst.AudioManager.PlayBGM("Audio/BGM/ui_bg");
            btnStart = this._uiObjects["enterGameButton"].unityVisualElement as Button;
            bar = _uiObjects["Bar"].unityVisualElement;
            barbg = _uiObjects["Progress"].unityVisualElement;
            barLabel = _uiObjects["ProgressLabel"].unityVisualElement as Label;


            // TO DO : unregister message
            btnStart.clicked += onEnterGameClick;
            btnStart.style.display = DisplayStyle.None;
            barbg.style.display = DisplayStyle.Flex;
        }

        public void show()
        {
            btnStart.style.display = DisplayStyle.Flex;
            barbg.style.display = DisplayStyle.None;
            //btnStart.style.display = DisplayStyle.None;
            //barbg.style.display = DisplayStyle.Flex;
            //time = (long)(DateTime.Now.Ticks / 10000);

            //UnityGameApp.Inst.addUpdateCall(onUpdate);
        }

        public void onUpdate()
        {
            long nowMillisecond = (long)(DateTime.Now.Ticks / 10000);
            var t = (float)(nowMillisecond - time) / 1000;
            float prog = (float)(t - 2) / 5;
            if (prog > 1f)
            {
                prog = 1.00f;
                barbg.style.display = DisplayStyle.None;
                btnStart.style.display = DisplayStyle.Flex;
                UnityGameApp.Inst.removeUpdateCall(onUpdate);
            }
            bar.style.width = new StyleLength(new Length(prog * 518));
            barLabel.text = $"{Math.Floor(prog * 100)}%";
        }

        public async void onEnterGameClick()
        {
            // 初始化游戏信息
            await UnityGameApp.Inst.Game.InitAsync();

            SDKManager.sdk.FBInit();

            //login
            //UnityGameApp.Inst.RESTFulClient.Login(
            //    new RESTFulAPI.C2S_LoginParam()
            //    {
            //        uid = "",
            //        token = ""
            //    },
            //    (RESTFulAPI.S2C_LoginResult res) =>
            //    {

            //        UnityGameApp.Inst.LoadMainScene();
            //    }
            //);

            //UnityGameApp.Inst.RESTFulClient.Upload(
            //    new BaseUserInfo()
            //    {
            //        uid = "12314",
            //        level = 10,
            //        vipLevel = 10
            //    },
            //    (RESTFulAPI.SC_LoginResult res) =>
            //    {
            //        UnityGameApp.Inst.LoadMainScene();
            //    }
            //);

            //UnityGameApp.Inst.RESTFulClient.Report(
            //     new CS_ReportParam()
            //     {
            //         uid = "12314",
            //         type = 1,
            //         createtime = DateTime.Now,
            //         msg = "测试",
            //     },
            //     (RESTFulAPI.SC_Result res) =>
            //     {

            //     }
            //);

            var cmGame = UnityGameApp.Inst.Game as ChickenMasterGame;
            if (cmGame != null && cmGame.isNewUser)
            {
                UIOpeningCartoonPanel _ui = UnityGameApp.Inst.UI.createUIPanel("OpeningCartoonUI") as UIOpeningCartoonPanel;
                _ui.unityGameObject.transform.SetParent(((MGGameObject)UnityGameApp.Inst.StartScene.uiRootObject).unityGameObject.transform);
                _ui.showUI();
            }
            else
            {
                UnityGameApp.Inst.LoadMainScene();
            }
        }
    }
}
