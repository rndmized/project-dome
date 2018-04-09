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
using System.Text.RegularExpressions;

namespace ServerEcho
{
	class Globals //Class used to share data between threades
	{
		public static TcpClient[] clients = new TcpClient[20];
		public static TcpClient[] httpClient = new TcpClient[2];
		public static Dictionary<int, Player> dicPlayers = new Dictionary<int, Player>();
		public static int i = 0;
		public static int port = 0;
		public static bool restart = false;
		public static bool mainRun = true;
		public static bool threadsRun = true;

		private static Player[] p = new Player[3];
		public static void FeedDataToArray()
		{
			p[2] = new Player();
			p[2].cName = "ubaduba";
			p[2].uName = "Player1";
			p[2].head = 4;
			p[2].body = 0;
			p[2].cloths = 4;

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

		public static void ChangeNoOfPlayers(int n)
		{
			clients = new TcpClient[n];
		}
	}

	/// <summary>
	/// The class that starts the server
	/// Contains method to load the server config and start the server.
	/// </summary>
	class TCP_Server
	{
		private string path;
		private bool run = true;
		TcpListener serverSocket;
		TcpListener httpSocket;
		int counter = 0;

		public TCP_Server(string path)
		{
			this.path = path;
			LoadConfig(path);

			serverSocket  = new TcpListener(IPAddress.Any, Globals.port);
			httpSocket = new TcpListener(IPAddress.Any, 5000);
		}

		private void LoadConfig(string path)
		{
			StreamReader reader = new StreamReader(path);
			string line = reader.ReadLine();
			Globals.port = int.Parse(line.Substring(line.LastIndexOf('=') + 1));

			line = reader.ReadLine();

			reader.Close();

			int nOfPlayers = int.Parse(line.Substring(line.LastIndexOf('=') + 1));
			Globals.ChangeNoOfPlayers(nOfPlayers);
		}

		static void Main(string[] args)
		{
			Globals.FeedDataToArray();
			TCP_Server tcp = new TCP_Server("D://config.txt");
			tcp.Start();
			
		}

		public void Start()
		{
			httpSocket.Start();
			

			Task task = Task.Run(() => 
			{
				while (run)
				{
					for (int i = 0; i < Globals.httpClient.Length; i++)
					{
						Globals.httpClient[i] = new TcpClient();
						try
						{
							Globals.httpClient[i] = httpSocket.AcceptTcpClient();
						}
						catch { break; }
						Console.WriteLine(">> New connection from http server");
						HandleHttpClient httpClient = new HandleHttpClient();
						httpClient.StartHttpClient(Globals.httpClient[i],i);
					}
				}
			});

			int counter = 0;

			JwtTokens.LoadKey("path to file containing key");
			
			serverstart:
			serverSocket.Start();
			Console.WriteLine(">> TCP IP Server Started on port "+ Globals.port);
			while (Globals.mainRun)
			{
				for (int i=0; i < Globals.clients.Length; i++)
				{
					if (Globals.clients[i] == null)
					{
						Globals.clients[i] = new TcpClient();
						Globals.clients[i] = serverSocket.AcceptTcpClient();
						if (!Globals.mainRun) break;
						Console.WriteLine(" >> Client No:" + Convert.ToString(counter) + " started! " + Globals.clients[i].Client.LocalEndPoint);
						HandleClinet client = new HandleClinet();
						client.startClient(Globals.clients[i], counter, Globals.clients[i].Client.LocalEndPoint.ToString());
						counter++;
					}
				}

			}

			CloseSocket();
			Console.WriteLine(">> Closing Server Socket");
			serverSocket.Stop();
			if (Globals.restart)
			{
				Globals.restart = false;
				Globals.threadsRun = true;
				Globals.mainRun = true;

				LoadConfig(path);
				serverSocket = new TcpListener(IPAddress.Any, Globals.port);
				goto serverstart;
			}
		}

