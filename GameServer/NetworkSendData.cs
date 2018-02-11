using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ByteBufferDLL;

namespace GameServer
{
    class NetworkSendData
    {
        public void SendDataTo(int index, byte[] data)
        {
			ByteBuffer buffer = new ByteBuffer();
			buffer.WriteBytes(data);
			try
			{
				Globals.Clients[index].myStream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
			}
			catch (Exception e) { }
            buffer = null;
        }

        public async void SendDataToAll(byte[]data)
        {
            for(int i = 1; i < Constants.MAX_PLAYERS; i++)
            {
                if(Globals.Clients[i].Socket != null)
                {
                    await Task.Delay(1000);
                    SendDataTo(i, data);
                }
            }
        }

        public void SendDataToAllBut(int index, byte[] data)
        {
            for (int i = 1; i < Constants.MAX_PLAYERS; i++)
            {
                if (Globals.Clients[i].Socket != null)
                {
                    if (i != index)
                    {
                        SendDataTo(i, data);
                    }
                }
            }
        }

        public void SendJoinGame(int index) //Instantiates main player and notify others
        {
			ByteBuffer buffer = new ByteBuffer(); 
			buffer.WriteInt(2); //change this to enum
			buffer.WriteInt(index); //Sending packet to instantiate main player
			SendDataTo(index, buffer.ToArray());
            //Sending main player to other players
            SendInstantiatePlayer(index);
            //Sends other players to the main player
            SendOtherToPlayer(index);

        }

        public async void SendInstantiatePlayer(int index) //Sends the player to the others
		{
            ByteBuffer buffer = new ByteBuffer();
			
			for (int i = 1; i < Constants.MAX_PLAYERS; i++)
            {
                if (Globals.Clients[i].Socket != null)
                {
                    if (i != index)
                    {
						buffer.WriteInt(1); //change this to enum
						buffer.WriteInt(index);
						await Task.Delay(1000);
						SendDataTo(i, buffer.ToArray());

						/*buffer.WriteInteger(index);
						await Task.Delay(1000);
						SendDataTo(index, buffer.ToArray());*/
					}
                }
            }


		}

        public async void SendOtherToPlayer(int index)
        {
			ByteBuffer buffer = new ByteBuffer();
			buffer.WriteInt(3);

            for (int i = 1; i < Constants.MAX_PLAYERS; i++)
            {
                if (Globals.Clients[i].Socket != null)
                {
                    if(i != index)
                    {
						buffer.WriteInt(1);
						buffer.WriteInt(i);
                        await Task.Delay(1000);
                        SendDataTo(index, buffer.ToArray());
                    }
                }
            }
        }

		public void SendIngame(int index)
		{

		}
	}
}
