using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPServer
{
	class tst
	{
		public static TcpListener serverSocket;
		public CancellationTokenSource cts = new CancellationTokenSource();

		public tst()
		{
			serverSocket = new TcpListener(8080);
		}

		static void Maain(String[] args)
		{
			tst t = new tst();
			Task task = Task.Run(() =>
			{
				Console.WriteLine("Enter something: ");
				Console.ReadLine();
				t.cts.Cancel();
			});
			try
			{
				t.a();
			}
			catch (Exception) { Console.WriteLine("ex"); }
			Console.WriteLine("Terminou");
			Console.ReadKey();
		}

		public void a()
		{
			serverSocket.Start();
			serverSocket.AcceptTcpClient();
			if (cts.IsCancellationRequested)
			{ Console.WriteLine("Worked"); }
		}
	}
}
