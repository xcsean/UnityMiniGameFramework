using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameFramework
{
    public interface IFileSystem
    {
        bool isFileExist(string filename);
        void delFile(string filename);

        StreamReader getFileReadStream(string filename);
        StreamWriter getFileWriteStream(string filename);

        FileStream getFileReadBinaryStream(string filename);
        FileStream getFileWriteBinaryStream(string filename);

        byte[] readRawDataFrom(string filename);
        string readStringFrom(string filename);

        void writeRawDataTo(string filename, byte[] rawData);
        void writeStringDataTo(string filename, string strData);
    }
}
