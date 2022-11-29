using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    abstract public class DataProvider : IDataProvider
    {
        protected Dictionary<string, Func<IData>> _dataObjectCreators;

        protected Dictionary<string, Data> _datas;

        public DataProvider()
        {
            _dataObjectCreators = new Dictionary<string, Func<IData>>();
            _datas = new Dictionary<string, Data>();
        }

        virtual public void regDataObjectCreator(string dataName, Func<IData> creator)
        {
            if (_dataObjectCreators.ContainsKey(dataName))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"DataProvider regDataObjectCreator ({dataName}) already exist");
                return;
            }
            _dataObjectCreators[dataName] = creator;
        }

        protected Data _createData(string dataName)
        {
            if (_dataObjectCreators.ContainsKey(dataName))
            {
                return _dataObjectCreators[dataName]() as Data;
            }

            Debug.DebugOutput(DebugTraceType.DTT_Error, $"DataProvider _createData ({dataName}) not exist");

            return null;
        }

        virtual public IData CreateData(string dataName)
        {
            Data data = _createData(dataName);
            if(data == null)
            {
                return null;
            }

            _datas[dataName] = data;
            return data;
        }

        virtual public IData GetData(string dataName)
        {
            Data d;
            _datas.TryGetValue(dataName, out d);

            return d;
        }

        virtual public object ReadSingleData(string dataName, string key)
        {
            throw new NotImplementedException();
        }

        virtual public Task<object> ReadSingleDataAsync(string dataName, string key)
        {
            throw new NotImplementedException();
        }

        virtual public void WriteSingleData(string dataName, string key, object obj)
        {
            throw new NotImplementedException();
        }

        virtual public Task WriteSingleDataAsync(string dataName, string key, object obj)
        {
            throw new NotImplementedException();
        }

        virtual public void WritebackAll()
        {
            // TO DO : combine batch write ops
            foreach (var pair in _datas)
            {
                pair.Value.writeBack();
            }
        }

        virtual public async Task WritebackAllAsync()
        {
            // TO DO : combine batch write ops
            foreach (var pair in _datas)
            {
                await pair.Value.writeBackAsync();
            }
        }
    }
}
