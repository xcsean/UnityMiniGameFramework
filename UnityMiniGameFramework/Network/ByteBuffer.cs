namespace UnityMiniGameFramework
{
    public struct ByteBuffer
    {
        public ByteBuffer(byte[] buf, int len)
        {
            buffer = buf;
            Length = len;
        }
        
        public byte[] buffer;    

        public int Length
        {
            get;
            private set;
        }
    }
}