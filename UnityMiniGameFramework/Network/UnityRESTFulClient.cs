using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;
using UnityMiniGameFramework.RESTFulAPI;

namespace UnityMiniGameFramework
{
    public class UnityRESTFulClient : RESTFulClient
    {
        public void Login(C2S_LoginParam par, Action<S2C_LoginResult> cb)
        {
            this.DoAction<S2C_LoginResult, C2S_LoginParam>("login", par, (RCPostActResult res) =>
            {
                cb(res.content as S2C_LoginResult);
            });
        }
    }
}
