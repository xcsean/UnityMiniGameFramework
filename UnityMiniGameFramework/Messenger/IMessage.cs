using System;
using System.Collections.Generic;
using System.Text;

namespace UnityMiniGameFramework
{
    public interface IMessage
    {
        uint iCommand
        {
            get;
        }

        string strCommand
        {
            get;
        }

        byte[] rawData
        {
            get;
        }

        object data
        {
            get;
        }

        T getStructureData<T>();
    }
}
