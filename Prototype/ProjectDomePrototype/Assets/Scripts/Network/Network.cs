using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using ByteBufferDLL;
using System;

public class Network : MonoBehaviour
{
	TcpClient client = new TcpClient();
	const string ip = "127.0.0.1";
	const int port = 5500;
	const int buffersize = 4096;
	public NetworkStream myStream;
	byte[] inBuffer = new byte[4096];
	public static int playerID = -1;
	public GameObject mainPlayer;
	public GameObject otherPlayer;
	// Use this for initialization
	void Start()
	{
		client.Connect(ip, port);
		if (client.Connected)
			myStream = client.GetStream();
	}

	// Update is called once per frame
	void Update()
	{
		if (myStream.DataAvailable)
		{
			myStream.Read(inBuffer, 0, buffersize);
			int packetnum;
			ByteBuffer buffer = new ByteBuffer();
			buffer.WriteBytes(inBuffer);
			packetnum = buffer.ReadInt();
			buffer = null;
			if (packetnum == 0)
				return;

			HandleMessages(packetnum, buffer.ToArray());
		}
	}

	public void HandleMessages(int packetNum, byte[] data)
	{
		switch (packetNum)
		{
			case (int)Assets.Scripts.Enums.AllEnums.SSendingPlayerID:
				HandleSSendingPlayerID(packetNum, data); //
				break;
			case (int)Assets.Scripts.Enums.AllEnums.SSendingAlreadyConnectedToMain:
				HandleSSendingAlreadyConnectedToMain(packetNum, data);
				break;
			case (int)Assets.Scripts.Enums.AllEnums.SSendingMainToAlreadyConnected:
				HandleSSendingMainToAlreadyConnected(packetNum, data); //instantiating others to main player
				break;
			case (int)Assets.Scripts.Enums.AllEnums.SSyncingPlayerMovement:
				HandleSSyncingPlayerMovement(data);
				break;
		}
	}

	void HandleSSendingPlayerID(int packetNum, byte[] data)
	{
		int packetnum;
		ByteBuffer buffer = new ByteBuffer();
		buffer.WriteBytes(data);
		packetnum = buffer.ReadInt();
		int MyIndex = buffer.ReadInt();
		mainPlayer.name = Convert.ToString(MyIndex);
		
	}

	public void HandleSSendingAlreadyConnectedToMain(int packetNum, byte[] data)
	{
		ByteBuffer buffer = new ByteBuffer();
		buffer.WriteBytes(data);
		packetNum = buffer.ReadInt();
		int PlayerIndex = buffer.ReadInt();
		otherPlayer.name = Convert.ToString(PlayerIndex);
		playerID = PlayerIndex;
		/*Globals g = Globals.getIstance();
		g.MyIndex = PlayerIndex;*/
	}

	public void HandleSSendingMainToAlreadyConnected(int packetNum, byte[] data)
	{
		ByteBuffer buffer = new ByteBuffer();


		buffer.WriteBytes(data);
		int PlayerIndex = buffer.ReadInt();
		packetNum = buffer.ReadInt();

		otherPlayer.name = Convert.ToString(PlayerIndex);
	}

	public void HandleSSyncingPlayerMovement(byte[] data)
	{
		ByteBuffer buffer = new ByteBuffer();
		buffer.WriteBytes(data); // contains playerID, X,Y,Z in this other
		int playerID = buffer.ReadInt();
		int p = buffer.ReadInt();
		float x, y, z = 0;
		x = buffer.ReadFloat();
		y = buffer.ReadFloat();
		z = buffer.ReadFloat();
		/*msg = "Player moved: " + x + " " + y + " " + z;
		Debug.Log(msg);*/
		otherPlayer.transform.position = new Vector3(x, y, z); ;
		//Globals.players[p].gameObject.transform.position = new Vector3(x, y, z);*/
	}

	public void SendMovement()
	{

	}
}
