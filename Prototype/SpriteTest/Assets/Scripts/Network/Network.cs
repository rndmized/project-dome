using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using ByteBufferDLL;
using System;
using UnityEngine.AI;
//using UnityEditor.Animations;

public class Network : MonoBehaviour
{
	TcpClient client = new TcpClient();
	const string ip = "127.0.0.1";
	const int port = 5500;
	const int buffersize = 4096;
	public NetworkStream myStream;
	byte[] inBuffer = new byte[4096];
	//public string playerID = "";
	public GameObject NPC;
	public GameObject mainPlayer;
	public GameObject spawnPoint;
	Dictionary<string, GameObject> tst = new Dictionary<string, GameObject>();
	//public GameObject npc;
	//Dictonary<int,GameObject> npcs = new Dictonary<GameObject> ();

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
			/*ByteBuffer buffer = new ByteBuffer();
			buffer.WriteString(PlayerProfile.token);
			buffer.WriteString(PlayerProfile.uID);
			buffer.WriteString(PlayerProfile.cID);
			buffer.WriteFloat();
			buffer.WriteFloat();
			buffer.WriteFloat();*/
		}
	}
	private void OnGUI()
	{
		GUILayout.Label(msg);
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
			if (packetnum == 0) //keepAlive
				return;

			HandleMessages(packetnum, buffer.ToArray());
		}

		if (frameCounter == 60)
		{
			frameCounter = 0;
			//msg = "X: " + mainPlayer.transform.position.x + " Y: " + mainPlayer.transform.position.y + " Z: " + mainPlayer.transform.position.z;
			SendMovement(PlayerProfile.uID, mainPlayer.transform.position.x, mainPlayer.transform.position.y, mainPlayer.transform.position.z);
		}
		frameCounter++;
	}

	public void HandleMessages(int packetNum, byte[] data)
	{
		switch (packetNum)
		{
			case (int)Assets.Scripts.Enums.AllEnums.SSendingPlayerID:
				HandlePlayerPackage(data, true); //
				break;
			case (int)Assets.Scripts.Enums.AllEnums.SSendingAlreadyConnectedToMain:
				HandlePlayerPackage(data, false);
				break;
			case (int)Assets.Scripts.Enums.AllEnums.SSendingMainToAlreadyConnected:
				HandlePlayerPackage(data, false); 
				break;
			case (int)Assets.Scripts.Enums.AllEnums.SSyncingPlayerMovement:
				HandleSSyncingPlayerMovement(data);
				break;
		}
	}

	private void HandlePlayerPackage(byte[] data, bool isMainPlayer)
	{
		ByteBuffer buffer = new ByteBuffer();
		buffer.WriteBytes(data);
		buffer.ReadInt();
		String uName = buffer.ReadString();
		String cName = buffer.ReadString();
		int hair = buffer.ReadInt();
		int body = buffer.ReadInt();
		int clothes = buffer.ReadInt();

		InstantiatePlayer(uName,cName,clothes, hair, body, isMainPlayer);

	}

	public void HandleSSyncingPlayerMovement(byte[] data)
	{
		
		ByteBuffer buffer = new ByteBuffer();
		buffer.WriteBytes(data); // contains playerID, X,Y,Z in this other
		buffer.ReadInt();
		string playerID = buffer.ReadString();
		Debug.Log("*********entrou******"+playerID);
		float x, y, z = 0;
		x = buffer.ReadFloat();
		y = buffer.ReadFloat();
		z = buffer.ReadFloat();

		tst[playerID].GetComponent<NavMeshAgent>().SetDestination(new Vector3(x, y, z));

	}


	public void SendMovement(string id, float x, float y, float z)
	{
		ByteBuffer buffer = new ByteBuffer();
		buffer.WriteInt((int)Assets.Scripts.Enums.AllEnums.SSyncingPlayerMovement);
		buffer.WriteString(id);
		buffer.WriteFloat(x);
		buffer.WriteFloat(y);
		buffer.WriteFloat(z);
		myStream.Write(buffer.ToArray(), 0, buffer.ToArray().Length);
		myStream.Flush();
	}

	public void InstantiatePlayer(string uName, string cName,int clothes, int hair, int body, bool isMainPlayer)
	{
		Character NPC_Char;
		List<RuntimeAnimatorController> HairStyles;
		List<RuntimeAnimatorController> ClothesStyles;
		List<RuntimeAnimatorController> BodyStyle;

		NPC_Char = ScriptableObject.CreateInstance<Character>();
		HairStyles = FindObjectOfType<AssetList>().GetComponent<AssetList>().HairStyles;
		ClothesStyles = FindObjectOfType<AssetList>().GetComponent<AssetList>().ClothesStyles;
		BodyStyle = FindObjectOfType<AssetList>().GetComponent<AssetList>().BodyStyle;

		NPC_Char.ID = uName;
		NPC_Char.name = cName;
		NPC_Char = ScriptableObject.CreateInstance<Character>();
		NPC_Char.char_clothesAnimator = ClothesStyles[clothes];
		NPC_Char.char_headAnimator = HairStyles[hair];
		NPC_Char.char_bodyAnimator = BodyStyle[body];

		switch (isMainPlayer)
		{
			case true:
				{
					PlayerProfile.uID = uName;
					mainPlayer.GetComponent<CharacterRenderer>().character = NPC_Char;
					Instantiate(mainPlayer, transform.TransformPoint(0, 0, 0), new Quaternion(0, 0, 0, 0));
					Camera.main.GetComponent<CameraController>().character = mainPlayer.transform;
					break;
				}
			case false:
				{
					tst.Add(uName, NPC);
					msg = uName;
					NPC.GetComponent<CharacterRenderer>().character = NPC_Char;
					Instantiate(NPC, transform.TransformPoint(0, 0, 0), new Quaternion(0, 0, 0, 0));
					break;
				}
		}
	}

}
