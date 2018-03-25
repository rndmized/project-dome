using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using ByteBufferDLL;
using EnumsServer;
using System.Net;
using System.Collections.Generic;

namespace ServerEcho
{
	class Globals
	{
		public static TcpClient[] clients = new TcpClient[20];
		public static Dictionary<int, Player> dicPlayers = new Dictionary<int, Player>();
	}
	class TCP_Server
	{
		static void Main(string[] args)
		{
			/*TcpListener serverSocket = new TcpListener(IPAddress.Any,5500);
			//TcpClient clientSocket = default(TcpClient);
			int counter = 0;

			serverSocket.Start();
			Console.WriteLine(" >> " + "TCP IP Server Started");

			while (true)
			{
				for (int i = 0; i < 20; i++)
				{
					if (Globals.clients[i] == null)
					{
						Globals.clients[i] = new TcpClient();
						Globals.clients[i] = serverSocket.AcceptTcpClient();
						Console.WriteLine(" >> " + "Client No:" + Convert.ToString(counter) + " started! " + Globals.clients[i].Client.LocalEndPoint);
						handleClinet client = new handleClinet();
						client.startClient(Globals.clients[i], counter);
						counter++;
					}
				}

			}

			//clientSocket.Close();
			serverSocket.Stop();
			Console.WriteLine(" >> " + "exit");
			Console.ReadLine();*/

			
		}

		public void start()
		{
			TcpListener serverSocket = new TcpListener(IPAddress.Any, 5500);
			//TcpClient clientSocket = default(TcpClient);
			int counter = 0;

			serverSocket.Start();
			Console.WriteLine(" >> " + "TCP IP Server Started");

			while (true)
			{
				for (int i = 0; i < 20; i++)
				{
					if (Globals.clients[i] == null)
					{
						Globals.clients[i] = new TcpClient();
						Globals.clients[i] = serverSocket.AcceptTcpClient();
						Console.WriteLine(" >> " + "Client No:" + Convert.ToString(counter) + " started! " + Globals.clients[i].Client.LocalEndPoint);
						handleClinet client = new handleClinet();
						client.startClient(Globals.clients[i], counter);
						counter++;
					}
				}

			}

			//clientSocket.Close();
			serverSocket.Stop();
			Console.WriteLine(" >> " + "exit");
			Console.ReadLine();
		}
	}

	public class handleClinet
	{
		TcpClient clientSocket;
		int clNo;
		int count = 1;
		public void startClient(TcpClient inClientSocket, int clineNo)
		{
			this.clientSocket = inClientSocket;
			this.clNo = clineNo;
			Thread ctThread = new Thread(doClient);
			ctThread.Start();
		}
		private void doClient()
		{
			int requestCount = 0;
			byte[] bytesFrom = new byte[10025];
			string dataFromClient = null;
			Byte[] sendBytes = null;
			string serverResponse = null;
			string rCount = null;
			requestCount = 0;
			NetworkStream networkStream = clientSocket.GetStream();


			while (!networkStream.DataAvailable) // waits for package with the auth key
			{
				ByteBuffer bbuffer = new ByteBuffer();

				networkStream.Read(bytesFrom, 0, 4096);
				bbuffer.WriteBytes(bytesFrom);

				String user = bbuffer.ReadString();
				user = RSA.Decypher(user);
				Player player = DB.GetPlayer(user);
				Globals.dicPlayers.Add(clNo, player);
				break;
			} 

			ByteBuffer buffer = new ByteBuffer();
			buffer.WriteInt((int)Enums.AllEnums.SSendingPlayerID);
			buffer.WriteInt(clNo);
			//networkStream = clientSocket.GetStream();
			networkStream.Write(buffer.ToArray(), 0, buffer.ToArray().Length);
			//networkStream.Flush();
			NotifyAlreadyConnected(clNo);
			NotifyMainPlayerOfAlreadyConnected(clNo);

			count++;
			while ((true))
			{
				try
				{
					requestCount = requestCount + 1;
					networkStream = clientSocket.GetStream();

					if (networkStream.DataAvailable)
					{
						//ByteBuffer buffer = new ByteBuffer();
						buffer = new ByteBuffer();
						networkStream.Read(bytesFrom, 0, 4096);
						buffer.WriteBytes(bytesFrom);

						int packageID = buffer.ReadInt();
						if (packageID == 4)
						{
							Console.WriteLine("Movement apckage received");
							Console.WriteLine(buffer.ToString());
						}
						HandleMessage(packageID, buffer.ReadInt(),buffer.ToArray()); //Maybe use ref instead of sending a byte array to save memory and a bit of performance
						
						/*string msg = "";
						//int id = buffer.ReadInt();
						//string msg = id + " : ";
						/*msg += buffer.ReadString();
						buffer = new ByteBuffer();
						//dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
						//dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
						Console.WriteLine(" >> " + "From client-" + clNo + msg);
						buffer.WriteString(msg);
						networkStream.Write(buffer.ToArray(), 0, buffer.ToArray().Length);
						networkStream.Flush();*/
					}


					/*rCount = Convert.ToString(requestCount);
					serverResponse = "Server to clinet(" + clNo + ") " + rCount;
					sendBytes = Encoding.ASCII.GetBytes(serverResponse);
					networkStream.Write(sendBytes, 0, sendBytes.Length);
					networkStream.Flush();
					Console.WriteLine(" >> " + serverResponse);*/
				}
				catch (Exception ex)
				{
					Console.WriteLine(" >> " + ex.ToString());
				}
			}
		}
		static void HandleMessage(int mID,int id, byte[] data)
		{
			switch (mID)
			{
				case (int)Enums.AllEnums.SSyncingPlayerMovement:
					Console.WriteLine("Packet movement: "+id);
					SendToAllBut(id, data);
					break;
				case (int)Enums.AllEnums.SSendingMessage:
					SendToAllBut(id, data);
					break;
			}
		}
			
