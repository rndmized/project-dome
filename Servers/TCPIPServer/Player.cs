using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerEcho
{
	public class Player
	{
		public string uName { get; set; } //User name
		public string cName { get; set; } //Char name
		public string playerIP { get; set; }

		public float cX { get; set; }
		public float cY { get; set; }
		public float cZ { get; set; }

		public int head { get; set; }
		public int body { get; set; }
		public int cloths { get; set; }

		public int socketID { get; set; }
		public int playtime { get; set; }
	}
}
