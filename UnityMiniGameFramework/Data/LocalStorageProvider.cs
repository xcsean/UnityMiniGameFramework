using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    class LocalStorageProvider : IPersistDataProvider
    {
        protected string _localFileName;
        public string localFileName => _localFileName;

        public LocalStorageProvider(string storageFile)
        {
            _localFileName = storageFile;
        }

        public AsyncOpStatus readDataStatus => throw new NotImplementedException();

        public AsyncOpStatus writeDataStatus => throw new NotImplementedException();

        public bool isDirty => throw new NotImplementedException();

        public bool isReaded => throw new NotImplementedException();

        public bool isReadingOrWriting => throw new NotImplementedException();

        public bool startReadFrom(Action onFinish = null)
        {
            throw new NotImplementedException();
        }

        public bool startWriteBack(Action onFinish = null)
        {
            throw new NotImplementedException();
        }

        public object getUserData()
        {
            throw new NotImplementedException();
        }

        public T getUserData<T>()
        {
            throw new NotImplementedException();
        }
        public void updateUserData(object newData)
        {
            throw new NotImplementedException();
        }

        public object getData(string key)
        {
            throw new NotImplementedException();
        }

        public T getData<T>(string key)
        {
            throw new NotImplementedException();
        }
        public void updateData(string key, object newData)
        {
            throw new NotImplementedException();
        }

    }
}
