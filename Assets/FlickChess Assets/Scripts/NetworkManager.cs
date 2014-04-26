using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {

	public UIManager uiManager;
	public ChatLog chatLog;

	private string gameName;
	private bool refreshing;
	private HostData[] hostData;
	private List<NetworkPlayerInfo> playerConnectedSlots;
	private int maxPlayers;

	// Use this for initialization
	void Start () {
		gameName = "FlickChess3D Test";
		refreshing = false;
		maxPlayers = 32;
		hostData = null;
		playerConnectedSlots = new List<NetworkPlayerInfo>();
	}
	
	// Update is called once per frame
	void Update () {
		if(refreshing)
		{
			if(MasterServer.PollHostList().Length > 0)
			{
				refreshing = false;
				hostData = MasterServer.PollHostList();
			}
		}
	}

	void startServer()
	{
		bool useNat = !Network.HavePublicAddress();
		Network.InitializeServer(16, 25001, useNat);
		Debug.Log("UseNat: " + useNat);
		MasterServer.RegisterHost(gameName, "Network Test", "Network Testing");
	}

	void refreshHostList()
	{
		MasterServer.RequestHostList(gameName);
		refreshing = true;
	}

	void onConnected()
	{
		networkView.RPC("setPlayerInfo", RPCMode.Server, uiManager.playerName);
	}

	void OnServerInitialized()
	{
		Debug.Log("Server Initialized");
		uiManager.networkView.RPC("startNewGameNetworkedUI", RPCMode.AllBuffered);
		playerConnectedSlots.Add(new NetworkPlayerInfo(Network.player, uiManager.playerName));
	}

	void OnConnectedToServer() 
	{
		Debug.Log("Connected To Server");
		onConnected();
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
		//chatLog.networkView.RPC("addMessage", RPCMode.All, player.ipAddress + " Connected");
		//uiManager.dragRigidbodyNetwork.createSpringJoint)
	}

	[RPC]
	void setPlayerInfo(string playerName, NetworkMessageInfo messageInfo)
	{
		playerConnectedSlots.Add(new NetworkPlayerInfo(messageInfo.sender, playerName));
		uiManager.networkView.RPC("setPlayerNumber", messageInfo.sender);
		chatLog.networkView.RPC("addMessage", RPCMode.All, playerName + " (" + messageInfo.sender.ipAddress + ") Connected (" + playerConnectedSlots.Count + "/" + maxPlayers + ").");
	}

	void OnPlayerDisconnected(NetworkPlayer player)
	{
		foreach(NetworkPlayerInfo playerInfo in playerConnectedSlots)
		{
			if(playerInfo.m_networkPlayer == player)
			{
				chatLog.networkView.RPC("addMessage", RPCMode.All, "User " + playerInfo.m_playerName + " (" + player.ipAddress + ") disconnected.");
				break;
			}
		}
	}

	void OnMasterServerEvent(MasterServerEvent mse)
	{
		if(mse == MasterServerEvent.RegistrationSucceeded)
		{
			Debug.Log("Registration Succeeded");
		}
	}

	void OnGUI () 
	{
		//very temp
		if(!Network.isClient && !Network.isServer)
		{
			if(GUI.Button(new Rect(100,100,100,100), "Start Server"))
			{
				Debug.Log("Starting Server");
				startServer();
			}

			if(GUI.Button(new Rect(100,220,100,100), "Refresh Host"))
			{
				Debug.Log("Refreshing Server");	
				refreshHostList();
			}
			if(hostData != null)
			{
				for(int i = 0; i<hostData.Length; i++)
				{
					if(GUI.Button(new Rect(220, 100 + i*55, 100, 50), hostData[i].gameName))
					{
						Network.Connect(hostData[i]);
					}
				}
			}
		}
	}
}