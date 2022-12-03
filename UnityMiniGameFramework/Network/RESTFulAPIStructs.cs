using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework.RESTFulAPI
{
    public class C2S_LoginParam
    {
        public string uid { get; set; }
        public string token { get; set; }
    }
    public class S2C_LoginResult
    {
        public BaseUserInfo baseInfo { get; set; }
    }
}
