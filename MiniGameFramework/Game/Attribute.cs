using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;

namespace MiniGameFramework
{
    public class Attribute<T> : IAttribute
    {
        protected T _data;

        public object data
        {
            get
            {
                return _data;
            }
        }

        public string toJson()
        {
            //return JsonSerializer.Serialize(_data, typeof(T));
            return JsonUtil.ToJson(_data, typeof(T));
        }
        public void fromJson(string jsonStr)
        {
            //_data = (T)JsonSerializer.Deserialize(jsonStr, typeof(T));
            _data = (T) JsonUtil.FromJson(jsonStr, typeof(T));
        }

        public virtual void cloneTo<U>(U toObj)
        {
            Type type = typeof(T);
            // TO DO : write to toObj
        }
        public virtual void cloneFrom<U>(U fromObj)
        {
            Type type = typeof(T);
            // TO DO : read from toObj
        }
    }
}
