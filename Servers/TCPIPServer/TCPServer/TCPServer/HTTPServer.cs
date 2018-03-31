using ByteBufferDLL;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace TCPServer
{

	class Maain
	{
		public static void Main(string[] args)
		{
			HttpServer a = new HttpServer(8080);
			a.Run();
		}
	}
	class HttpServer
	{
		private HttpListener httpListener;
		//private BlockingCollection<List<String>> inQueue; //
		private TcpClient client = new TcpClient();
		public HttpServer(int port)//, ref ArrayList job)
		{
			httpListener = new HttpListener();
			httpListener.Prefixes.Add("http://localhost:" + port + "/");
			//jobs = job;
			client.Connect("ip", "port");
		}

		public void Run()
		{

			httpListener.Start();
			Console.WriteLine(">> HTTP Server started ");
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
						ListPlayers(response);
						break;
					}
				case "/ServerUpTime":
					{
						ServerUpTime(response);
						break;
					}
				default:
					{
						String json = "{ error: \"Not found\"}";
						var buffer = Encoding.UTF8.GetBytes(json);
						var output = response.Response.OutputStream;
						output.Write(buffer, 0, buffer.Length);
						output.Flush();
						output.Close();
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
						BroadcastMessage(response);
						break;
					}
				case "/kickPlayer":
					{
						KickPlayer(response);
						break;
					}
				default:
					{
						String json = "{ error: \"Not found\"}";
						var buffer = Encoding.UTF8.GetBytes(json);
						var output = response.Response.OutputStream;
						output.Write(buffer, 0, buffer.Length);
						output.Flush();
						output.Close();
						break;
					}
			}
		}

		/***********HAS TEMP STUFF**************/
		private void BroadcastMessage(HttpListenerContext response)
		{
			var requestBody = response.Request.InputStream;
			byte[] data = new byte[4096];
			requestBody.Read(data, 0, (int)(response.Request.ContentLength64));
			string msg = Encoding.UTF8.GetString(data);

			ByteBuffer buffer = new ByteBuffer();

			buffer.WriteInt(14); //TEMP
			buffer.WriteString(msg);

			Send(buffer.ToArray());
		}

		/***********HAS TEMP STUFF**************/
		private void ListPlayers(HttpListenerContext response)
		{
			byte[] data = new byte[4];
			data = BitConverter.GetBytes(12); //TEMP

			Send(data);

			while (!client.GetStream().DataAvailable) { Thread.Sleep(50); }

			byte[] dataFromServer = new byte[client.Available]; 
			client.GetStream().Read(dataFromServer, 0, dataFromServer.Length);

			var output = response.Response.OutputStream;
			output.Write(dataFromServer, 0, dataFromServer.Length); //if I create the json on server side, this is fine
			output.Flush();
			output.Close();
		}

		/***********HAS TEMP STUFF**************/
		private void ServerUpTime(HttpListenerContext response)
		{
			byte[] data = new byte[4];
			data = BitConverter.GetBytes(10); //TEMP

			Send(data);

			while (!client.GetStream().DataAvailable) { Thread.Sleep(50); }

			byte[] dataFromServer = new byte[client.Available];
			client.GetStream().Read(dataFromServer, 0, dataFromServer.Length); 

			//If I create the json on server side, I can just send the byte array direnctly to client

			/*ByteBuffer buffer = new ByteBuffer();

			buffer.WriteBytes(dataFromServer); */

			var output = response.Response.OutputStream; 
			output.Write(dataFromServer, 0, dataFromServer.Length); 
			output.Flush();
			output.Close();
		}

		/***********HAS TEMP STUFF**************/
		private void KickPlayer(HttpListenerContext response)
		{
			var requestBody = response.Request.InputStream;
			byte[] data = new byte[4096];
			requestBody.Read(data, 0, (int)(response.Request.ContentLength64));
			string json = Encoding.UTF8.GetString(data);

			ByteBuffer buffer = new ByteBuffer();
			buffer.WriteInt(11); //*******TEMP********* change the ID later
			buffer.WriteString(json); //*******TEMP********* Deserialize json first

			Send(buffer.ToArray());
			//Send

			/*while (!client.GetStream().DataAvailable) { }

			byte[] dataA = new byte[client.Available];
			client.GetStream().Read(dataA, 0, dataA.Length);

			ByteBuffer buffer = new ByteBuffer();

			buffer.WriteBytes(dataA);*/

		}

		private void Send(byte[] data)
		{
			client.GetStream().Write(data, 0, data.Length);
			client.GetStream().Flush();
		}
	}
}
