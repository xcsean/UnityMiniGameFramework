using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;
using UnityEngine;

namespace UnityMiniGameFramework
{
    class UnityProjFileSystem : IFileSystem
    {
        protected string _getFullPath(string fileName)
        {
#if UNITY_ANDROID || UNITY_IPHONE
            return Application.persistentDataPath + filename;
#else
            return Application.dataPath + fileName;
#endif
        }

        public StreamReader getFileReadStream(string filename)
        {
            string path = _getFullPath(filename);
            return new StreamReader(path);
        }
        public StreamWriter getFileWriteStream(string filename)
        {
            string path = _getFullPath(filename);
            return new StreamWriter(path, true);
        }

        public FileStream getFileReadBinaryStream(string filename)
        {
            string path = _getFullPath(filename);
            return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read);
        }
        public FileStream getFileWriteBinaryStream(string filename)
        {
            string path = _getFullPath(filename);
            return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
        }

        public bool isFileExist(string filename)
        {
            string path = _getFullPath(filename);
            return File.Exists(path);
        }
        public void delFile(string filename)
        {
            string path = _getFullPath(filename);
            File.Delete(filename);
        }

        public byte[] readRawDataFrom(string filename)
        {
            throw new NotImplementedException();
        }

        public string readStringFrom(string filename)
        {
            string path = _getFullPath(filename);
            using (StreamReader reader = new StreamReader(path))
            {
                return reader.ReadToEnd();
            }
        }

        public void writeRawDataTo(string filename, byte[] rawData)
        {
            throw new NotImplementedException();
        }

        public void writeStringDataTo(string filename, string strData)
        {
            string path = _getFullPath(filename);
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.Write(strData);
            }
        }
    }
}
