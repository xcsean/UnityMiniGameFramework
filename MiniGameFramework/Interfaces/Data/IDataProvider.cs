using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public interface IDataProvider
    {
        IData CreateData(string dataName);
        IData GetData(string dataName);

        object ReadSingleData(string dataName, string key);
        void WriteSingleData(string dataName, string key, object obj);

        Task<object> ReadSingleDataAsync(string dataName, string key);
        Task WriteSingleDataAsync(string dataName, string key, object obj);
        
        void WritebackAll();
        Task WritebackAllAsync();
    }
}
