using ByteBufferDLL;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameServer
{
    class NetworkHandleData
    {
        private delegate void Packet_(int Index, byte[] Data);
        private Dictionary<int, Packet_> Packets;

        public void InitMessages()
        {
            Packets = new Dictionary<int, Packet_>();
        }

        public void HandleData(int index, byte[] data)
        {
			Console.WriteLine("Entrou id "+index);
			Debug.WriteLine(data);
            int packetnum;
            Packet_ Packet;
			ByteBuffer buffer = new ByteBuffer();
			buffer.WriteBytes(data);
            packetnum = buffer.ReadInt();
            //buffer = null;

            if (packetnum == 0)
                return;

			switch (packetnum)
			{
				case (int)ServerEnums.ServerPackets.SPlayerMovement:
					/*string tst = buffer.ReadString(); 
					Debug.WriteLine(tst);*/
					HandlePlayerMovement((int)ServerEnums.ServerPackets.SPlayerMovement, buffer.ReadInt(), buffer.ToArray());
					break;
				case (int)ServerEnums.ServerPackets.SAlertSyncGame:
					break;
				default:
					return;
			}

            if (Packets.TryGetValue(packetnum, out Packet))
            {
                Packet.Invoke(index, data);
            }

        }

		public void HandlePlayerMovement(int option, int playerID, byte[] data) 
		{
			ByteBuffer buffer = new ByteBuffer();

			buffer.WriteInt(option);
			buffer.WriteInt(playerID);
			buffer.WriteBytes(data);

			Globals.networkSendData.SendDataToAllBut(playerID, buffer.ToArray());
		}

	}
}
