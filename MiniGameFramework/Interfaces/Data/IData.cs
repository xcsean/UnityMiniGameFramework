using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public interface IData
    {
        string name { get; }
        IDataProvider provider { get; }

        bool isDirty { get; }

        List<string> initKeys { get; }
        void initFromProvider();
        Task initFromProviderAsync();

        object getData(string key);
        void modifyData(string key, object newData);

        void writeBack();
        Task writeBackAsync();
    }
}
