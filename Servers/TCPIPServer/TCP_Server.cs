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
using MongoDB.Bson;
using TCPServer;
using System.Linq.Expressions;

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

		//private static Player[] p = new Player[3];
	
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

			DB db = DB.getInstance("mongodb://admin:admin@ds221339.mlab.com:21339/project-dome", "GameDB");
		}

		static void Main(string[] args)
		{
			
			TCP_Server tcp = new TCP_Server("config.txt");
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
					Thread.Sleep(1000);
				}
			});

			int counter = 0;

			JwtTokens.LoadKey("key.txt");
			
			serverstart:
			serverSocket.Start();
			Globals.dicPlayers = new Dictionary<int, Player>();
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
			this.ip = ip;
			this.clientSocket = inClientSocket;
			this.clNo = clineNo;
			Thread ctThread = new Thread(doClient);
			ctThread.Start();
		}

		private void doClient() 
		{
			int requestCount = 0;
			byte[] bytesFrom = new byte[4096];
			requestCount = 0;
			NetworkStream networkStream = clientSocket.GetStream();

			
			while (!networkStream.DataAvailable) {Thread.Sleep(50);}// waits for package with the auth key
			
			ByteBuffer buffer = new ByteBuffer();

			networkStream.Read(bytesFrom, 0, 4096);
			buffer.WriteBytes(bytesFrom);
			buffer.ReadInt();
			string token = buffer.ReadString();
			if (!JwtTokens.EvaluateToken(token))
			{
				byte[] bufferE = new byte[4];
				bufferE[0] = (byte)Enums.AllEnums.SInvalid;
				networkStream.Write(bufferE, 0, 4);
				return;
			}

			Player pl = new Player();
			pl.uName = buffer.ReadString();
			pl.cName = buffer.ReadString();
			pl.head = buffer.ReadInt();
			pl.body = buffer.ReadInt();
			pl.cloths = buffer.ReadInt();
			pl.currentPlaytime = DateTime.Now;
			pl.playerIP = ip;
			pl.totalPlaytime = DB.getInstance("","").GetPlayTime(pl.uName);
			pl.socketID = clNo;
			
			Globals.dicPlayers.Add(clNo, pl);

			Globals.clients[clNo].GetStream().Write(buffer.ToArray(), 0, buffer.ToArray().Length);
			networkStream.Flush();
			

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

						int packageID = buffer.ReadInt();

						if (packageID == (int)Enums.AllEnums.SCloseConnection)
						{
							run = false;
							break;
						}

						HandleMessage(packageID, clNo,buffer.ToArray()); 
						
					}

					Thread.Sleep(50);

				}
				catch (Exception ex)
				{
					try
					{
						if (!Globals.clients[clNo].Connected)
						{
							SCloseConnection(clNo);
						}
						Globals.clients[clNo] = null;
					}
					catch (Exception e) { }
					finally
					{
						byte[] closeBuffer = new byte[5];
						closeBuffer[0] = 7;
						closeBuffer[4] = (byte)clNo;
						run = false;
					}
				}
			}

			
		}

		static void HandleMessage(int mID,int id, byte[] data)
		{
			switch (mID)
			{
				case (int)Enums.AllEnums.SSyncingPlayerMovement:
					{
						SendToAllBut(id, data);
						break;
					}
				case (int)Enums.AllEnums.SSendingMessageWorld:
					{
						SendToAllBut(id, data);
						break;
					}
				case (int)Enums.AllEnums.SSendingMessage:
					{
						SendMessage(id, data);
						break;
					}
				case (int)Enums.AllEnums.SCloseConnection:
					{
						SCloseConnection(id);
						break;
					}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		static void SCloseConnection(int id)
		{
			ByteBuffer buffer = new ByteBuffer();
			buffer.WriteInt((int)Enums.AllEnums.SCloseConnection);
			buffer.WriteString(Globals.dicPlayers[id].uName);

			SendToAllBut(id, buffer.ToArray());

			Globals.clients[id].Client.Close();
			Globals.clients[id] = null;
			Console.WriteLine(">> Closing connection from player " + Globals.dicPlayers[id].uName);
			DB.getInstance("", "").UpdatePlayerPlayTime(Globals.dicPlayers[id]);
			try
			{
				Globals.dicPlayers.Remove(id);
			}
			catch (Exception) { }
		}

		/// <summary>
		/// Sends already connected clients to the connecting one
		/// </summary>
		/// <param name="id"></param>
		/// <param name="p"></param>
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
						buffer.WriteString(Globals.dicPlayers[i].uName);
						buffer.WriteString(Globals.dicPlayers[i].cName);
						buffer.WriteInt(Globals.dicPlayers[i].head);
						buffer.WriteInt(Globals.dicPlayers[i].body);
						buffer.WriteInt(Globals.dicPlayers[i].cloths);
						buffer.WriteFloat(Globals.dicPlayers[i].cX);
						buffer.WriteFloat(Globals.dicPlayers[i].cY);
						buffer.WriteFloat(Globals.dicPlayers[i].cZ);
						Thread.Sleep(1000); //If the thread doesnt sleep, the packet is not sent
											//Console.WriteLine(buffer.ToArray().Length+" to "+id);
						try
						{
							Globals.clients[id].GetStream().Write(buffer.ToArray(), 0, buffer.ToArray().Length);
						}
						catch (Exception) { continue; }
					}
				}
			}
		}

		/// <summary>
		/// Notifies already connected clients of new player
		/// </summary>
		/// <param name="id"></param>
		/// <param name="p"></param>
		static void NotifyAlreadyConnected(int id, Player p)  
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
				if (Globals.clients[i] != null && Globals.clients[i].Connected) // sends current player to already connected player 
				{
					if (i != id)
					{
						try
						{
							Globals.clients[i].GetStream().Write(buffer.ToArray(), 0, buffer.ToArray().Length);
							Globals.clients[i].GetStream().Flush();
						}
						catch (Exception) { continue; }
						
					}
				}
			}
		}

		/// <summary>
		/// Sends a packet to a all connected client but a specific one
		/// </summary>
		/// <param name="id">client's id</param>
		/// <param name="data">Array with packet data to be sent</param>
		public static void SendToAllBut(int id, byte[] data)
		{
			for (int i = 0; i < 20; i++)
			{
				if (Globals.clients[i] != null && Globals.clients[i].Connected)
				{
					try
						{
							Globals.clients[i].GetStream().Write(data, 0, data.Length);
							Globals.clients[i].GetStream().Flush();
						}
						catch (Exception) { continue; }
				}
			}
		}

		/// <summary>
		/// Sends a packet to a specific connected client
		/// </summary>
		/// <param name="id">client's id</param>
		/// <param name="data">Array with packet data to be sent</param>
		static void SendToSpecific(int id, byte[] data)
		{
			if (Globals.clients[id] != null && Globals.clients[id].Connected)
			{
				try
				{
					Globals.clients[id].GetStream().Write(data, 0, data.Length);
					Globals.clients[id].GetStream().Flush();
				}
				catch (Exception) { }

			}
			
		}

		/// <summary>
		/// Sends a message from a player to another
		/// </summary>
		/// <param name="id"></param>
		/// <param name="data"></param>
		/// <param name="sendToAll"></param>
		static void SendMessage(int id, byte[] data)
		{
			ByteBuffer buffer = new ByteBuffer();
				buffer.WriteBytes(data);
				buffer.ReadInt();
				string pName = buffer.ReadString();
				int size = Globals.dicPlayers.Count; //Saves size in memory to avoid exeception if a player disconnect during execution of this method
				for (int i = 0; i < size; i++)
				{
					try
					{
						if (pName == Globals.dicPlayers[i].uName)
						{
							SendToSpecific(Globals.dicPlayers[i].socketID, data);
							break;
						}
					}
					catch (Exception) { break; }
				}
			
		}

	}

	/// <summary>
	/// Class responsible to authenticate token
	/// </summary>
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
			StreamReader reader = new StreamReader(path);
			key = reader.ReadLine();
			reader.Close();
		}

	}

	/// <summary>
	/// Connector to the MongoDB database
	/// </summary>
	public class DB //Singleton
	{
		private static DB _db;
		private static IMongoDatabase mongodb;
		static MongoClient client;

		private DB() { }
		public static DB getInstance(string path, string dbName)
		{
			if (_db == null)
			{
				_db = new DB();

				client = new MongoClient(path);
				mongodb = client.GetDatabase("project-dome");
			}

			return _db;
		}
		
		/// <summary>
		/// Gets the total play time from the DB
		/// </summary>
		/// <param name="uName">user name</param>
		/// <returns>play time</returns>
		public int GetPlayTime(string uName)
		{
			IMongoCollection<UserOnDB> collection = mongodb.GetCollection<UserOnDB>("users");

			//IMongoCollection<BsonDocument> collection = mongodb.GetCollection<BsonDocument>("users");
			Expression<Func<UserOnDB, bool>> filter = x => x.Username.Contains(uName);
			UserOnDB player = collection.Find(filter).FirstOrDefault();

			return player.Playtime;
		}
		
		/// <summary>
		/// Updates player total playtime
		/// </summary>
		/// <param name="p">Reference to the player</param>
		public void UpdatePlayerPlayTime(Player p)
		{
			var collection = mongodb.GetCollection<BsonDocument>("users");

			var aux = DateTime.Now - p.currentPlaytime;
			
			var filter = Builders<BsonDocument>.Filter.Eq("username", p.uName);
			var update = Builders<BsonDocument>.Update.Set("playtime", aux.Hours * 60 + aux.Minutes);

			collection.UpdateOne(filter, update);
		}

	}

	/// <summary>
	/// The class that handles the incoming connections from clients
	/// Contains all methods to handle received data packets and send packets to clients
	/// </summary>
	public class HandleHttpClient
	{
		private TcpClient clientSocket;
		static private int clNo;
		bool run = false;

		public void StartHttpClient(TcpClient inClientSocket, int clineNo)
		{
			this.clientSocket = inClientSocket;
			clNo = clineNo;
			Thread ctThread = new Thread(HandleClient);
			run = true;
			ctThread.Start();
		}

		private void HandleClient()
		{
			byte[] bytesFrom = new byte[4096];
			
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
				{
					SendDefaultRespose(false, clNo);
				}
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
			byte restart = buffer.ReadByte();
			string configFile = "port=" + port+ System.Environment.NewLine+"NumberOfPlayers=" +nOfPlayers;
			try
			{
				StreamWriter writer = new StreamWriter("config.txt");
				writer.Write(configFile);
				writer.Close();
			}
			catch (Exception)
			{
				SendDefaultRespose(false, clNo);
			}
			SendDefaultRespose(true, clNo);

			if (restart == 1) HRestartServer();
		}

		/// <summary>
		/// Returns server current configuration 
		/// </summary>
		private void HGetSettings()
		{
			List<byte> buffer = new List<byte>();
			StreamReader reader = new StreamReader("config.txt");
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
		/// Closes the connection of a specific player
		/// </summary>
		private void HKickPlayer(byte[] data)
		{
			ByteBuffer buffer = new ByteBuffer();
			buffer.WriteBytes(data);
			buffer.ReadByte();
			buffer.ReadString();
			string playerid = buffer.ReadString();

			foreach (Player p in Globals.dicPlayers.Values)
			{
				if (p.uName == playerid)
				{
					CloseConnection(p.socketID);
					break;
				}
			}
			//SendDefaultRespose(true,clNo);
		}
		
		/// <summary>
		/// Returns stats of all current connected players
		/// </summary>
		private void HListPlayers()
		{
			ByteBuffer buffer = new ByteBuffer();
			TimeSpan aux = new TimeSpan();

			if (Globals.dicPlayers.Count == 0)
			{
				buffer.WriteByte(0);
				Globals.httpClient[clNo].GetStream().Write(buffer.ToArray(), 0, buffer.ToArray().Length);
				return;
			}
			buffer.WriteInt(Globals.dicPlayers.Count);
			foreach (Player p in Globals.dicPlayers.Values)
			{
				Console.WriteLine(">>> List players: "+ p.uName);
				buffer.WriteString(p.uName);
				buffer.WriteString(p.cName);
				buffer.WriteString(p.playerIP);
				aux = DateTime.Now - p.currentPlaytime;
				buffer.WriteInt(aux.Hours*60+aux.Minutes); 
				//buffer.WriteInt(p.totalPlaytime+(aux.Hours * 60 + aux.Minutes));
			}
			Console.WriteLine("\n");
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

			SendDefaultRespose(true, clNo);
		}

		/// <summary>
		/// Send an default boolean response to http Server
		/// </summary>
		/// <param name="success">Bool if method failed or not</param>
		/// <param name="id">if of which connection from the http the bytearray has to be sent</param>
		private static void SendDefaultRespose(bool success, int id)
		{
			byte[] buffer = new byte[1];
			buffer = BitConverter.GetBytes(success);
			Globals.httpClient[id].GetStream().Write(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Remove all player reference from memory and closes the connection
		/// </summary>
		/// <param name="id">Player id</param>
		static void CloseConnection(int id)
		{
			ByteBuffer buffer = new ByteBuffer();
			buffer.WriteInt((int)Enums.AllEnums.SCloseConnection);
			buffer.WriteString(Globals.dicPlayers[id].uName);

			HandleClinet.SendToAllBut(id, buffer.ToArray());

			Globals.clients[id].Client.Close();
			Globals.clients[id] = null;
			Console.WriteLine(">> Closing connection from player " + Globals.dicPlayers[id].uName);
			DB.getInstance("","").UpdatePlayerPlayTime(Globals.dicPlayers[id]);
			try
			{
				Globals.dicPlayers.Remove(id);
			}
			catch (Exception) { }
			//SendDefaultRespose(true, clNo);
			
		}
	}
}