using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using ByteBufferDLL;
using EnumsServer;
using System.Net;
using System.Collections.Generic;
using MongoDB.Driver;
using System.IO;

namespace ServerEcho
{
	class Globals //Class used to share data between threades
	{
		public static TcpClient[] clients = new TcpClient[20];
		public static Dictionary<string, Player> dicPlayers1 = new Dictionary<string, Player>();
		public static Dictionary<int, Player> dicPlayers = new Dictionary<int, Player>();
		public static int i = -1;
		private static Player[] p = new Player[2];

		public static void FeedDataToArray()
		{
			p[0] = new Player();
			p[0].cName = "ubaduba";
			p[0].uName = "Player1";
			p[0].head = 4;
			p[0].body = 0;
			p[0].cloths = 4;

			p[1] = new Player();
			p[1].cName = "sfsadfsafd";
			p[1].uName = "Player2";
			p[1].head = 3;
			p[1].body = 0;
			p[1].cloths = 3;
		}

		public static Player GetPlayer()
		{
			i++;
			return p[i];
		}

	}

	/// <summary>
	/// The class that starts the server
	/// Contains method to load the server config and start the server.
	/// </summary>
	class TCP_Server
	{
		private bool run = true;
		TcpListener serverSocket;
		int counter = 0;

		public TCP_Server(string path)
		{
			//load config file
			serverSocket  = new TcpListener(IPAddress.Any, 5500);
		}

		static void Main(string[] args)
		{
			Globals.FeedDataToArray();
			TCP_Server tcp = new TCP_Server("");
			tcp.Start();
			
		}

		public void Start()
		{
			int counter = 0;
			JwtTokens.LoadKey("");
			serverSocket.Start();
			Console.WriteLine(" >> TCP IP Server Started");

			while (run)
			{
				for (int i = 0; i < Globals.clients.Length; i++)
				{
					if (Globals.clients[i] == null)
					{
						Globals.clients[i] = new TcpClient();
						Globals.clients[i] = serverSocket.AcceptTcpClient();
						Console.WriteLine(" >> " + "Client No:" + Convert.ToString(counter) + " started! " + Globals.clients[i].Client.LocalEndPoint);
						HandleClinet client = new HandleClinet();
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

		public void CloseSocket()
		{
			for (int i = 0; i < Globals.clients.Length; i++)
			{
				if (Globals.clients[i] != null)
				{
					//update db
					Globals.clients[i].Close();
				}
			}
			serverSocket.Stop();
		}
	}

	/// <summary>
	/// The class that handles the incoming connections from clients
	/// Contains all methods to handle received data packets and send packets to clients
	/// </summary>
	public class HandleClinet
	{
		private TcpClient clientSocket;
		private int clNo;
		private int count = 1;
		

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
			bool run = true;
			requestCount = 0;
			NetworkStream networkStream = clientSocket.GetStream();

			
			//while (!networkStream.DataAvailable) {Thread.Sleep(50);}// waits for package with the auth key
			
			ByteBuffer buffer = new ByteBuffer();

			networkStream.Read(bytesFrom, 0, 4096);
			buffer.WriteBytes(bytesFrom);

			JwtTokens.EvaluateToken(buffer.ReadString());
			Player pl = new Player();
			pl.uName = buffer.ReadString();
			pl.cName = buffer.ReadString();
			pl.head = buffer.ReadInt();
			pl.body = buffer.ReadInt();
			pl.cloths = buffer.ReadInt();
			pl.socketID = clNo;
			
			Globals.dicPlayers.Add(clNo, pl);

			

			//IF YOU WANT TO TEST WITH THE TEST SCENE, USE THIS CODE
			/*ByteBuffer buffer = new ByteBuffer();
			buffer.WriteInt((int)Enums.AllEnums.SSendingPlayerID);
			//buffer.WriteInt(clNo);

			//Player pl = Globals.GetPlayer();
			Globals.dicPlayers.Add(clNo, pl); 
			buffer.WriteString(pl.uName);
			buffer.WriteString(pl.cName);
			buffer.WriteInt(pl.head);
			buffer.WriteInt(pl.body);
			buffer.WriteInt(pl.cloths);
			//networkStream = clientSocket.GetStream();
			networkStream.Write(buffer.ToArray(), 0, buffer.ToArray().Length);
			//networkStream.Flush();*/



			NotifyAlreadyConnected(clNo, pl);
			NotifyMainPlayerOfAlreadyConnected(clNo);

			count++;
			while (run)
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

						if (packageID == (int)Enums.AllEnums.SCloseConnection)
						{
							run = false;
						}

						HandleMessage(packageID, clNo,buffer.ToArray()); //Maybe use ref instead of sending a byte array to save memory and a bit of performance
						
					}

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
					{
						//Console.WriteLine("Packet movement: " + id);
						SendToAllBut(id, data);
						break;
					}
				case (int)Enums.AllEnums.SSendingMessage:
					{
						SendToAllBut(id, data);
						break;
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
						Player aux = Globals.dicPlayers[i];
						ByteBuffer buffer = new ByteBuffer();
						buffer.WriteInt((int)Enums.AllEnums.SSendingAlreadyConnectedToMain);
						buffer.WriteString(aux.uName);
						buffer.WriteString(aux.cName);
						buffer.WriteInt(aux.head);
						buffer.WriteInt(aux.body);
						buffer.WriteInt(aux.cloths);
						Thread.Sleep(100); //If the thread doesnt sleep, the packet is not sent
										   //Console.WriteLine(Globals.clients[id].GetStream().);

						
						Globals.clients[id].GetStream().Write(buffer.ToArray(), 0, buffer.ToArray().Length);
						Globals.clients[id].GetStream().Flush();
						Console.WriteLine("Sending sync to "+id);
					}
				}
			}
		}

		static void NotifyAlreadyConnected(int id, Player p) // sends current player to already connected player 
		{
			ByteBuffer buffer = new ByteBuffer();
			buffer.WriteInt((int)Enums.AllEnums.SSendingMainToAlreadyConnected);
			buffer.WriteString(p.uName);
			buffer.WriteString(p.cName);
			buffer.WriteInt(p.head);
			buffer.WriteInt(p.body);
			buffer.WriteInt(p.cloths);

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
						Globals.clients[i].GetStream().Write(buffer.ToArray(), 0, buffer.ToArray().Length);
						Globals.clients[i].GetStream().Flush();
					}
				}
			}
		}

		static void SendMessage()
		{

		}

		static void CloseConnection(int id)
		{
			Globals.clients[id].Client.Close();
			//Update player playtime
		}
	}

	public static class JwtTokens
	{
		private static string key;

		public static bool EvaluateToken(string text)
		{
			try
			{
				Jose.JWT.Decode(text, Encoding.ASCII.GetBytes(key));
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static void LoadKey(string path)
		{
			key = "";
			/*StreamReader reader = new StreamReader(path);
			key = reader.ReadLine();
			reader.Close();*/
		}

	}

	public class DB //Singleton
	{
		private DB _db;
		private IMongoDatabase mongodb;
		MongoClient client;

		private DB() { }
		public DB getInstance(string path, string port, string dbName)
		{
			if (_db == null)
			{
				_db = new DB();
				client = new MongoClient();
				mongodb = client.GetDatabase(dbName);
			}

			return _db;
		}
		public Player GetPlayer(string uName,string cName)
		{
			var coll = mongodb.GetCollection<Player>(""); //collection's name in db

			var p = coll.Find(pl => pl.uName == uName && pl.cName==cName);

			return (Player)p;
		}
	}
}