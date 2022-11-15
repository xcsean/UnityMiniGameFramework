using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Google.Protobuf;

namespace UnityMiniGameFramework
{
    class ProtocolProtoBuffer : IProtocol
    {
        protected static ConcurrentDictionary<uint, ProtoBufferMessageBinder> _dictProtoBufferMessages;

        public void regCommand(uint iCmd, Type t)
        {
            try
            {
                if(_dictProtoBufferMessages == null)
                {
                    _dictProtoBufferMessages = new ConcurrentDictionary<uint, ProtoBufferMessageBinder>();
                }

                _dictProtoBufferMessages[iCmd] = new ProtoBufferMessageBinder(t);
            }
            catch (Exception ex)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"ProtocolProtoBuffer.regCommand:{iCmd} error: {ex.Message}");
                Debug.DebugOutput(DebugTraceType.DTT_Error, ex.StackTrace);
            }
        }

        public IMessage decode(byte[] buff, uint offset, out uint length)
        {
            length = (uint)buff.Length;
            try
            {
                // read len
                uint len = BitConverter.ToUInt32(buff, (int)offset);
                
                if (BitConverter.IsLittleEndian)
                {
                    byte[] intBytes = new byte[] { buff[offset+3], buff[offset + 2], buff[offset + 1], buff[offset + 0] };
                    len = BitConverter.ToUInt32(intBytes, 0);
                }

                uint iCmd = BitConverter.ToUInt32(buff, (int)offset + 4);

                if (BitConverter.IsLittleEndian)
                {
                    byte[] intBytes = new byte[] { buff[offset + 7], buff[offset + 6], buff[offset + 5], buff[offset + 4] };
                    iCmd = BitConverter.ToUInt32(intBytes, 0);
                }

                CodedInputStream input = new CodedInputStream(buff, (int)offset+8, (int)len);
                
                length = len + 8; // leng + cmdid

                if (!_dictProtoBufferMessages.ContainsKey(iCmd))
                {
                    Debug.DebugOutput(DebugTraceType.DTT_Error, $"ProtocolProtoBuffer.decode iCmd:({iCmd}) proto buffer message not exist");
                    return null;
                }
                ProtoBufferMessageBinder messageBinder = _dictProtoBufferMessages[iCmd];
                object data = messageBinder.decodeFromBytes(input);

                return new MessageProtoBuffer(iCmd, null, data);
            }
            catch (Exception ex)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"ProtocolProtoBuffer.decode error: {ex.Message}");
                Debug.DebugOutput(DebugTraceType.DTT_Error, ex.StackTrace);
            }

            return null;
        }

        public IMessage encode<T>(uint iCmd, T t)
        {
            byte[] data = encodeToBytes(iCmd, t);

            return new MessageProtoBuffer(iCmd, data, t); 
        }

        public byte[] encodeToBytes<T>(uint iCmd, T t)
        {
            try
            {
                using (MemoryStream sr = new MemoryStream())
                {
                    CodedOutputStream output = new CodedOutputStream(sr);

                    // write command
                    if (BitConverter.IsLittleEndian)
                    {
                        byte[] intBytes = BitConverter.GetBytes(iCmd);
                        Array.Reverse(intBytes);
                        uint rev = BitConverter.ToUInt32(intBytes, 0);
                        output.WriteFixed32(rev);
                    }
                    else
                    {
                        output.WriteFixed32(iCmd);
                    }

                    ProtoBufferMessageBinder messageBinder = _dictProtoBufferMessages[iCmd];
                    messageBinder.encodeToBytes<T>(t, output);

                    output.Flush();
                    return sr.ToArray(); // TO DO : use GetBuffer instead
                }
            }
            catch (Exception ex)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"ProtocolProtoBuffer.encodeToBytes cmd:{iCmd} error: {ex.Message}");
                Debug.DebugOutput(DebugTraceType.DTT_Error, ex.StackTrace);
            }

            return null;
        }
    }

    class ProtoBufferMessageBinder
    {
        protected static Type[] _parserParameterTypes = new Type[] { typeof(CodedInputStream) };

        protected MethodInfo _writeToMethod;
        protected MethodInfo _parserFromMethod;
        protected object _parserObject;
        protected Type _type;

        public ProtoBufferMessageBinder(Type t)
        {
            _type = t;

            Type protoClsType = t;
            _writeToMethod = protoClsType.GetMethod("WriteTo");
            
            PropertyInfo prop = protoClsType.GetProperty("Parser");
            _parserObject = prop.GetValue(null);

            Type parserClsType = _parserObject.GetType();
            
            _parserFromMethod = parserClsType.GetMethod("ParseFrom", _parserParameterTypes);
        }

        public object decodeFromBytes(CodedInputStream input)
        {
            return _parserFromMethod.Invoke(_parserObject, new object[] { input });
        }

        public void encodeToBytes<T>(T t, CodedOutputStream output)
        {
            _writeToMethod.Invoke(t, new object[] { output });
        }
    }
}

