using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public enum DebugTraceType
    {
        DTT_None,
        DTT_Error,
        DTT_System,
        DTT_Debug,
        DTT_Detail
    }

    public class Debug
    {
        protected static Action<string> _dbgOutput;
        protected static DebugTraceType _traceType;

        public static void DebugOutput(DebugTraceType traceType, string msg)
        {
            if (_dbgOutput == null)
            {
                return;
            }

            if(traceType > _traceType)
            {
                return;
            }

            _dbgOutput($"[{traceType.ToString()}]:{msg}");
        }
        
        public static void Init(Action<string> dbgOutput, DebugTraceType traceType = DebugTraceType.DTT_Detail)
        {
            _dbgOutput = dbgOutput;
            _traceType = traceType;
        }
    }
}
