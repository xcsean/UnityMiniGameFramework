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
        public static UIPanelStartMain create()
        {
            return new UIPanelStartMain();
        }

        override public void Init(UIPanelConf conf)
        {
            base.Init(conf);

            var ctrl = this._uiObjects["enterGameButton"];

            var btn = ctrl.unityVisualElement as Button;


            // TO DO : unregister message
            btn.clicked += onEnterGameClick;
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
    }
}
