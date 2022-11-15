using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace UnityMiniGameFramework
{
    public interface IHeightMap
    {
        float getHeightByVector(Vector3 pos);
    }
}
