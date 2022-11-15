using System;
using System.Collections.Generic;
using System.Text;

namespace MiniGameFramework
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
