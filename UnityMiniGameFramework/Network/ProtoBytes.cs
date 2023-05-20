using System;

namespace UnityMiniGameFramework
{
    public class ProtoBytes
    {
        public uint ProtoId;
        public int Size;
        public byte[] Bytes = new byte[128];

        void ResizeBytes(int newSize)
        {
            Size = newSize;
            if (Bytes.Length <= newSize)
                Array.Resize(ref Bytes, newSize + newSize / 3);
        }

        public ProtoBytes SetProtoMsg(ProtoMsg protoMsg)
        {
            ProtoId = protoMsg.Cmd;
            ResizeBytes(protoMsg.Size);
            Buffer.BlockCopy(protoMsg.Bytes, 0, Bytes, 0, protoMsg.Size);
            return this;
        }
    }
}