using System;

namespace UnityMiniGameFramework
{
    public class ProtoMsg
    {
        public const int HeaderSize = 28;

        public UInt16 Version;
        public UInt16 PassCode;
        public UInt32 Seq;
        public UInt64 Uid;
        public UInt32 AppVersion;
        public UInt32 Cmd;
        public UInt32 BodyLen;

        public byte[] Bytes = new byte[128];
        public int Size;
        public int Offset;

        void ResizeBytes(int newSize)
        {
            Size = newSize;
            if (Bytes.Length <= newSize)
                Array.Resize(ref Bytes, newSize + newSize / 3);
        }

        public void ReadHeader()
        {
            Offset = 0;
            Version = ReadUInt16();
            PassCode = ReadUInt16();
            Seq = ReadUInt32();
            Uid = ReadUInt64();
            AppVersion = ReadUInt32();
            Cmd = ReadUInt32();
            BodyLen = ReadUInt32();

            ResizeBytes((int) BodyLen);
        }

        public void WriteProto(byte[] bytes)
        {
            ResizeBytes(bytes.Length + HeaderSize);
            BodyLen = (uint) bytes.Length;
            Offset = 0;
            Write(Version);
            Write(PassCode);
            Write(Seq);
            Write(Uid);
            Write(AppVersion);
            Write(Cmd);
            Write(BodyLen);
            Write(bytes);
        }


        UInt16 ReadUInt16()
        {
            int ret = 0;
            int size = 2;
            for (int i = Offset, n = Offset + size; i < n; ++i)
            {
                ret = (ret << 8) | Bytes[i];
            }

            Offset += size;
            return (UInt16) ret;
        }

        void Write(UInt16 value)
        {
            int tmp = (int) value;
            int size = 2;
            for (int i = Offset + size - 1; i >= Offset; --i)
            {
                Bytes[i] = (byte) (tmp & 0x00FF);
                tmp = tmp >> 8;
            }

            Offset += size;
        }

        UInt32 ReadUInt32()
        {
            UInt32 ret = 0;
            int size = 4;
            for (int i = Offset, n = Offset + size; i < n; ++i)
            {
                ret = (ret << 8) | Bytes[i];
            }

            Offset += size;
            return ret;
        }

        void Write(UInt32 value)
        {
            int tmp = (int) value;
            int size = 4;
            for (int i = Offset + size - 1; i >= Offset; --i)
            {
                Bytes[i] = (byte) (tmp & 0x00FF);
                tmp = tmp >> 8;
            }

            Offset += size;
        }

        UInt64 ReadUInt64()
        {
            int ret = 0;
            int size = 8;
            for (int i = Offset, n = Offset + size; i < n; ++i)
            {
                ret = (ret << 8) | Bytes[i];
            }

            Offset += size;
            return (UInt64) ret;
        }

        void Write(UInt64 value)
        {
            long tmp = (long) value;
            int size = 8;
            for (int i = Offset + size - 1; i >= Offset; --i)
            {
                Bytes[i] = (byte) (tmp & 0x00FF);
                tmp = tmp >> 8;
            }

            Offset += size;
        }

        void Write(byte[] bytes)
        {
            int n = bytes.Length;
            for (int i = 0; i < n; ++i)
            {
                Bytes[Offset + i] = bytes[i];
            }

            Offset += n;
        }
    }
}