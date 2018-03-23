using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using ByteBufferDLL;
using EnumsServer;
using System.Net;
using System.Collections;
using System.IO;

namespace ServerEcho
{
	class Globals
	{
		public static TcpClient[] clients = new TcpClient[20];
	}
	class Server2
	{
		static void Main(string[] args)
		{
			TcpListener serverSocket = new TcpListener(IPAddress.Any,5500);
			TcpClient clientSocket = default(TcpClient);
			HttpListener http = new HttpListener();
			http.
			int counter = 0;

			serverSocket.Start();
			Console.WriteLine(" >> " + "Echo Server 2 Started");

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

			clientSocket.Close();
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
			NetworkStream networkStream;
			
			ByteBuffer buffer = new ByteBuffer();
			buffer.WriteInt((int)Enums.AllEnums.SSendingPlayerID);
			buffer.WriteInt(clNo);
			networkStream = clientSocket.GetStream();
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

	class HttpServer
	{
		private HttpListener httpListener;
		private ArrayList jobs;
		public HttpServer(int port, ref ArrayList job)
		{
			httpListener = new HttpListener();
			httpListener.Prefixes.Add("http://localhost:"+port+"/");
			jobs = job;
		}

		public void Run()
		{
			httpListener.Start();
			var context = httpListener.GetContext(); // The contexts(request) has a field rawUrl and httpMethod that encapsulates the url and method(post,get...)

			switch (context.Request.HttpMethod)
			{
				case "GET":
					{
						HandleGet(context);
						break;
					}
				case "POST":
					{
						HandlePost(context);
						break;
					}
			}
			/*var response = context.Response;
			const string responseString = "<html><body>Hello world</body></html>";
			var buffer = Encoding.UTF8.GetBytes(responseString);
			response.ContentLength64 = buffer.Length;
			var output = response.OutputStream;
			output.Write(buffer, 0, buffer.Length);

			Console.WriteLine(output);

			output.Close();

			//Console.ReadKey();*/
		}

		private void HandleGet(HttpListenerContext response)
		{
			switch (response.Request.RawUrl)
			{
				case "/listPlayers":
					{
						break;
					}
				case "/toDo":
					{
						break;
					}
			}
		}

		private void HandlePost(HttpListenerContext response)
		{
			switch (response.Request.RawUrl)
			{
				case "/broadcastMessage":
					{
						Stream body = response.Request.InputStream;
						body.
						break;
					}
				case "/changePlayerScore":
					{
						break;
					}
			}
		}

		private void BroadcastMessage(String msg)
		{
			jobs.Add(msg);
		}
	}

}