using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using UnityMiniGameFramework;

namespace ConsoleApp1
{
    class SessionClientHandler : ISessionClientHandler
    {
        public void onClose()
        {
            Console.WriteLine("on session close");
        }

        public void onConnected()
        {
            Console.WriteLine("on session connected");

            Console.WriteLine("Please input message");
        }

        public void onConnecting()
        {
            Console.WriteLine("on session connecting...");
        }

        public void onError(string errMsg, int errCode)
        {
            Console.WriteLine($"on session error {errCode}");
            Console.WriteLine(errMsg);
        }

        public void onReceive(UnityMiniGameFramework.IMessage msg)
        {
            if(msg == null)
            {
                return;
            }

            Console.WriteLine($"on session receiving msg iCmd{msg.iCommand}");
            Console.WriteLine($"data:{msg.data}");
        }

        public void onSended(UnityMiniGameFramework.IMessage msg)
        {
            Console.WriteLine($"on session command {msg.iCommand} sended");
        }

        public void onSending(UnityMiniGameFramework.IMessage msg)
        {
            Console.WriteLine($"on session command {msg.iCommand} sending...");
        }
    }

    class Program
    {
        static void dbgOutput(string msg)
        {
            Console.WriteLine(msg);
        }

        static void Main(string[] args)
        {

            Console.WriteLine("Init network...");
            Debug.Init(dbgOutput);

            IConnector conn = Network.CreateConnector("websock", 512 * 1024);
            IProtocol proto = Network.CreateProtocol("protoBuff");

            SessionClientHandler handler = new SessionClientHandler();
            SessionClient client = Network.CreateSessionClient(conn, proto, handler);

            Console.WriteLine("register command...");
            client.protocol.regCommand(1101, typeof(Server.LoginData));
            client.protocol.regCommand(1102, typeof(Server.LoginResult));

            client.protocol.regCommand(11201, typeof(Common.Empty));
            client.protocol.regCommand(11202, typeof(Server.GetTrainTownAllResult));

            client.protocol.regCommand(11211, typeof(Common.Empty));
            client.protocol.regCommand(11212, typeof(Common.CommonResult));

            client.protocol.regCommand(11301, typeof(Client.CreateBuildingParams));
            client.protocol.regCommand(11302, typeof(Server.TrainChangeBuildingResult));

            //client.protocol.regCommand(19001, typeof());
            //client.protocol.regCommand(19002, typeof(Server));
            client.protocol.regCommand(19003, typeof(Server.TownBaseInfo));

            //Server.TrainChangeBuildingResult r = new Server.TrainChangeBuildingResult
            //{
            //    Code = 0,
            //    Msg = "success",
            //    Data = new Server.TrainBuildingInfo
            //    {
            //        BuildingId = "6370e8bf25500b765f936ec2",
            //        ConfigId = "101101",
            //        Type = "house",
            //        //SubType = undefined,
            //        Name = "民房",
            //        State = 0,
            //        LogicPos = new Server.TrainBuildingLogicPos
            //        {
            //            X = 31,
            //            Y = 22
            //        },
            //        Orientation = 1,
            //        WorkSlotCount = 0,
            //        StoreSlotCount = 0,
            //        SeqNum = -1,
            //        CreateTime = 1668344341319,
            //        FinishTime = 1668344641319,
            //        Level = 1,
            //        UpgradeTime = 1668344341319,
            //        UpgradeFinishTime = 1668344341319,
            //        WorkerId = "",
            //        ParentBuildingId = "",
            //        //ChildrenBuildingList = []
            //    }
            //};
            //using (MemoryStream sr = new MemoryStream())
            //{
            //    CodedOutputStream output = new CodedOutputStream(sr);
            //    r.WriteTo(sr);

            //    byte[] bytes = sr.ToArray();

            //    Console.WriteLine(string.Join(", ", bytes));

            //    var newr = Server.TrainChangeBuildingResult.Parser.ParseFrom(bytes);

            //    int i = 0;
            //}

            //string uri = "ws://10.10.50.92:3100/?token=cujao9zs486bu2z9a5";
            string uri = "ws://10.10.50.87:3100/?token=1akyz8mckhqchvdd0e";
            //string uri = "ws://121.40.165.18:8800";

            Console.WriteLine($"connect to {uri}"); 

            client.Connect(uri, 0);
            
            while(true)
            {
                string cmd = Console.ReadLine();
                if (cmd == "exit" || cmd == "quit")
                {
                    break;
                }
                else if(cmd== "getAllRes")
                {
                    //var par = new Common.CommonResult()
                    //{
                    //    Code = 1,
                    //    Msg = "tesss"
                    //};

                    //byte[] ary1;
                    //byte[] ary2;

                    //using (MemoryStream sr = new MemoryStream())
                    //{
                    //    CodedOutputStream output = new CodedOutputStream(sr);

                    //    output.WriteUInt32(10);

                    //    par.WriteTo(output);

                    //    output.Flush();
                    //    ary1 = sr.ToArray();

                    //    ary2 = par.ToByteArray();

                    //    int i = 0;
                    //}

                    UnityMiniGameFramework.IMessage msg = client.protocol.encode(11201, new Common.Empty());
                    client.Send(msg);
                }
                else if (cmd == "randName")
                {
                    UnityMiniGameFramework.IMessage msg = client.protocol.encode(11211, new Common.Empty());
                    client.Send(msg);
                }
                else if (cmd == "build")
                {
                    var par = new Client.CreateBuildingParams
                    {
                        BuildingConfigId = "101101",
                        LogicPos = new Common.LogicPos
                        {
                            X = 31,
                            Y = 22,
                            Z = 0
                        },
                        Orientation = 1
                    };
                    UnityMiniGameFramework.IMessage msg = client.protocol.encode(11301, par);
                    client.Send(msg);
                }
            }
        }
    }
}
