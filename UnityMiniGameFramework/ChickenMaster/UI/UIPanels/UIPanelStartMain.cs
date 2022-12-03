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

        public void onEnterGameClick()
        {
            UnityGameApp.Inst.Game.Init();

            // login
            UnityGameApp.Inst.RESTFulClient.Login(
                new RESTFulAPI.C2S_LoginParam()
                {
                    uid="",
                    token=""
                },
                (RESTFulAPI.S2C_LoginResult res) =>
                {

                    UnityGameApp.Inst.LoadMainScene();
                }
            );

        }
    }
}