		static void Handledata(int id, string msg)
		{
			for (int i = 0; i < 20; i++)
			{
				if (i != id)
				{
					Globals.clients[i].GetStream().Write(Encoding.ASCII.GetBytes(msg), 0, Encoding.ASCII.GetBytes(msg).Length);
				}
			}
		}

		static void NotifyMainPlayerOfAlreadyConnected(int id) // sends already connected to players current player
		{
			for (int i = 0; i < 20; i++)
			{
				if (Globals.clients[i] != null && Globals.clients[i].Connected)
				{
					if (i != id)
					{
						Console.WriteLine(i);
						ByteBuffer buffer = new ByteBuffer();
						buffer.WriteInt((int)Enums.AllEnums.SSendingAlreadyConnectedToMain);
						buffer.WriteInt(i);
						Thread.Sleep(100); //If the thread doesnt sleep, the packet is not sent
						//Console.WriteLine(Globals.clients[id].GetStream().);
						Globals.clients[id].GetStream().Write(buffer.ToArray(), 0, buffer.ToArray().Length);
						Globals.clients[id].GetStream().Flush();
						Console.WriteLine("Sending sync to "+id);
					}
				}
			}
		}

		static void NotifyAlreadyConnected(int id) // sends current player to already connected player 
		{
			ByteBuffer buffer = new ByteBuffer();
			buffer.WriteInt((int)Enums.AllEnums.SSendingMainToAlreadyConnected);
			buffer.WriteInt(id);
			for (int i = 0; i < 20; i++)
			{
				if (Globals.clients[i] != null && Globals.clients[i].Connected)
				{
					if (i != id)
					{
						Globals.clients[i].GetStream().Write(buffer.ToArray(), 0, buffer.ToArray().Length);
						Globals.clients[i].GetStream().Flush();
					}
				}
			}
		}

		static void SendToAllBut(int id, byte[] data)
		{
			ByteBuffer buffer = new ByteBuffer();
			buffer.WriteBytes(data);

			for (int i = 0; i < 20; i++)
			{
				if (Globals.clients[i] != null && Globals.clients[i].Connected)
				{
					if (i != id)
					{
						Console.WriteLine("Sending move from "+id+" to " + i);
						Console.WriteLine(i);
						Globals.clients[i].GetStream().Write(buffer.ToArray(), 0, buffer.ToArray().Length);
						Globals.clients[i].GetStream().Flush();
					}
				}
			}
		}

		static void SendMessage()
		{ }
	}

	public static class RSA
	{
		public static string Decypher(String text)
		{
			throw new Exception("To be implemented");
		}

		private static bool Autenthicate(String text)
		{
			throw new Exception("To be implemented");
		}
	}

	public static class DB
	{
		public static Player GetPlayer(String id)
		{
			throw new Exception("To be implemented");
		}
	}
}