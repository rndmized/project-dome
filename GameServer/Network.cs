using System;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    class Network
    {
        public TcpListener ServerSocket;

        public void InitTCP()
        {
            ServerSocket = new TcpListener(IPAddress.Any, 5500);
            ServerSocket.Start();
            ServerSocket.BeginAcceptTcpClient(OnClientConnect, null);
            Console.WriteLine("Server has successfully started.");
        }

        void OnClientConnect(IAsyncResult result)
        {
            TcpClient client = ServerSocket.EndAcceptTcpClient(result);
            client.NoDelay = false;
            ServerSocket.BeginAcceptTcpClient(OnClientConnect, null);

            for (int i = 1; i < Constants.MAX_PLAYERS; i++)
            {
                if (Globals.Clients[i].Socket == null)
                {
                    Globals.Clients[i].Socket = client;
                    Globals.Clients[i].Index = i;
                    Globals.Clients[i].IP = client.Client.RemoteEndPoint.ToString();
                    Globals.Clients[i].Start();
                    Console.WriteLine("Incoming Connection from " + Globals.Clients[i].IP + "|| Index: " + i);
                    Globals.networkSendData.SendJoinGame(i);
                    return;
                }
            }
        }
    }
}
