using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class LocalStorageProvider : DataProvider
    {
        protected struct storeFile
        {
            public string fileName;
            public Dictionary<string, object> objects;
        }

        protected string _localFileName;
        public string localFileName => _localFileName;

        protected Dictionary<string, storeFile> _fileWriters;

        public LocalStorageProvider(string storageFile)
        {
            _localFileName = storageFile;
            _fileWriters = new Dictionary<string, storeFile>();
        }

        public void Init()
        {
        }

        override public IData CreateData(string dataName)
        {
            IData data = base.CreateData(dataName);
            if(data == null)
            {
                return null;
            }

            string fileName = _localFileName + dataName;
            var objects = new Dictionary<string, object>();

            if (!UnityGameApp.Inst.File.isLocalSaveFileExist(fileName))
            {
                // create file
            }
            else
            {
                using (var readStream = UnityGameApp.Inst.File.getFileReadBinaryStream(fileName))
                {
                    // TO DO : read objects from file

                    BinaryReader reader = new BinaryReader(readStream, Encoding.UTF8);
                    int objectCount = reader.ReadInt32();

                    for(int i=0; i< objectCount; ++i)
                    {
                        string key = reader.ReadString();
                        string clsName = reader.ReadString();
                        var jsonStr = reader.ReadString();

                        Type t = Type.GetType(clsName);
                        if (t != null)
                        {
                            var obj = JsonSerializer.Deserialize(jsonStr, t);

                            objects[key] = obj;
                        }
                        else
                        {
                            Debug.DebugOutput(DebugTraceType.DTT_Error, $"LocalStorageProvider CreateData [{fileName}] class [{clsName}] not exist");
                        }
                    }
                }
            }

            _fileWriters[dataName] = new storeFile()
            {
                fileName = fileName,
                objects = objects
            };

            return data;
        }

        protected object _readSingleData(string dataName, string key)
        {
            if (!_fileWriters.ContainsKey(dataName))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"LocalStorageProvider _readSingleData ({dataName}) not exist");
                return null;
            }

            var fw = _fileWriters[dataName];
            if (!fw.objects.ContainsKey(key))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"LocalStorageProvider _readSingleData ({dataName}) key ({key}) not exist");
                return null;
            }

            return fw.objects[key];
        }

        override public object ReadSingleData(string dataName, string key)
        {
            return _readSingleData(dataName, key);
        }

        override public Task<object> ReadSingleDataAsync(string dataName, string key)
        {
            return Task.Run(()=> {
                return _readSingleData(dataName, key);
            });
        }

        protected object _writeSingleData(string dataName, string key, object obj)
        {
            if (!_fileWriters.ContainsKey(dataName))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"LocalStorageProvider _writeSingleData ({dataName}) not exist");
                return null;
            }

            var fw = _fileWriters[dataName];
            return fw.objects[key] = obj;
        }

        override public void WriteSingleData(string dataName, string key, object obj)
        {
            _writeSingleData(dataName, key, obj);
        }

        override public Task WriteSingleDataAsync(string dataName, string key, object obj)
        {
            return Task.Run(() => {
                _writeSingleData(dataName, key, obj);
            });
        }

        public void writeFile(string dataName)
        {
            if (!_fileWriters.ContainsKey(dataName))
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"LocalStorageProvider writeFile ({dataName}) not exist");
                return;
            }
            
            _writeFile(_fileWriters[dataName]);

        }

        protected void _writeFile(storeFile fw)
        {
            // write int

            using (var fileStream = UnityGameApp.Inst.File.getFileWriteBinaryStream(fw.fileName))
            {
                BinaryWriter writer = new BinaryWriter(fileStream, Encoding.UTF8);
                writer.Seek(0, SeekOrigin.Begin);

                writer.Write(fw.objects.Count);

                foreach (var objPair in fw.objects)
                {
                    writer.Write(objPair.Key);
                    writer.Write(objPair.Value.GetType().FullName);
                    var jsonStr = JsonSerializer.Serialize(objPair.Value, objPair.Value.GetType());

                    writer.Write(jsonStr);
                }
            }
        }

        protected void _writeFiles()
        {
            foreach (var pair in _fileWriters)
            {
                _writeFile(pair.Value);
            }
        }

        override public void WritebackAll()
        {
            base.WritebackAll();

            _writeFiles();
        }

        override public async Task WritebackAllAsync()
        {
            await Task.Run(async () => {

                await base.WritebackAllAsync();

                _writeFiles();
            });
        }
    }
}
