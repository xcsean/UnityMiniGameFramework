using MiniGameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.UIElements;

namespace UnityMiniGameFramework
{
    public class UIPanelStartMain : UIPanel
    {
        override public string type => "UIPanelStartMain";

        public ProgressBar bar;
        public VisualElement barbg;
        public Button btnStart;
        private int time = 0;
        private System.Timers.Timer timer;
        public static UIPanelStartMain create()
        {
            return new UIPanelStartMain();
        }

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            btnStart = this._uiObjects["enterGameButton"].unityVisualElement as Button;


            // TO DO : unregister message
            btnStart.clicked += onEnterGameClick;
            bar = _uiObjects["Bar"].unityVisualElement as ProgressBar;
            barbg = _uiObjects["Progress"].unityVisualElement;
            btnStart.visible = true;
            barbg.visible = false;
        }

        public void show()
        {

            btnStart.visible = false;
            barbg.visible = true;
            time = 0;
            //timer = new System.Threading.Timer(new System.Threading.TimerCallback(onProgress), null, 0, 100);
            //timer.Change(0, 100);
            timer = new System.Timers.Timer(1000);
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(onProgress);
        }

        public async void onEnterGameClick()
        {
            // 初始化游戏信息
            await UnityGameApp.Inst.Game.InitAsync();

            // login
            //UnityGameApp.Inst.RESTFulClient.Login(
            //    new RESTFulAPI.C2S_LoginParam()
            //    {
            //        uid="",
            //        token=""
            //    },
            //    (RESTFulAPI.S2C_LoginResult res) =>
            //    {

            //        UnityGameApp.Inst.LoadMainScene();
            //    }
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

        private void onProgress(object sender,System.Timers.ElapsedEventArgs e)
        {
            time ++;
            float prog = (float)time / 10;
            Debug.DebugOutput(DebugTraceType.DTT_Debug, $"????????{time}---{prog}");
            if (prog > 1f)
            {
                barbg.visible = false;
                btnStart.visible = true;
                //timer.Change(-1, -1);
                timer.AutoReset = false;
            }
            else
            {
                bar.style.width = new StyleLength(new Length(prog * 334));
            }

        }
    }
}
