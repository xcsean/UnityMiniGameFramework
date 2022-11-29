using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class LocalStorageProvider : DataProvider
    {
        protected string _localFileName;
        public string localFileName => _localFileName;

        public LocalStorageProvider(string storageFile)
        {
            _localFileName = storageFile;
        }

        public void Init()
        {

        }

        override public object ReadSingleData(string dataName, string key)
        {
            throw new NotImplementedException();
        }

        override public Task<object> ReadSingleDataAsync(string dataName, string key)
        {
            throw new NotImplementedException();
        }

        override public void WriteSingleData(string dataName, string key, object obj)
        {
            throw new NotImplementedException();
        }

        override public Task WriteSingleDataAsync(string dataName, string key, object obj)
        {
            throw new NotImplementedException();
        }
    }
}
