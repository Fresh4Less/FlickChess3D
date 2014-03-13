using UnityEngine;

//
// Valil C# code originally is from chess engine by  http://www.valil.com
// Modified to run on UnityEngine...
// Used for fun & educational purposes, no commerce...
//
using Valil.Chess.Model;
using Valil.Chess.Engine;

		// Object for scripting....
public class ValilScriptObject : MonoBehaviour {

public string Name="Valil chess engine script object";	
public string Request="";
public string Answer="";
public int Deep=3;
	
public ChessEngine Engine1 = new ChessEngine(true);

	// Use this for initialization
	void Start () {
		
	//Application.ExternalEval("JSAlive()");
	((TextMesh)GetComponent(typeof(TextMesh))).text="Waiting...";

	}
	// on Javascript sends a request...
	public void JSRequest(string req_string)
		{
			((TextMesh)GetComponent(typeof(TextMesh))).text="Calculating request...";
			Request=req_string;
			Answer="";
		}
    // on Javascript sets deep level...
	public void JSSetDeep(string set_deep)
		{
			((TextMesh)GetComponent(typeof(TextMesh))).text="Level set to " + set_deep;
			Deep=System.Convert.ToInt32(set_deep);
		}				
		
	// Update is called once per frame
	void Update () {
		if(Request.Length>0)
		{
			string answ="";
			Answer=Engine1.GetNextMove(Request, null, Deep);
			Request="";
			if(Answer.Length>0) answ=Answer.Substring(0,2)+"-"+Answer.Substring(2,2);
			if(Answer.Length>4) answ+="="+(Answer.Substring(4,1)).ToUpper();
			((TextMesh)GetComponent(typeof(TextMesh))).text=answ;
			
			//Application.ExternalCall("JSAnswer", answ);
			
			(GameObject.Find("Script2")).SendMessage("EngineAnswer",answ);
		}
	
	}
	
}
