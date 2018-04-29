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
			HttpServer a = new HttpServer(8080);
			a.Run();
		}
	}
	class HttpServer
	{
		private HttpListener httpListener;
		private TcpClient client = new TcpClient();
		public HttpServer(int port)
		{
			httpListener = new HttpListener();
			httpListener.Prefixes.Add("http://127.0.0.1:" + port + "/");

			client.Connect("", 5000);
		}

		public void Run()
		{

			httpListener.Start();
			Console.WriteLine(">> HTTP Server started ");

			while (true)
			{
				var context = httpListener.GetContext(); // The contexts(request) has a field rawUrl and httpMethod that encapsulates the url and method(post,get...)
				if (context.Request.HttpMethod == "OPTIONS")
				{
					SendToClient(context, "", 200);
					continue;
				}
				string token = context.Request.Headers["authorization"];
				if (true) //validate token
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
					SendToClient(context, "{ error: \"Authentication failed\"}", 404);
				}
			}

		}

		private void HandleGet(HttpListenerContext response, string token)
		{
			switch (response.Request.Url.LocalPath)
			{
				case "/listPlayers":
					{
						Console.WriteLine(">> Received listPlayers request");
						ListPlayers(response, token);
						break;
					}
				case "/restartServer":
					{
						Console.WriteLine(">> Received restartServer request");
						RestartServer(response, token);
						break;
					}
				case "/getSettings":
					{
						Console.WriteLine(">> Received getSettings request");
						GetSettings(response, token);
						break;
					}
				default:
					{
						string json = "{ error: \"Not found\"}";
						SendToClient(response, json, 404);
						break;
					}
			}
		}

		private void HandlePost(HttpListenerContext response, string token)
		{
			switch (response.Request.RawUrl)
			{
				case "/changeSettings":
					{
						Console.WriteLine(">> Received changeSettings request");
						ChangeSettings(response, token);
						break;
					}
				case "/kickPlayer":
					{
						Console.WriteLine(">> Received kickPlayer request");
						KickPlayer(response, token);
						break;
					}
				default:
					{
						SendToClient(response, "{ error: \"Not found\"}", 404);
						break;
					}
			}
		}

		/// <summary>
		/// Sends new configuration to game server
		/// </summary>
		/// <param name="response">Underlaying in and out streams</param>
		/// <param name="token">Token for authentication</param>
		private void ChangeSettings(HttpListenerContext response, string token)
		{
			var requestBody = response.Request.InputStream;
			byte[] data = new byte[4096];
			requestBody.Read(data, 0, (int)(response.Request.ContentLength64));
			string msg = Encoding.UTF8.GetString(data);
			ChangeSettingsJson csj = JsonConvert.DeserializeObject<ChangeSettingsJson>(msg);

			ByteBuffer buffer = new ByteBuffer();

			buffer.WriteByte((int)EnumsServer.Enums.AllEnums.HChangeSettings); //mudar
			buffer.WriteInt(csj.concurrent_players);
			buffer.WriteInt(csj.port);
			buffer.WriteByte(Convert.ToByte(csj.restart));
			SendToGameServer(buffer.ToArray());

			while (!client.GetStream().DataAvailable) { Thread.Sleep(50); }

			client.GetStream().Read(data, 0, 1);

			SendToClient(response, "{\"success\":" + Convert.ToBoolean(data[0]) + "}",200);
		}

		private void ListPlayers(HttpListenerContext response, string token)
		{
			List<byte> data = new List<byte>();
			data.Add((int)EnumsServer.Enums.AllEnums.HListPlayers);
			data.AddRange(BitConverter.GetBytes(Encoding.ASCII.GetBytes(token).Length));
			data.AddRange(Encoding.ASCII.GetBytes(token));
			
			SendToGameServer(data.ToArray());

			//response:
			while (!client.GetStream().DataAvailable) { Thread.Sleep(50); }

			byte[] dataFromServer = new byte[4096];
			client.GetStream().Read(dataFromServer, 0, dataFromServer.Length);
			//if (dataFromServer[0] == 1 && dataFromServer[4]==0) goto response;
			string json = ConvertToJson(dataFromServer);

			SendToClient(response,json,200);
		}

		/// <summary>
		/// Gets game server current configuration
		/// </summary>
		/// <param name="response">Underlaying in and out streams</param>
		/// <param name="token">Token for authentication</param>
		private void GetSettings(HttpListenerContext response, string token)
		{
			List<byte> buffer = new List<byte>();
			buffer.Add((int)EnumsServer.Enums.AllEnums.HGetSettings); //Mudar
			buffer.AddRange(BitConverter.GetBytes(Encoding.ASCII.GetBytes(token).Length));
			buffer.AddRange(Encoding.ASCII.GetBytes(token));
			SendToGameServer(buffer.ToArray());


			while (!client.GetStream().DataAvailable) { Thread.Sleep(50); }

			byte[] dataFromServer = new byte[4096];
			client.GetStream().Read(dataFromServer, 0, dataFromServer.Length);

			ByteBuffer bf = new ByteBuffer();
			bf.WriteBytes(dataFromServer);

			string json = "{\"port\":" + bf.ReadInt() + ", \"concurrent_players\":" + bf.ReadInt() + "}";

			SendToClient(response, json, 200);
			
		}

		private void KickPlayer(HttpListenerContext response, string token)
		{
			var requestBody = response.Request.InputStream;
			byte[] data = new byte[4096];
			requestBody.Read(data, 0, (int)(response.Request.ContentLength64));
			string json = Encoding.ASCII.GetString(data);
			KickPlayerJson p = JsonConvert.DeserializeObject<KickPlayerJson>(json);

			ByteBuffer buffer = new ByteBuffer();
			buffer.WriteByte((int)EnumsServer.Enums.AllEnums.HKickPlayer); 
			buffer.WriteString(token); 
			buffer.WriteString(p.player_ID);
			buffer.WriteString(p.char_ID);
			SendToGameServer(buffer.ToArray());

			/*while (!client.GetStream().DataAvailable) { Thread.Sleep(50); }

			byte aux = (byte)client.GetStream().ReadByte();*/
			byte aux = 1;
			if (aux == 1)	
				json = "{ \"success\" : true }";
			else
				json = "{ \"success\" : false }";

			SendToClient(response, json, 200);

		}

		private void RestartServer(HttpListenerContext response, string token)
		{
			List<byte> buffer = new List<byte>();
			buffer.Add((int)EnumsServer.Enums.AllEnums.HRestartServer); //Same as writting a short
			buffer.AddRange(BitConverter.GetBytes(Encoding.ASCII.GetBytes(token).Length));
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
			if (numberPlayers == 0) return "";
			for (int i = 0; i < numberPlayers; i++)
			{
				PlayerJson p = new PlayerJson();
				p.username = buffer.ReadString();
				p.char_name = buffer.ReadString();
				p.player_IP = buffer.ReadString();
				p.current_playtime = buffer.ReadInt();
				//p.total_playtime = buffer.ReadInt();

				players.Add(p);
			}

			return JsonConvert.SerializeObject(players);
		}

		private void SendToClient(HttpListenerContext response, string json, int httpcode)
		{
			byte[] buffer = Encoding.UTF8.GetBytes(json);

			response.Response.ContentLength64 = buffer.Length;
			response.Response.StatusCode = httpcode;
			response.Response.AddHeader("Content-Type", "application/json");
			response.Response.ContentType = "application/json";
			response.Response.AddHeader("Access-Control-Allow-Origin", "*");
			response.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With, authorization");
			response.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
			response.Response.ContentType = "application/json";
			var output = response.Response.OutputStream;
			output.Write(buffer, 0, buffer.Length);
			output.Flush();
		}
	}

	class PlayerJson
	{
		public string username { get; set; }
		public string char_name { get; set; }
		public string player_IP { get; set; }
		public int current_playtime { get; set; }
		public int total_playtime { get; set; }
	}

	class ChangeSettingsJson
	{
		public int port { get; set; }
		public int concurrent_players { get; set; }
		public bool restart { get; set; }
	}

	class KickPlayerJson
	{
		public string player_ID { get; set; }
		public string char_ID { get; set; }
	}

	/*
	 *	Default response
	 *	{}
	 */

}