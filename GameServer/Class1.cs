using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using ByteBufferDLL;

namespace ConsoleApplication1
{
	class Globals
	{
		public static TcpClient[] clients = new TcpClient[20];
	}
	class Program
	{
		

		static void Main12(string[] args)
		{
			TcpListener serverSocket = new TcpListener(5500);
			TcpClient clientSocket = default(TcpClient);
			int counter = 0;

			serverSocket.Start();
			Console.WriteLine(" >> " + "Server Started");

			counter = 0;
			while (true)
			{
				for (int i = 0; i < 20; i++)
				{
					if (Globals.clients[i] == null)
					{
						counter += 1;
						Globals.clients[i] = new TcpClient();
						Globals.clients[i] = serverSocket.AcceptTcpClient();
						Console.WriteLine(" >> " + "Client No:" + Convert.ToString(counter) + " started! " + Globals.clients[i].Client.LocalEndPoint);
						handleClinet client = new handleClinet();
						client.startClient(Globals.clients[i], counter);
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
			Thread ctThread = new Thread(doChat);
			ctThread.Start();
		}
		private void doChat()
		{
			int requestCount = 0;
			byte[] bytesFrom = new byte[10025];
			string dataFromClient = null;
			Byte[] sendBytes = null;
			string serverResponse = null;
			string rCount = null;
			requestCount = 0;

			ByteBuffer buffer = new ByteBuffer();
			buffer.WriteInt(count);
			NetworkStream networkStream = clientSocket.GetStream();
			networkStream.Write(buffer.ToArray(), 0, buffer.ToArray().Length);

			count++;
			while ((true))
			{
				try
				{
					requestCount = requestCount + 1;
					networkStream = clientSocket.GetStream();

					if (networkStream.DataAvailable)
					{
						buffer = new ByteBuffer();
						networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
						buffer.WriteBytes(bytesFrom);
						int id = buffer.ReadInt();
						string msg = id + " : ";
						msg += buffer.ReadString();
						Console.WriteLine();
						//dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
						//dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
						Console.WriteLine(" >> " + "From client-" + clNo + msg);
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
	}
}