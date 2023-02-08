using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;
using UnityMiniGameFramework.RESTFulAPI;

namespace UnityMiniGameFramework
{
    /// <summary>
    /// 数据打点
    /// </summary>
    public class AnalysisDataManager
    {

        public AnalysisConfig _analysisConf;
        public AnalysisConfig analysisConf => _analysisConf;

        public void Init()
        {
            _analysisConf = (AnalysisConfig)UnityGameApp.Inst.Conf.getConfig("analysis");
            if (_analysisConf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"AnalysisDataManager [analysis] config not exist");
                return;
            }
        }

        protected AnalysicPointConf GetConfig(int id)
        {
            var conf = _analysisConf.getPointsConfig(id.ToString());
            if (conf == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"AnalysisDataManager points[{id}] config not exist");
                return null;
            }
            return conf;
        }

        /// <summary>
        /// 埋点描述
        /// </summary>
        protected string GetPointMsg(int id)
        {
            var conf = GetConfig(id);
            if (conf == null)
            {
                return "";
            }
            return conf.point;
        }

        /// <summary>
        /// 通用打点数据接口
        /// </summary>
        /// <param name="_id">埋点配置id</param>
        /// <param name="_msg">自定义描述</param>
        /// <returns></returns>
        public CS_ReportParam GetPointDataById(int _id, string _msg = "")
        {
            int id = _id;
            return new CS_ReportParam()
            {
                type = id,
                point = $"{GetPointMsg(id)}",
                msg = _msg,
            };
        }

        /// <summary>
        /// 在线时长
        /// </summary>
        /// <param name="_msg"></param>
        /// <param name="_subType">1登录 2切后台 3切前台 4退出</param>
        /// <returns></returns>
        public CS_ReportParam GetPointData1(string _msg = "", int _subType = 0)
        {
            int id = 1;
            return new CS_ReportParam() {
                type = id,
                subType = _subType,
                point = $"{GetPointMsg(id)}",
                msg = _msg,
            };
        }

        /// <summary>
        /// 等级
        /// </summary>
        public CS_ReportParam GetPointData2(string _msg = "")
        {
            int id = 2;
            return new CS_ReportParam()
            {
                type = id,
                point = $"{GetPointMsg(id)}",
                msg = _msg,
            };
        }

        /// <summary>
        /// 获得金币
        /// </summary>
        public CS_ReportParam GetPointData3(string _msg = "")
        {
            int id = 3;
            return new CS_ReportParam()
            {
                type = id,
                point = $"{GetPointMsg(id)}",
                msg = _msg,
            };
        }

        /// <summary>
        /// 鸡肉数量
        /// </summary>
        public CS_ReportParam GetPointData4(string _msg = "")
        {
            int id = 4;
            return new CS_ReportParam()
            {
                type = id,
                point = $"{GetPointMsg(id)}",
                msg = _msg,
            };
        }

        /// <summary>
        /// 关卡数量
        /// </summary>
        public CS_ReportParam GetPointData5(string _msg = "")
        {
            int id = 5;
            return new CS_ReportParam()
            {
                type = id,
                point = $"{GetPointMsg(id)}",
                msg = _msg,
            };
        }

        /// <summary>
        /// 6个英雄的升级等级
        /// </summary>
        public CS_ReportParam GetPointData6(string _msg = "")
        {
            int id = 6;
            return new CS_ReportParam()
            {
                type = id,
                point = $"{GetPointMsg(id)}",
                msg = _msg,
            };
        }

        /// <summary>
        /// 18武器的星级
        /// </summary>
        public CS_ReportParam GetPointData7(string _msg = "")
        {
            int id = 7;
            return new CS_ReportParam()
            {
                type = id,
                point = $"{GetPointMsg(id)}",
                msg = _msg,
            };
        }

        /// <summary>
        /// 18武器碎片数量
        /// </summary>
        public CS_ReportParam GetPointData8(string _msg = "")
        {
            int id = 8;
            return new CS_ReportParam()
            {
                type = id,
                point = $"{GetPointMsg(id)}",
                msg = _msg,
            };
        }

        /// <summary>
        /// 广告点击
        /// </summary>
        public CS_ReportParam GetPointData9(string _msg = "")
        {
            int id = 9;
            return new CS_ReportParam()
            {
                type = id,
                point = $"{GetPointMsg(id)}",
                msg = _msg,
            };
        }

        /// <summary>
        /// 广告看完
        /// </summary>
        public CS_ReportParam GetPointData10(string _msg = "")
        {
            int id = 10;
            return new CS_ReportParam()
            {
                type = id,
                point = $"{GetPointMsg(id)}",
                msg = _msg,
            };
        }

        /// <summary>
        /// 蛋掉血的关卡
        /// </summary>
        public CS_ReportParam GetPointData11(string _msg = "")
        {
            int id = 11;
            return new CS_ReportParam()
            {
                type = id,
                point = $"{GetPointMsg(id)}",
                msg = _msg,
            };
        }

        /// <summary>
        /// 建筑等级
        /// </summary>
        public CS_ReportParam GetPointData12(string _msg = "")
        {
            int id = 12;
            return new CS_ReportParam()
            {
                type = id,
                point = $"{GetPointMsg(id)}",
                msg = _msg,
            };
        }

        /// <summary>
        /// 消耗金币
        /// </summary>
        public CS_ReportParam GetPointData13(string _msg = "")
        {
            int id = 13;
            return new CS_ReportParam()
            {
                type = id,
                point = $"{GetPointMsg(id)}",
                msg = _msg,
            };
        }

    }
}
