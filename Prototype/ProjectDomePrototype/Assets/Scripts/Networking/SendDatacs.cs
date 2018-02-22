using ByteBufferDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Networking
{
	class SendData
	{
		public static SendData instance;
		public Network network;
		
		// Use this for initialization
		void Awake()
		{
			instance = this;
		}

		
	}
}
