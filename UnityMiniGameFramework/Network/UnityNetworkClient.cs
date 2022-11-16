using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniGameFramework;

namespace UnityMiniGameFramework
{
    public class UnityNetworkClient : ISessionClientHandler
    {
        protected static uint MIN_BUFF_SIZE = 128 * 1024;

        protected string _uri;
        protected uint _port;

        protected SessionClient _sessionClient;
        public SessionClient client => _sessionClient;

        public void Init(NetConfig conf)
        {
            Debug.DebugOutput(DebugTraceType.DTT_System, "Init Network Client");

            if(conf.webSockConn == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, "Init Network webSockConn conf not exist");
                return;
            }

            uint buffSize = conf.webSockConn.buffSize;
            if(buffSize < MIN_BUFF_SIZE)
            {
                buffSize = MIN_BUFF_SIZE;
            }

            if (conf.webSockConn.addr == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, "Init Network webSockConn.addr conf not exist");
                return;
            }

            _uri = conf.webSockConn.addr.uri;
            _port = conf.webSockConn.addr.port;

            IConnector conn = GameApp.Inst.Network.CreateConnector(conf.webSockConn.connectorType, buffSize);
            if(conn == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Init Network connector ({conf.webSockConn.connectorType}) create failed");
                return;
            }
            IProtocol proto = GameApp.Inst.Network.CreateProtocol(conf.webSockConn.protocolType);
            if (proto == null)
            {
                Debug.DebugOutput(DebugTraceType.DTT_Error, $"Init Network protocol ({conf.webSockConn.protocolType}) create failed");
                return;
            }

            _sessionClient = GameApp.Inst.Network.CreateSessionClient(conn, proto, this);
            
            Debug.DebugOutput(DebugTraceType.DTT_System, "register protocols");

            //client.protocol.regCommand(11201, typeof(Common.Empty));
            //client.protocol.regCommand(11202, typeof(Server.GetTrainTownAllResult));
            
        }

        public void Connect()
        {
            _sessionClient.Connect(_uri, _port);
        }

        public void onClose()
        {
            Debug.DebugOutput(DebugTraceType.DTT_System, "on session close");

            // TO DO : on close, try re connect
        }

        public void onConnected()
        {
            Debug.DebugOutput(DebugTraceType.DTT_System, "on session connected");

            // TO DO : on connected
        }

        public void onConnecting()
        {
            Debug.DebugOutput(DebugTraceType.DTT_System, "on session connecting...");
        }

        public void onError(string errMsg, int errCode)
        {
            Debug.DebugOutput(DebugTraceType.DTT_Error, $"on session error {errCode}");
            Debug.DebugOutput(DebugTraceType.DTT_Error, errMsg);

            // TO DO : on error
        }

        public void onReceive(MiniGameFramework.IMessage msg)
        {
            if (msg == null)
            {
                return;
            }

            Debug.DebugOutput(DebugTraceType.DTT_Detail, $"on session receiving msg iCmd{msg.iCommand}");
            Debug.DebugOutput(DebugTraceType.DTT_Detail, $"data:{msg.data}");

            // TO DO : on message reveive
        }

        public void onSended(MiniGameFramework.IMessage msg)
        {
            Debug.DebugOutput(DebugTraceType.DTT_Detail, $"on session command {msg.iCommand} sended");

            // TO DO : on message sended
        }

        public void onSending(MiniGameFramework.IMessage msg)
        {
            Debug.DebugOutput(DebugTraceType.DTT_Detail, $"on session command {msg.iCommand} sending...");

            // TO DO : on message sending
        }
    }
}
