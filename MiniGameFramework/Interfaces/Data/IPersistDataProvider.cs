using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public interface IPersistDataProvider
    {
        AsyncOpStatus readDataStatus { get; }
        AsyncOpStatus writeDataStatus { get; }

        bool isDirty { get; }
        bool isReaded { get; }

        bool isReadingOrWriting { get; }

        bool startReadFrom(Action onFinish = null);
        bool startWriteBack(Action onFinish = null);

        void updateUserData(object newData);

        object getUserData();

        T getUserData<T>();

        object getData(string key);

        T getData<T>(string key);

        void updateData(string key, object newData);

    }
}