		public void CloseSocket()
		{
			for (int i = 0; i < Globals.clients.Length; i++)
			{
				if (Globals.clients[i] != null)
				{
					//update db
					Console.WriteLine("Closing socket "+i+1);
					Globals.clients[i].Close();
					Globals.clients[i] = null;
				}
			}
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
		private string ip;
		private bool run = true;

		public void startClient(TcpClient inClientSocket, int clineNo, string ip)
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

			
			while (!networkStream.DataAvailable) {Thread.Sleep(50);}// waits for package with the auth key
			
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
			//pl.socketID = clNo;
			
			Globals.dicPlayers.Add(clNo, pl);

			

			//IF YOU WANT TO TEST WITH THE TEST SCENE, USE THIS CODE
			/*ByteBuffer buffer = new ByteBuffer();
			//buffer.WriteInt(0);
			buffer.WriteInt((int)Enums.AllEnums.SSendingPlayerID);
			//buffer.WriteInt(clNo);

			Player pl = Globals.GetPlayer();
			Globals.dicPlayers.Add(clNo, pl); 
			buffer.WriteString(pl.uName);
			buffer.WriteString(pl.cName);
			buffer.WriteInt(pl.head);
			buffer.WriteInt(pl.body);
			buffer.WriteInt(pl.cloths);*/

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
			while (Globals.threadsRun && run)
			{
				try
				{
					requestCount++;
					networkStream = clientSocket.GetStream();

					if (networkStream.DataAvailable)
					{
						buffer = new ByteBuffer();
						networkStream.Read(bytesFrom, 0, 4096);
						buffer.WriteBytes(bytesFrom);

						buffer.ReadInt(); // ignoring package size
						int packageID = buffer.ReadInt();

						if (packageID == (int)Enums.AllEnums.SCloseConnection)
						{
							run = false;
							//CloseConnection(clNo);
							break;
						}

						HandleMessage(packageID, clNo,buffer.ToArray()); 
						
					}

					Thread.Sleep(50);

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

		public static void SendToAllBut(int id, byte[] data)
		{
			for (int i = 0; i < 20; i++)
			{
				if (Globals.clients[i] != null && Globals.clients[i].Connected)
				{
					if (i != id)
					{
						Globals.clients[i].GetStream().Write(data, 0, data.Length);
						Globals.clients[i].GetStream().Flush();
					}
				}
			}
		}

		static void SendToSpecific(int id, byte[] data)
		{
			if (Globals.clients[id] != null && Globals.clients[id].Connected)
			{
				Globals.clients[id].GetStream().Write(data, 0, data.Length);
				Globals.clients[id].GetStream().Flush();
				
			}
			
		}

		static void SendMessage(int id, byte[] data,bool sendToAll)
		{
			if (sendToAll)
				SendToAllBut(id, data);
			else
			{
				ByteBuffer buffer = new ByteBuffer();
				buffer.WriteBytes(data);
				buffer.ReadInt();
				string pName = buffer.ReadString();
				int size = Globals.dicPlayers.Count; //Saves size in memory to avoid exeception if a player disconnect during execution of this method
				for (int i = 0; i < size; i++)
				{
					if (pName == Globals.dicPlayers[i].uName)
					{
						SendToSpecific(Globals.dicPlayers[i].socketID,data);
						break;
					}
				}
			}
		}

	}

	public static class JwtTokens
	{
		private static string key;

		public static bool EvaluateToken(string text)
		{
			return true;
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
		public void UpdatePlayer(Player p)
		{ }
	}

	/// <summary>
	/// The class that handles the incoming connections from clients
	/// Contains all methods to handle received data packets and send packets to clients
	/// </summary>
	public class HandleHttpClient
	{
		private TcpClient clientSocket;
		private int clNo;

		public void StartHttpClient(TcpClient inClientSocket, int clineNo)
		{
			this.clientSocket = inClientSocket;
			this.clNo = clineNo;
			Thread ctThread = new Thread(HandleClient);
			ctThread.Start();
		}

		private void HandleClient()
		{
			byte[] bytesFrom = new byte[4096];
			bool run = true;
			NetworkStream networkStream = clientSocket.GetStream();

			while (run)
			{
				while(!networkStream.DataAvailable) { Thread.Sleep(50); }

				networkStream.Read(bytesFrom, 0, 4096);

				ByteBuffer buffer = new ByteBuffer();
				buffer.WriteBytes(bytesFrom);
				short id=buffer.ReadByte();
				string token = buffer.ReadString();
				if (!JwtTokens.EvaluateToken(token))
				{ }
				else { HandleID(id, bytesFrom); }
			}
		}

		
		private void HandleID(short id,byte[] data)
		{
			switch (id)
			{
				case (short)Enums.AllEnums.HChangeSettings:
					{
						HChangeSettings(data);
						break;
					}
				case (short)Enums.AllEnums.HGetSettings:
					{
						HGetSettings();
						break;
					}
				case (short)Enums.AllEnums.HKickPlayer:
					{
						HKickPlayer(data);
						break;
					}
				case (short)Enums.AllEnums.HListPlayers:
					{
						HListPlayers();
						break;
					}
				case (short)Enums.AllEnums.HRestartServer:
					{
						HRestartServer();
						break;
					}
			}
		}

		/// <summary>
		/// **************Change file path*******************
		/// Changes the server's cofiguration file
		/// </summary>
		/// <param name="data">Byte array containing new server configuration</param>
		private void HChangeSettings(byte[] data)
		{
			ByteBuffer buffer = new ByteBuffer();
			buffer.WriteBytes(data);
			buffer.ReadByte();
			int nOfPlayers = buffer.ReadInt();
			int port = buffer.ReadInt();

			string configFile = "port=" + port+ System.Environment.NewLine+"NumberOfPlayers=" +nOfPlayers;
			try
			{
				StreamWriter writer = new StreamWriter("D://config.txt");
				writer.Write(configFile);
				writer.Close();
			}
			catch (Exception)
			{
				SendDefaultRespose(false);
			}
			SendDefaultRespose(true);
		}

		/// <summary>
		/// Returns server current configuration 
		/// </summary>
		private void HGetSettings()
		{
			List<byte> buffer = new List<byte>();
			StreamReader reader = new StreamReader("D:\\config.txt");
			string line = reader.ReadLine();
			int aux= int.Parse(line.Substring(line.LastIndexOf('=') + 1));
			buffer.AddRange(BitConverter.GetBytes(aux));

			line = reader.ReadLine();
			aux = int.Parse(line.Substring(line.LastIndexOf('=') + 1));
			buffer.AddRange(BitConverter.GetBytes(aux));

			reader.Close();

			/*byte[] buffer = new byte[Encoding.ASCII.GetByteCount(file)];
			buffer = Encoding.ASCII.GetBytes(file);*/

			Globals.httpClient[clNo].GetStream().Write(buffer.ToArray(), 0, buffer.ToArray().Length);
		}

		/// <summary>
		/// ********NOT FISHED*********
		/// Closes the connection of a specific player
		/// </summary>
		private void HKickPlayer(byte[] data)
		{
			ByteBuffer buffer = new ByteBuffer();
			buffer.WriteBytes(data);

			string playerid = buffer.ReadString();
			for (int i = 0; i < Globals.dicPlayers.Count; i++)
			{
				if (Globals.dicPlayers[i].uName == playerid)
				{
					//update playtime on db
					//update index
					CloseConnection(i);
					/*Globals.clients[i].Close();
					Globals.clients[i] = null;
					Globals.i = i;*/
					break;
				}
			}

			
		}
		
		/// <summary>
		/// Returns stats of all current connected players
		/// </summary>
		private void HListPlayers()
		{
			ByteBuffer buffer = new ByteBuffer();
			TimeSpan aux = new TimeSpan();
			
			foreach (Player p in Globals.dicPlayers.Values)
			{
				buffer.WriteString(p.uName);
				buffer.WriteString(p.cName);
				buffer.WriteString(p.playerIP);
				aux = DateTime.Now - p.currentPlaytime;
				buffer.WriteInt(aux.Hours*60+aux.Minutes); 
				buffer.WriteInt(p.totalPlaytime+(aux.Hours * 60 + aux.Minutes));
			}

			Globals.httpClient[clNo].GetStream().Write(buffer.ToArray(), 0, buffer.ToArray().Length);
		}

		/// <summary>
		/// Set the flag to restart the server
		/// </summary>
		private void HRestartServer()
		{
			Globals.restart = true;
			Globals.mainRun = false;
			Console.WriteLine(Globals.mainRun);
			TcpClient client = new TcpClient();
			client.Connect("", Globals.port);
			client.Close();
		}

		private void SendDefaultRespose(bool success)
		{
			byte[] buffer = new byte[1];
			buffer = BitConverter.GetBytes(success);
			Globals.httpClient[clNo].GetStream().Write(buffer, 0, buffer.Length);
		}

		//UPDATE DB
		static void CloseConnection(int id)
		{
			ByteBuffer buffer = new ByteBuffer();
			buffer.WriteInt((int)Enums.AllEnums.SCloseConnection);
			buffer.WriteString(Globals.dicPlayers[id].uName);

			HandleClinet.SendToAllBut(id, buffer.ToArray());
			Globals.dicPlayers.Remove(id);
			Globals.clients[id].Client.Close();
			Globals.clients[id] = null;
			//Update player playtime
		}
	}
}