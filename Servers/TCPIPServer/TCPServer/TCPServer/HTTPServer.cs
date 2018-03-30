using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
		private BlockingCollection<List<String>> inQueue; //
		public HttpServer(int port)//, ref ArrayList job)
		{
			httpListener = new HttpListener();
			httpListener.Prefixes.Add("http://localhost:" + port + "/");
			//jobs = job;
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
						Stream body = response.Request.InputStream;
						//body.
						break;
					}
				case "/kickPlayer":
					{
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

		private void BroadcastMessage(String msg)
		{
			//jobs.Add(msg);
		}

		private void ListPlayers(HttpListenerContext response)
		{
			String json = "";
			var buffer = Encoding.UTF8.GetBytes(json);
			// Comunicate with server and get the list of players
			var output = response.Response.OutputStream;
			output.Write(buffer, 0, buffer.Length);
			output.Flush();
			output.Close();
		}

		private void ServerUpTime(HttpListenerContext response)
		{

			// Comunicate with server and get the server up time

			String json = "";
			var buffer = Encoding.UTF8.GetBytes(json);

			var output = response.Response.OutputStream;
			output.Write(buffer, 0, buffer.Length);
			output.Flush();
			output.Close();
		}

		private void KickPlayer(HttpListenerContext response)
		{
			var a = response.Request.InputStream;
			byte[] data = new byte[4096];
			a.Read(data, 0, (int)(response.Request.ContentLength64));
			string json = Encoding.UTF8.GetString(data);
			//Deserialize json and send message to server
		}
	}
}
