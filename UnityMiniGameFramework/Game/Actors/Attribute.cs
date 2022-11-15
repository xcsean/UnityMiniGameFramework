using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityMiniGameFramework
{
    public class Attribute
    {
        protected object _data;

        public object rawData
        {
            get
            {
                return _data;
            }
        }
        public T getStructData<T>()
        {
            return (T)_data;
        }

        public string toJson()
        {
            return "{}";
        }
        public void fromJson()
        {
        }
        
    }
}
