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
using System.Threading.Tasks;

namespace ServerEcho
{
	class Globals //Class used to share data between threades
	{
		public static TcpClient[] clients = new TcpClient[20];
		public static TcpClient[] httpClient = new TcpClient[20];
		//public static Dictionary<string, Player> dicPlayers1 = new Dictionary<string, Player>();
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
		TcpListener httpSocket;
		int counter = 0;

		public TCP_Server(string path)
		{
			//load config file
			//StreamReader reader = new StreamReader(path);
			//string port = reader.ReadLine();
			serverSocket  = new TcpListener(IPAddress.Any, 5500);
			httpSocket = new TcpListener(IPAddress.Any, 5000);
		}

		static void Main(string[] args)
		{
			Globals.FeedDataToArray();
			TCP_Server tcp = new TCP_Server("");
			tcp.Start();
			
		}
		
		public void Start()
		{
			httpSocket.Start();
			serverSocket.Start();

			Task task = Task.Run(() => 
			{
				while (run)
				{
					for (int i = 0; i < Globals.httpClient.Length; i++)
					{
						Globals.httpClient[i] = new TcpClient();
						Globals.httpClient[i] = httpSocket.AcceptTcpClient();
					}
				}
			});
			int counter = 0;

			JwtTokens.LoadKey("");
			
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
			byte[] bytesFrom = new byte[4096];
			bool run = true;
			requestCount = 0;
			NetworkStream networkStream = clientSocket.GetStream();

			
			/*while (!networkStream.DataAvailable) {Thread.Sleep(50);}// waits for package with the auth key
			
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
			
			Globals.dicPlayers.Add(clNo, pl);*/

			

			//IF YOU WANT TO TEST WITH THE TEST SCENE, USE THIS CODE
			ByteBuffer buffer = new ByteBuffer();
			//buffer.WriteInt(0);
			buffer.WriteInt((int)Enums.AllEnums.SSendingPlayerID);
			//buffer.WriteInt(clNo);

			Player pl = Globals.GetPlayer();
			Globals.dicPlayers.Add(clNo, pl); 
			buffer.WriteString(pl.uName);
			buffer.WriteString(pl.cName);
			buffer.WriteInt(pl.head);
			buffer.WriteInt(pl.body);
			buffer.WriteInt(pl.cloths);

			/*byte[] size = BitConverter.GetBytes(buffer.Size());
			byte[] aux = buffer.ToArray();

			aux[0] = size[0];
			aux[1] = size[1];
			aux[2] = size[2];
			aux[3] = size[3];

			/*Console.WriteLine(buffer.Size());
			Console.WriteLine(buffer.ToArray().Length);*/
			//networkStream = clientSocket.GetStream();
			Globals.clients[clNo].GetStream().Write(buffer.ToArray(), 0, buffer.ToArray().Length);
			networkStream.Flush();
			Console.WriteLine(buffer.ToArray().Length+" to "+clNo);

			NotifyAlreadyConnected(clNo, pl);
			NotifyMainPlayerOfAlreadyConnected(clNo);

			count++;
			while (run)
			{
				try
				{
					requestCount++;
					networkStream = clientSocket.GetStream();

					if (networkStream.DataAvailable)
					{
						//ByteBuffer buffer = new ByteBuffer();
						buffer = new ByteBuffer();
						networkStream.Read(bytesFrom, 0, 4096);
						buffer.WriteBytes(bytesFrom);

						buffer.ReadInt(); // ignoring package size
						int packageID = buffer.ReadInt();

						if (packageID == (int)Enums.AllEnums.SCloseConnection)
						{
							run = false;
						}

						HandleMessage(packageID, clNo,buffer.ToArray()); 
						
					}

					//Thread.Sleep(50);

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
						ByteBuffer buffer = new ByteBuffer();
						//buffer.WriteInt(0);
						buffer.WriteInt((int)Enums.AllEnums.SSendingAlreadyConnectedToMain);
						buffer.WriteString(Globals.dicPlayers[i].uName);
						buffer.WriteString(Globals.dicPlayers[i].cName);
						buffer.WriteInt(Globals.dicPlayers[i].head);
						buffer.WriteInt(Globals.dicPlayers[i].body);
						buffer.WriteInt(Globals.dicPlayers[i].cloths);

						/*byte[] size = BitConverter.GetBytes(buffer.Size());
						byte[] aux = buffer.ToArray();

						aux[0] = size[0];
						aux[1] = size[1];
						aux[2] = size[2];
						aux[3] = size[3];*/

						//Thread.Sleep(1500); //If the thread doesnt sleep, the packet is not sent

						Console.WriteLine(buffer.ToArray().Length);

						Globals.clients[id].GetStream().Write(buffer.ToArray(), 0, buffer.ToArray().Length);
						//Globals.clients[id].GetStream().Flush();
						Console.WriteLine("Sending sync to "+id);
					}
				}
			}
		}

		static void NotifyAlreadyConnected(int id, Player p) // sends current player to already connected player 
		{
			ByteBuffer buffer = new ByteBuffer();

			//buffer.WriteInt(0);
			buffer.WriteInt((int)Enums.AllEnums.SSendingMainToAlreadyConnected);
			buffer.WriteString(p.uName);
			buffer.WriteString(p.cName);
			buffer.WriteInt(p.head);
			buffer.WriteInt(p.body);
			buffer.WriteInt(p.cloths);

			/*byte[] size = BitConverter.GetBytes(buffer.Size());
			byte[] aux = buffer.ToArray();

			aux[0] = size[0];
			aux[1] = size[1];
			aux[2] = size[2];
			aux[3] = size[3];*/

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
			/*ByteBuffer buffer = new ByteBuffer();
			buffer.WriteBytes(data);*/
			
			for (int i = 0; i < 20; i++)
			{
				if (Globals.clients[i] != null && Globals.clients[i].Connected)
				{
					if (i != id)
					{
						Console.WriteLine("Sending move from "+id+" to " + i);
						Globals.clients[i].GetStream().Write(data, 0, data.Length);
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

	/// <summary>
	/// The class that handles the incoming connections from clients
	/// Contains all methods to handle received data packets and send packets to clients
	/// </summary>
	public class HandleHttpClient
	{
		private TcpClient clientSocket;
		private int clNo;

		public HandleHttpClient()
		{

		}
		public void StartHttpClient(TcpClient inClientSocket, int clineNo)
		{
			this.clientSocket = inClientSocket;
			this.clNo = clineNo;
			Thread ctThread = new Thread(HandleClient);
			ctThread.Start();
		}

		private void HandleClient()
		{
			int requestCount = 0;
			byte[] bytesFrom = new byte[4096];
			bool run = true;
			requestCount = 0;
			NetworkStream networkStream = clientSocket.GetStream();

			while (run)
			{
				if(!networkStream.DataAvailable) { Thread.Sleep(50); }

				networkStream.Read(bytesFrom, 0, 4096);
			}
		}

		private void HandleID(int id,byte[] data)
		{
			switch (id)
			{
				case (int)Enums.AllEnums.HChangeSettings:
					{
						break;
					}
				case (int)Enums.AllEnums.HGetSettings:
					{
						break;
					}
				case (int)Enums.AllEnums.HKickPlayer:
					{
						break;
					}
				case (int)Enums.AllEnums.HListPlayers:
					{
						break;
					}
				case (int)Enums.AllEnums.HRestartServer:
					{
						break;
					}
			}
		}

	}
}