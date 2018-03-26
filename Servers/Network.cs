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
	public int playerID = -1;
	public GameObject mainPlayer;
	public GameObject npc;
	Dictonary<int,GameObject> npcs = new Dictonary<GameObject> ();
	
	public static Network instance;

	string msg = "";
	private int frameCounter = 0;

	private void Awake()
	{
		instance = this;
	}
	// Use this for initialization
	void Start()
	{
		
		client.Connect(ip, port);
		if (client.Connected)
		{
			myStream = client.GetStream();
		}
	}
	/*private void OnGUI()
	{
		GUILayout.Label(msg);
	}*/
	

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
			if (packetnum == 0) //keepAlive
				return;

			HandleMessages(packetnum, buffer.ToArray());
		}

		if (frameCounter == 60)
		{
			frameCounter = 0;
			msg = "X: " + mainPlayer.transform.position.x + " Y: " + mainPlayer.transform.position.y + " Z: " + mainPlayer.transform.position.z;
			SendMovement(playerID, mainPlayer.transform.position.x, mainPlayer.transform.position.y, mainPlayer.transform.position.z);
		}
		frameCounter++;
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
		//THIS IS WHERE THE MAIN IS HANDLED
		int packetnum;
		ByteBuffer buffer = new ByteBuffer();
		buffer.WriteBytes(data);
		packetnum = buffer.ReadInt();
		playerID = buffer.ReadInt();
		String pName = buffer.getString();
		String head = buffer.getString();
		String body = buffer.getString();
		String legs = buffer.getString();
		
		//TODO 
		//Instantiate the player
	}

	public void HandleSSendingAlreadyConnectedToMain(int packetNum, byte[] data)
	{
		ByteBuffer buffer = new ByteBuffer();
		buffer.WriteBytes(data);
		packetNum = buffer.ReadInt();
		int PlayerIndex = buffer.ReadInt();
		Instantiate(npc,spawnpoint, new Quaternium(0,0,0,0);
		npcs.add(PlayerIndex,npc);
		/*playerID = PlayerIndex;
		/*Globals g = Globals.getIstance();
		g.MyIndex = PlayerIndex;*/
	}

	public void HandleSSendingMainToAlreadyConnected(int packetNum, byte[] data)
	{
		ByteBuffer buffer = new ByteBuffer();


		buffer.WriteBytes(data);
		int PlayerIndex = buffer.ReadInt();
		packetNum = buffer.ReadInt();

		//otherPlayer.name = Convert.ToString(PlayerIndex);
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
		
		GameObject temp = npcs[playerID];
		temp.GetComponent<NavMeshAgent>().setDestination(new Vector3(x, y, z));
		/*msg = "Player moved: " + x + " " + y + " " + z;
		Debug.Log(msg);*/
		//otherPlayer.transform.position =  ;
		//Globals.players[p].gameObject.transform.position = new Vector3(x, y, z);*/
	}

	public void SendMovement(int id, float x, float y, float z)
	{
		ByteBuffer buffer = new ByteBuffer();
		buffer.WriteInt((int)Assets.Scripts.Enums.AllEnums.SSyncingPlayerMovement);
		buffer.WriteInt(id);
		buffer.WriteFloat(x);
		buffer.WriteFloat(y);
		buffer.WriteFloat(z);
		myStream.Write(buffer.ToArray(), 0, buffer.ToArray().Length);
		myStream.Flush();
	}
	
	public void SendNewChar(String pID,String cName, int hair, int clothes, int body)
	{
		ByteBuffer buffer = new ByteBuffer();
		buffer.write(pID);
	}
}
