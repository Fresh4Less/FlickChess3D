using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public AudioClip enterSlowMotionSound;
	public AudioClip exitSlowMotionSound;

	public GameObject BlackBishop_Prefab;
	public GameObject BlackKing_Prefab;
	public GameObject BlackKnight_Prefab;
	public GameObject BlackPawn_Prefab;
	public GameObject BlackQueen_Prefab;
	public GameObject BlackRook_Prefab;

	public GameObject WhiteBishop_Prefab;
	public GameObject WhiteKing_Prefab;
	public GameObject WhiteKnight_Prefab;
	public GameObject WhitePawn_Prefab;
	public GameObject WhiteQueen_Prefab;
	public GameObject WhiteRook_Prefab;

	private bool networkedGame = false;
	private List<GameObject> chessPieces;
	public List<GameObject> chessPiecePrefabs;

	private Vector3 boardOffset = new Vector3(-3.5f,0.0f,-3.5f);
	private Vector2 squareSize = new Vector3(1.0f,1.0f);

	//instant replay
	private GameObject lastPieceLaunched;
	static private bool slowMotion;
	private bool checkSlowMotion;
	private float slowMotionTimer;
	public static float slowMotionTriggerDistance = 2.0f;
	public static float slowMotionTriggerSpeed = 0.01f;
	public static float slowMotionMinDuration = 1.0f;
	public static float slowMotionDilation = 0.25f;

	private bool sendRigidbodyData = false;
	private int sendRigidbodyDelay = 0;

	void Start () 
	{
		slowMotion = false;
		checkSlowMotion = false;
		slowMotionTimer = 0.0f;


		chessPieces = new List<GameObject>();

		chessPiecePrefabs = new List<GameObject>();
		chessPiecePrefabs.Add(null);
		chessPiecePrefabs.Add(WhiteBishop_Prefab);
		chessPiecePrefabs.Add(WhiteKing_Prefab);
		chessPiecePrefabs.Add(WhiteKnight_Prefab);
		chessPiecePrefabs.Add(WhitePawn_Prefab);
		chessPiecePrefabs.Add(WhiteQueen_Prefab);
		chessPiecePrefabs.Add(WhiteRook_Prefab);
		chessPiecePrefabs.Add(BlackBishop_Prefab);
		chessPiecePrefabs.Add(BlackKing_Prefab);
		chessPiecePrefabs.Add(BlackKnight_Prefab);
		chessPiecePrefabs.Add(BlackPawn_Prefab);
		chessPiecePrefabs.Add(BlackQueen_Prefab);
		chessPiecePrefabs.Add(BlackRook_Prefab);

		//startNewGameLocal();
	}
	
	// Update is called once per frame
	void Update () {

		if(slowMotion)
			updateSlowMotion();

		if(checkSlowMotion)
		{
			if(doSlowMotionCheck())
			{
				activateSlowMotion();
			}
		}

		if(Input.GetMouseButtonDown(0))
			flickChessPieceAtMouse();
	}

	void FixedUpdate()
	{
		if(sendRigidbodyData)
		{
			sendRigidbodyDelay++;
			if(sendRigidbodyDelay == 2)
			{
				GameObject obj = lastPieceLaunched;
				networkView.RPC("onPieceFlicked", RPCMode.Others, obj.networkView.viewID, obj.transform.position, obj.transform.rotation, obj.rigidbody.velocity, obj.rigidbody.angularVelocity);
				sendRigidbodyData = false;
				sendRigidbodyDelay = 0;
			}
		}
	}

	public void startNewGameLocal()
	{
		Debug.Log("NEW LOCAL GAME");
		networkedGame = false;
		//GetComponent(DragRigidbody).enabled = true;
		//GetComponent(DragRigidbodyNetwork).enabled = false;
		
		destroyAllChessPiecesLocal();
		createAndSetupChessPiecesLocal();
	}

	void destroyAllChessPiecesLocal()
	{
			//var oldObjs:GameObject[] = GameObject.FindGameObjectsWithTag("ChessPiece");
		//for(var obj:GameObject in oldObjs)
		foreach(GameObject obj in chessPieces)
		{
			Destroy(obj);
		}
		chessPieces.Clear();
	}

	void createAndSetupChessPiecesLocal()
	{
		for(int file = 0; file < 8; file++)
		{
			for(int rank = 0; rank < 8; rank++)
			{
				int pieceType = chessBoardSetupStandard[7-rank, file];
				GameObject objPrefab = chessPiecePrefabs[pieceType];
				if(objPrefab == null)
					continue;
				GameObject obj = (GameObject) Instantiate(objPrefab,
														 new Vector3(file * squareSize.x + boardOffset.x,
														 			 -objPrefab.transform.Find("Base").position.y + boardOffset.y,
														 			 rank * squareSize.y + boardOffset.z), 
														 Quaternion.identity);
				if(pieceType > 6) //is a black piece
					obj.transform.Rotate(new Vector3(0,180,0));
				chessPieces.Add(obj);
			}
		}
	}

	public void startNewGameNetworked()
	{
		Debug.Log("NEW NETWORKED GAME");
		networkedGame = true;
		if(Network.isServer)
		{
			destroyAllChessPiecesNetworked();
			createAndSetupChessPiecesNetworked();
		}	
	}

	void destroyAllChessPiecesNetworked()
	{
		foreach(GameObject obj in chessPieces)
		{
			Network.RemoveRPCs(obj.networkView.viewID);
			Network.Destroy(obj);
		}
	}

	void createAndSetupChessPiecesNetworked()
	{
		for(int file = 0; file < 8; file++)
		{
			for(int rank = 0; rank < 8; rank++)
			{
				int pieceType = chessBoardSetupStandard[7-rank, file];
				GameObject objPrefab = chessPiecePrefabs[pieceType];
				if(objPrefab == null)
					continue;
					GameObject obj = null;
					if(pieceType < 7) //white piece
					{
				obj = (GameObject) Network.Instantiate(objPrefab,
														 new Vector3(file * squareSize.x + boardOffset.x,
														 			 -objPrefab.transform.Find("Base").position.y + boardOffset.y,
														 			 rank * squareSize.y + boardOffset.z), 
														 Quaternion.identity, 0);
				}
				else
				obj = (GameObject) Network.Instantiate(objPrefab,
														 new Vector3(file * squareSize.x + boardOffset.x,
														 			 -objPrefab.transform.Find("Base").position.y + boardOffset.y,
														 			 rank * squareSize.y + boardOffset.z), 
														 Quaternion.Euler(0,180,0), 0);
				//if(pieceType > 6) //is a black piece
				//	obj.transform.Rotate(new Vector3(0,180,0));
				chessPieces.Add(obj);
			}
		}
	}

	void flickChessPieceAtMouse()
	{
		 Camera mainCamera = Camera.main;
			
		// We need to actually hit an object
		RaycastHit hit;
		Ray rayToMouse = mainCamera.ScreenPointToRay(Input.mousePosition);
		if (!Physics.Raycast(rayToMouse, out hit, 100))
			return;
		// We need to hit a rigidbody that is not kinematic
		if (!hit.rigidbody || hit.rigidbody.isKinematic)
			return;
		//make sure it's a chesspiece
		if(hit.transform.tag != "ChessPiece")
			return;

			if(networkedGame)
			{
				applyForceToPiece(hit.transform.networkView.viewID, rayToMouse.direction * 1000 * (1.0f/Time.timeScale), hit.point);
				if(Network.isClient)
				{
					networkView.RPC("applyForceToPiece", RPCMode.Server, hit.transform.networkView.viewID, rayToMouse.direction * 1000 * (1.0f/Time.timeScale), hit.point);
				}
			}
			else
			{
				applyForceToPiece(hit.transform.networkView.viewID, rayToMouse.direction * 1000 * (1.0f/Time.timeScale), hit.point);
			}

			//lastPieceLaunched = hit.transform.gameObject;
			//checkSlowMotion = true;
	}

	[RPC]
	void applyForceToPiece(NetworkViewID piece, Vector3 force, Vector3 point)
	{
		var obj = NetworkView.Find(piece).gameObject;
		obj.rigidbody.AddForceAtPosition(force, point);
		onPieceFlicked(piece, obj.transform.position, obj.transform.rotation, obj.rigidbody.velocity, obj.rigidbody.angularVelocity);
		if(networkedGame && Network.isServer)
		{
			sendRigidbodyData = true;
		}

	}

	[RPC]
	public void onPieceFlicked(NetworkViewID piece, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
	{
		var obj = NetworkView.Find(piece).gameObject;
		lastPieceLaunched = obj;
		checkSlowMotion = true;
		if(networkedGame && Network.isClient)
			obj.GetComponent<ChessPiece>().updateClientChessPiecePhysicsData(piece, position, rotation, velocity, angularVelocity);
	}

	bool doSlowMotionCheck()
	{
		if(lastPieceLaunched.rigidbody.velocity.magnitude < slowMotionTriggerSpeed || lastPieceLaunched.transform.position.y < boardOffset.y)
		{
			checkSlowMotion = false;
			return false;
		}
		//find chess piece of opposing color in radius
		Collider[] colliders = Physics.OverlapSphere (lastPieceLaunched.transform.position, slowMotionTriggerDistance);
	    foreach(Collider hit in colliders) 
	    {
	        ChessPiece chessComponent = hit.transform.parent.parent.gameObject.GetComponent<ChessPiece>();
	        if(chessComponent != null && lastPieceLaunched.gameObject.GetComponent<ChessPiece>().team != chessComponent.team)
	        	return true;
	    }

		return false;
	}

	void activateSlowMotion()
	{
		slowMotion = true;
		checkSlowMotion = false;
		slowMotionTimer = 0.0f;
		Time.timeScale = slowMotionDilation;
		Time.fixedDeltaTime = 0.02f * slowMotionDilation;
		GetComponent<AudioSource>().PlayOneShot(enterSlowMotionSound);
	}

	void disableSlowMotion()
	{
		slowMotion = false;
		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = 0.02f;
		//AudioSource.PlayClipAtPoint(exitSlowMotionSound, Camera.main.transform.position);
	}

	void updateSlowMotion()
	{
		slowMotionTimer += Time.deltaTime * (1.0f / Time.timeScale);
		if(slowMotionTimer > slowMotionMinDuration)
		{
			disableSlowMotion();
		}
	}

	static public bool slowMotionEnabled()
	{
		return slowMotion;
	}


	//key - none = 0, 
	//White:
	//Bishop = 1
	//King   = 2
	//Knight = 3
	//Pawn   = 4
	//Queen  = 5
	//Rook   = 6
	//Black:
	//Bishop = 7
	//King   = 8
	//Knight = 9
	//Pawn   = 10
	//Queen  = 11
	//Rook   = 12
	private int[,] chessBoardSetupStandard = new int[,] 
	{ 
		{ 12, 9, 7,11, 8, 7, 9,12 },
		{ 10,10,10,10,10,10,10,10 },
		{  0, 0, 0, 0, 0, 0, 0, 0 },
		{  0, 0, 0, 0, 0, 0, 0, 0 },
		{  0, 0, 0, 0, 0, 0, 0, 0 },
		{  0, 0, 0, 0, 0, 0, 0, 0 },
		{  4, 4, 4, 4, 4, 4, 4, 4 },
		{  6, 3, 1, 5, 2, 1, 3, 6 }
	  };
}