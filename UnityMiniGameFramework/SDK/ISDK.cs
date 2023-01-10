using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public interface ISDK
    {
        void Init(SDKBehaviour sdk);
        void showVideo();
        void loadVideo();
    }

    public class AdVideoResult
    {
    }
}
