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

        List<string> initKeys { get; }
        void initFromProvider();
        Task initFromProviderAsync();

        IDataObject getData(string key);
        void addNewData(string key, IDataObject newData);

        void writeBack();
        Task writeBackAsync();
    }
}
