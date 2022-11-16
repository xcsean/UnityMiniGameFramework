using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
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
        protected static Action<string> _dbgError;
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

            if(_dbgError != null && traceType == DebugTraceType.DTT_Error)
            {
                _dbgError($"[{traceType.ToString()}]:{msg}");
            }
            else
            {
                _dbgOutput($"[{traceType.ToString()}]:{msg}");
            }
        }
        
        public static void Init(Action<string> dbgOutput, Action<string> dbgError = null, DebugTraceType traceType = DebugTraceType.DTT_Detail)
        {
            _dbgOutput = dbgOutput;
            _dbgError = dbgError;
            _traceType = traceType;
        }
    }
}
