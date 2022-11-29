using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    abstract public class Data : IData
    {
        protected Dictionary<string, object> _datas;
        protected Dictionary<string, object> _lastWritebackDatas;

        protected string _name;
        public string name => _name;

        protected IDataProvider _provider;
        public IDataProvider provider => _provider;

        protected bool _isDirty;
        public bool isDirty => _isDirty;
        
        public List<string> initKeys => throw new NotImplementedException();

        public Data(string n, IDataProvider p)
        {
            _name = n;
            _provider = p;
            
            _datas = new Dictionary<string, object>();
            _isDirty = false;
        }


        public object getData(string key)
        {
            object d = null;
            _datas.TryGetValue(key, out d);

            return d;
        }
        
        public void initFromProvider()
        {
            // TO DO : combine batch read ops
            foreach (var key in initKeys)
            {
                object initData = _provider.ReadSingleData(_name, key);

                _datas[key] = initData;
                _lastWritebackDatas[key] = initData;
            }
        }
        public async Task initFromProviderAsync()
        {
            // TO DO : combine batch read ops
            foreach (var key in initKeys)
            {
                object initData = await _provider.ReadSingleDataAsync(_name, key);

                _datas[key] = initData;
                _lastWritebackDatas[key] = initData;
            }
        }

        public void modifyData(string key, object newData)
        {
            _isDirty = true;
            _datas[key] = newData;
        }

        protected List<KeyValuePair<string, object>> _getDirtyDatas()
        {
            List<KeyValuePair<string, object>> array = new List<KeyValuePair<string, object>>();
            
            foreach (var pair in _datas)
            {
                object lwObj;
                if (_lastWritebackDatas.TryGetValue(pair.Key, out lwObj))
                {
                    if (lwObj == pair.Value)
                    {
                        // same, do not need writeback
                        continue;
                    }
                }

                array.Add(pair);
            }

            return array;
        }

        public void writeBack()
        {
            if(!_isDirty)
            {
                return;
            }

            var dirtyArray = _getDirtyDatas();

            // TO DO : combine batch write ops
            foreach(var pair in dirtyArray)
            {
                _provider.WriteSingleData(_name, pair.Key, pair.Value);
            }
        }

        public async Task writeBackAsync()
        {
            if (!_isDirty)
            {
                return;
            }

            var dirtyArray = _getDirtyDatas();

            // TO DO : combine batch write ops
            foreach (var pair in dirtyArray)
            {
                await _provider.WriteSingleDataAsync(_name, pair.Key, pair.Value);
            }
        }
    }
}
