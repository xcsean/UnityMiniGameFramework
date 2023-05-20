namespace UnityMiniGameFramework
{
    public class NetManager
    {
        public void ConnectServer(string host, int port)
        {
            ClientSocket.GetInstance().SetIp(host, port);
            ClientSocket.GetInstance().Connect();
        }
    }
}