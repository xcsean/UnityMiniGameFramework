using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public interface IDataObject
    {
        bool isDirty { get; }

        object getData();
        void modifyData(object newData);

        void markDirty();
    }
}
