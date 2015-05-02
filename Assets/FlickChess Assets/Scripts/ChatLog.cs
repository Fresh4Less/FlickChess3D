using UnityEngine;
using System.Collections;

public class ChatLog : MonoBehaviour {
	
	public UIManager uiManager;
	public Rect screenRect = new Rect(0.2f,0.7f,0.6f,1.0f);


	private string text = "";
	private Rect screenRectPixels;
	private float fadeoutDuration = 5.0f;
	private float fadeoutTimer;


	public bool isActive = false;
	public Rect boxScreenRect = new Rect(0.2f,0.7f,0.6f,0.1f);
	private Rect boxScreenRectPixels;
	public string boxText = "";

	// Use this for initialization
	void Start () {
		screenRectPixels = FreshTools.calcPixelPosition(screenRect);
		boxScreenRectPixels = FreshTools.calcPixelPosition(boxScreenRect);
		fadeoutTimer = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		fadeoutTimer += Time.deltaTime;

		if(isActive)
		    {
		        foreach (char c in Input.inputString) {
		            // Backspace - Remove the last character
		            if (c == "\b"[0]) 
		            {
		                if (boxText.Length != 0)
		                    boxText = boxText.Substring(0, boxText.Length - 1);
		            }
		            // End of entry
		            /*
		            else if (c == "\n"[0] || c == "\r"[0]) // "\n" for Mac, "\r" for windows.
		            {
		                
		            }
		            */
		            // Normal text input - just append to the end
		            else {
		                boxText += c;
		            }
		        }
		        if(Input.GetKeyDown(KeyCode.Return))
		        {
		            //remove leading spaces
		            int bIndex = 0;
		            while(bIndex < boxText.Length && boxText.Substring(bIndex, 1) == " ")
		            {
		                bIndex++;
		            }
		            string str = boxText.Substring(bIndex, boxText.Length - bIndex);
		            if(str.Length > 0)
		                GetComponent<NetworkView>().RPC("addMessage", RPCMode.All, uiManager.playerName + ": " + str);
		            boxText = "";
		            isActive = false;
		            fadeoutTimer = 0.0f;
		        }
		    }
		    else if(Input.GetKeyDown(KeyCode.Return))
		    {
		        isActive = true;
		    }
	}

	void OnGUI () 
	{
		if(isActive || fadeoutTimer < fadeoutDuration)
		{
		    GUI.skin.label.normal.textColor = new Color(0.0f,0.0f,0.0f,1.0f);
			GUI.Label(screenRectPixels, text);
		}
		 if(isActive)
   		 {
    	    GUI.Box(boxScreenRectPixels, boxText);
    	}
	}

	[RPC]
	void addMessage(string str)
	{
		text = text + str + "\n"[0];
		screenRectPixels.y -= 15;
		fadeoutTimer = 0.0f;
	}
}
