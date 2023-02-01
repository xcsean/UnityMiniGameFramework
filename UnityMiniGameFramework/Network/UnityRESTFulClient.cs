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
        public void Login(CS_LoginParam par, Action<SC_LoginResult> cb)
        {
            this.DoAction<SC_LoginResult, CS_LoginParam>("login", par, (RCPostActResult res) =>
            {
                cb(res.content as SC_LoginResult);
            });
        }

        public void Upload(BaseUserInfo par, Action<SC_LoginResult> cb)
        {
            this.DoAction<SC_LoginResult, BaseUserInfo>("upload", par, (RCPostActResult res) =>
            {
                cb(res.content as SC_LoginResult);
            });
        }

        /// <summary>
        /// 埋点上报
        /// </summary>
        public void Report(CS_ReportParam par, Action<SC_Result> cb = null)
        {
            var _userInfo = (UnityGameApp.Inst.Game as ChickenMasterGame).userInfo;
            if (_userInfo != null && par.uid == null)
            {
                par.uid = (_userInfo.getData() as LocalUserInfo).uuid;
            }
            else if (par.uid == null)
            {
                par.uid = "guest1";
            }
            par.createtime = DateTime.Now;

            this.DoAction<SC_Result, CS_ReportParam>("report", par, (RCPostActResult res) =>
            {
                Debug.DebugOutput(DebugTraceType.DTT_Debug, $"UnityRESTFulClient report result:{res.content}");
                cb(res.content as SC_Result);
            });
        }

        public void ReportList(CS_ReportParamList par, Action<SC_Result> cb = null)
        {
            var _userInfo = (UnityGameApp.Inst.Game as ChickenMasterGame).userInfo;

            this.DoAction<SC_Result, CS_ReportParamList>("reportList", par, (RCPostActResult res) =>
            {
                Debug.DebugOutput(DebugTraceType.DTT_Debug, $"UnityRESTFulClient report result:{res.content}");
                cb(res.content as SC_Result);
            });
        }

        public void ReportList(CS_ReportParam par, List<string> _msgs, Action<SC_Result> cb = null)
        {
            if (_msgs.Count == 1)
            {
                par.msg = _msgs[0];
                Report(par, cb);
            }
            else
            {
                var _userInfo = (UnityGameApp.Inst.Game as ChickenMasterGame).userInfo;
                if (_userInfo != null && par.uid == null)
                {
                    par.uid = (_userInfo.getData() as LocalUserInfo).uuid;
                }
                else if (par.uid == null)
                {
                    par.uid = "guest1";
                }
                CS_ReportParamList parList = new CS_ReportParamList();
                for(var i = 0; i < _msgs.Count; i++)
                {
                    par.msg = _msgs[i];
                    parList.list[i] = par;
                }

                ReportList(parList, cb);
            }
        }
    }
}
