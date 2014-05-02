using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

	public GameManager gameManager;
	public NetworkManager networkManager;

	public string playerName;

	// Use this for initialization
	void Start () {
		playerName = "samdamana";
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		playerName = GUI.TextField(new Rect(60,60,300,30), playerName, 32);
		if(GUI.Button(new Rect(Screen.width-60, 0, 60, 60), "New Game"))
		{
			if(!Network.isClient && !Network.isServer)
				startNewGameLocalUI();
			else
				networkView.RPC("startNewGameNetworkedUI", RPCMode.AllBuffered);
		}
	}

	void startNewGameLocalUI()
	{
		gameManager.startNewGameLocal();
	}

	[RPC]
	void startNewGameNetworkedUI()
	{
		gameManager.startNewGameNetworked();
	}
}
