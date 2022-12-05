using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    abstract public class Data : IData
    {
        protected Dictionary<string, DataObject> _datas;

        protected string _name;
        public string name => _name;

        protected IDataProvider _provider;
        public IDataProvider provider => _provider;


        virtual public List<string> initKeys => throw new NotImplementedException();

        public Data()
        {
            _datas = new Dictionary<string, DataObject>();
        }

        public void setNameAndProvider(string n, IDataProvider p)
        {
            _name = n;
            _provider = p;
        }


        public IDataObject getData(string key)
        {
            DataObject d = null;
            _datas.TryGetValue(key, out d);

            return d;
        }
        public void addNewData(string key, IDataObject newData)
        {
            if(newData == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Data [{_name}] add new data key[{key}] with null object");
                return;
            }

            _datas[key] = newData as DataObject;
        }

        public void initFromProvider()
        {
            // TO DO : combine batch read ops
            foreach (var key in initKeys)
            {
                object initData = _provider.ReadSingleData(_name, key);
                if (initData == null)
                {
                    continue;
                }

                _datas[key] = new DataObject(initData);
            }
        }
        public async Task initFromProviderAsync()
        {
            // TO DO : combine batch read ops
            foreach (var key in initKeys)
            {
                object initData = await _provider.ReadSingleDataAsync(_name, key);
                if(initData == null)
                {
                    continue;
                }

                _datas[key] = new DataObject(initData);
            }
        }

        protected List<KeyValuePair<string, DataObject>> _getDirtyDatas()
        {
            List<KeyValuePair<string, DataObject>> array = new List<KeyValuePair<string, DataObject>>();
            
            foreach (var pair in _datas)
            {
                // TO DO : compare current object with last update object
                //object lwObj;
                //if (_lastWritebackDatas.TryGetValue(pair.Key, out lwObj))
                //{
                //    if (lwObj == pair.Value)
                //    {
                //        // same, do not need writeback
                //        continue;
                //    }
                //}

                if(pair.Value.isDirty)
                {
                    array.Add(pair);
                }

            }

            return array;
        }

        virtual public void writeBack()
        {
            var dirtyArray = _getDirtyDatas();
            if(dirtyArray.Count <= 0)
            {
                return;
            }

            // TO DO : combine batch write ops
            foreach(var pair in dirtyArray)
            {
                _provider.WriteSingleData(_name, pair.Key, pair.Value.getData());
            }
        }

        virtual public async Task writeBackAsync()
        {
            var dirtyArray = _getDirtyDatas();
            if (dirtyArray.Count <= 0)
            {
                return;
            }

            // TO DO : combine batch write ops
            foreach (var pair in dirtyArray)
            {
                await _provider.WriteSingleDataAsync(_name, pair.Key, pair.Value.getData());
            }
        }
    }
}
