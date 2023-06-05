using System.Collections;

namespace UnityMiniGameFramework
{
    public abstract class ProcedureBase : StartupBase
    {
        internal protected abstract IEnumerator Execute(GameManager manager);
    }
}