using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    class UnityProjFileSystem : IFileSystem
    {
        protected string _getFullPath(string fileName)
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return Application.persistentDataPath + fileName;
            }
            else
            {
                return Application.dataPath + fileName;
            }
        }

        private string getLocalSaveFilePath(string fileName)
        {
            if (UnityGameApp.Inst.Platform == PlatformEnum.PlatformAndroid || UnityGameApp.Inst.Platform == PlatformEnum.PlatformIPhone)
            {
                return Application.persistentDataPath + fileName;
            }
            else
            {
                return Application.dataPath + fileName;
            }
        }

        public StreamReader getFileReadStream(string fileName)
        {
            string path = _getFullPath(fileName);
            return new StreamReader(path);
        }

        public StreamWriter getFileWriteStream(string fileName)
        {
            string path = _getFullPath(fileName);
            return new StreamWriter(path, true);
        }

        public FileStream getFileReadBinaryStream(string fileName)
        {
            string path = getLocalSaveFilePath(fileName);
            createDir(path);
            return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read);
        }

        public FileStream getFileWriteBinaryStream(string fileName)
        {
            string path = getLocalSaveFilePath(fileName);
            createDir(path);
            return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
        }
        
        private void createDir(string filePath)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir ?? string.Empty);
            }
        }

        public bool isFileExist(string fileName)
        {
            string path = _getFullPath(fileName);
            return File.Exists(path);
        }

        public bool isLocalSaveFileExist(string fileName)
        {
            string path = getLocalSaveFilePath(fileName);
            return File.Exists(path);
        }
        
        public void delFile(string fileName)
        {
            string path = _getFullPath(fileName);
            File.Delete(path);
        }

        public void delLocalSaveFile(string fileName)
        {
            string path = getLocalSaveFilePath(fileName);
            File.Delete(path);
        }

        public byte[] readRawDataFrom(string fileName)
        {
            throw new NotImplementedException();
        }

        public string readStringFrom(string fileName)
        {
            string path = _getFullPath(fileName);
            using (StreamReader reader = new StreamReader(path))
            {
                return reader.ReadToEnd();
            }
        }

        public string readStringFromStreamPath(string fileName)
        {
            string path = Application.streamingAssetsPath + fileName;
            if (UnityGameApp.Inst.Platform == PlatformEnum.PlatformEditorMac)
            {
                path = "file://" + path;
            }
            WWW www = new WWW(path);
            while (!www.isDone)
            {
            }

            return www.text;
        }

        public void writeRawDataTo(string fileName, byte[] rawData)
        {
            throw new NotImplementedException();
        }

        public void writeStringDataTo(string fileName, string strData)
        {
            string path = _getFullPath(fileName);
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.Write(strData);
            }
        }
    }
}