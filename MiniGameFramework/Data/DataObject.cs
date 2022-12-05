using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public class DataObject : IDataObject
    {
        protected object _data;

        protected bool _isDirty;
        public bool isDirty => _isDirty;

        public DataObject(object d)
        {
            _data = d;
            _isDirty = false;
        }

        public object getData()
        {
            return _data;
        }

        public void markDirty()
        {
            _isDirty = true;
        }

        public void modifyData(object newData)
        {
            _data = newData;
            _isDirty = true;
        }
    }
}
