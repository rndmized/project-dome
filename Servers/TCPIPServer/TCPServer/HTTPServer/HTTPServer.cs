using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HTTPServer
{
	class HttpServer
	{
		private HttpListener httpListener;
		private ArrayList jobs;
		public HttpServer(int port, ref ArrayList job)
		{
			httpListener = new HttpListener();
			httpListener.Prefixes.Add("http://localhost:" + port + "/");
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
						//body.
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

	public static class JwtTokens
	{
		private static string key;

		public static string Decypher(String text)
		{
			String tst;
			try
			{
				tst = Jose.JWT.Decode(text, Encoding.ASCII.GetBytes(key));
			}
			catch (Exception)
			{
				return null;
			}

			return tst;
		}

		public static void LoadKey(string path)
		{
			StreamReader reader = new StreamReader(path);
			key = reader.ReadLine();
			reader.Close();
		}

	}
}

