using ByteBufferDLL;
using Newtonsoft.Json;
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
			Console.WriteLine(sizeof(bool));
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
			//client.Connect("ip", "port");
		}

		public void Run()
		{

			httpListener.Start();
			Console.WriteLine(">> HTTP Server started ");
			var context = httpListener.GetContext(); // The contexts(request) has a field rawUrl and httpMethod that encapsulates the url and method(post,get...)
			string token = context.Request.Headers.GetValues("token")[0];

			if (token == "")
			{
				switch (context.Request.HttpMethod)
				{
					case "GET":
						{
							HandleGet(context, token);
							break;
						}
					case "POST":
						{
							HandlePost(context, token);
							break;
						}

				}
			}
			else
			{
				//var response = context.Response;
				const string responseString = "Token is invalid";
				byte[] buffer = Encoding.UTF8.GetBytes(responseString);
				context.Response.ContentLength64 = buffer.Length;
				context.Response.StatusCode = 403;
				Stream output = context.Response.OutputStream;
				output.Write(buffer, 0, buffer.Length);
				Console.WriteLine(output);
				output.Close();
				//Console.ReadKey();
			}

		}

		private void HandleGet(HttpListenerContext response, string token)
		{
			switch (response.Request.Url.LocalPath)
			{
				case "/listPlayers":
					{
						ListPlayers(response, token);
						break;
					}
				case "/restartServer":
					{
						ServerUpTime(response, token);
						break;
					}
				default:
					{
						String json = "{ error: \"Not found\"}";
						var buffer = Encoding.UTF8.GetBytes(json);
						response.Response.ContentLength64 = buffer.Length;
						response.Response.StatusCode = 404;
						var output = response.Response.OutputStream;
						output.Write(buffer, 0, buffer.Length);
						output.Flush();
						output.Close();
						break;
					}
			}
		}

		private void HandlePost(HttpListenerContext response, string token)
		{
			switch (response.Request.RawUrl)
			{
				case "/broadcastMessage":
					{
						ChangeSettings(response, token);
						break;
					}
				case "/kickPlayer":
					{
						KickPlayer(response, token);
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

		/***********HAS TEMP STUFF****************/
		private void ChangeSettings(HttpListenerContext response, string token)
		{
			var requestBody = response.Request.InputStream;
			byte[] data = new byte[4096];
			requestBody.Read(data, 0, (int)(response.Request.ContentLength64));
			string msg = Encoding.UTF8.GetString(data);
			ChangeSettingsJson csj = JsonConvert.DeserializeObject<ChangeSettingsJson>(msg);

			ByteBuffer buffer = new ByteBuffer();

			buffer.WriteInt(14); //mudar
			buffer.WriteString(csj.json_token);
			buffer.WriteInt(csj.concurrent_players);
			buffer.WriteInt(csj.port);

			SendToGameServer(buffer.ToArray());
		}

		/***********HAS TEMP STUFF****************/
		private void ListPlayers(HttpListenerContext response, string token)
		{
			List<byte> data = new List<byte>();
			data.Add(12);
			data.AddRange(Encoding.ASCII.GetBytes(token));
			
			SendToGameServer(data.ToArray());

			while (!client.GetStream().DataAvailable) { Thread.Sleep(50); }

			byte[] dataFromServer = new byte[4096];
			client.GetStream().Read(dataFromServer, 0, dataFromServer.Length);

			string json = ConvertToJson(dataFromServer);

			SendToClient(response,json,200);
		}

		/***********HAS TEMP STUFF****************/
		private void GetSettings(HttpListenerContext response, string token)
		{
			List<byte> buffer = new List<byte>();
			buffer.Add(10); //Mudar
			buffer.AddRange(Encoding.ASCII.GetBytes(token));
			SendToGameServer(buffer.ToArray());


			while (!client.GetStream().DataAvailable) { Thread.Sleep(50); }

			byte[] dataFromServer = new byte[4096];
			client.GetStream().Read(dataFromServer, 0, dataFromServer.Length);

			ByteBuffer bf = new ByteBuffer();
			bf.WriteBytes(dataFromServer);

			string json = "{ port:" + bf.ReadInt() + ", concurrent_players: " + bf.ReadInt() + "}";

			SendToClient(response, json, 200);
			
		}

		/***********HAS TEMP STUFF****************/
		private void KickPlayer(HttpListenerContext response, string token)
		{
			var requestBody = response.Request.InputStream;
			byte[] data = new byte[4096];
			requestBody.Read(data, 0, (int)(response.Request.ContentLength64));
			string json = Encoding.ASCII.GetString(data);
			KickPlayerJson p = JsonConvert.DeserializeObject<KickPlayerJson>(json);

			ByteBuffer buffer = new ByteBuffer();
			buffer.WriteInt(11); //*******TEMP********* change the ID later
			buffer.WriteString(p.json_token); //*******TEMP********* Deserialize json first
			buffer.WriteString(p.player_ID);
			buffer.WriteString(p.char_ID);
			SendToGameServer(buffer.ToArray());

			while (!client.GetStream().DataAvailable) { Thread.Sleep(50); }

			byte aux = (byte)client.GetStream().ReadByte();
			json = "{ success : " + Convert.ToBoolean(data) + " }";

			SendToClient(response, json, 200);

		}

		/***********HAS TEMP STUFF****************/
		private void RestartServer(HttpListenerContext response, string token)
		{
			List<byte> buffer = new List<byte>();
			buffer.Add(10); //Mudar
			buffer.AddRange(Encoding.ASCII.GetBytes(token));
			SendToGameServer(buffer.ToArray());

			while (!client.GetStream().DataAvailable) { Thread.Sleep(50); }

			byte data = (byte)client.GetStream().ReadByte();
			string json = "{ success : " + Convert.ToBoolean(data) + " }";

			SendToClient(response, json, 200);
		}

		private void SendToGameServer(byte[] data)
		{
			client.GetStream().Write(data, 0, data.Length);
			client.GetStream().Flush();
		}

		private string ConvertToJson(byte[] data)
		{
			List<PlayerJson> players = new List<PlayerJson>();
			ByteBuffer buffer = new ByteBuffer();
			buffer.WriteBytes(data);

			int numberPlayers = buffer.ReadInt();
			for (int i = 0; i < numberPlayers; i++)
			{
				PlayerJson p = new PlayerJson();
				p.username = buffer.ReadString();
				p.char_name = buffer.ReadString();
				p.player_ip = buffer.ReadString();
				p.current_playtime = buffer.ReadInt();
				p.total_playtime = buffer.ReadInt();

				players.Add(p);
			}

			return JsonConvert.SerializeObject(players);
		}

		private void SendToClient(HttpListenerContext response,string json,int httpcode)
		{
			byte[] buffer = Encoding.UTF8.GetBytes(json);

			response.Response.ContentLength64 = buffer.Length;
			response.Response.StatusCode = httpcode;

			var output = response.Response.OutputStream;
			output.Write(buffer, 0, buffer.Length);
			output.Flush();
		}

	}

	class PlayerJson
	{
		public string username { get; set; }
		public string char_name { get; set; }
		public string player_ip { get; set; }
		public int current_playtime { get; set; }
		public int total_playtime { get; set; }
	}

	class ChangeSettingsJson
	{
		public string json_token { get; set; }
		public int port { get; set; }
		public int concurrent_players { get; set; }
	}

	class KickPlayerJson
	{
		public string json_token { get; set; }
		public string player_ID { get; set; }
		public string char_ID { get; set; }
	}

	/*
	 *	Default response
	 *	{}
	 */

}