using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class BuffDataManager
    {
        protected BuffConfig _buffConfig;
        public BuffConfig BuffConfig => _buffConfig;

        public void Init()
        {
            _buffConfig = UnityGameApp.Inst.Conf.getConfig("buffs") as BuffConfig;
            if (_buffConfig == null)
                Debug.DebugOutput(DebugTraceType.DTT_Error, "buffsConfig Init Fail");
        }
    }
}