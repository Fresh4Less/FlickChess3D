using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

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
	private List<GameObject> chessPiecePrefabs;

	private Vector3 boardOffset = new Vector3(-3.5f,0.0f,-3.5f);
	private Vector2 squareSize = new Vector3(1.0f,1.0f);


	void Start () 
	{
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

		startNewGameLocal();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0))
			flickChessPieceAtMouse();
	}

	void startNewGameLocal()
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

	void flickChessPieceAtMouse()
	{
		// Make sure the user pressed the mouse down
		if (!Input.GetMouseButtonDown (0))
			return;

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

			hit.rigidbody.AddForceAtPosition(rayToMouse.direction * 1000, hit.point);
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