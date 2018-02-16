using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
	class ServerEnums
	{
		public enum ServerPackets
		{
			SAlertSyncGame = 1, 
			SPlayerMovement = 2
		}

		public enum ClientPackets
		{ }
	}
}
