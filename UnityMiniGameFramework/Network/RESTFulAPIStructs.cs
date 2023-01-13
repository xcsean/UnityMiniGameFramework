using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework.RESTFulAPI
{
    public class SC_Result
    {
        public bool succeed { get; set; }
        public string tips { get; }
    }

    public class CS_LoginParam
    {
        public string uid { get; set; }

        public string token { get; set; }
    }

    public class SC_LoginResult
    {
        public BaseUserInfo baseInfo { get; set; }
    }

    public class CS_ReportParam
    {
        public string uid { get; set; }

        public int type { get; set; }

        public int subType { get; set; }


        public DateTime createtime { get; set; }

        public string point { get; set; }
        public string msg { get; set; }
    }

    public class CS_ReportParamList
    {
        public CS_ReportParam[] list { get; set; }
    }
}
