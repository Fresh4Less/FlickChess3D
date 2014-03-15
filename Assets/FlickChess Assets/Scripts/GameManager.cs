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
				chessPieces.Add(obj);
			}
		}
/*
		//file, rank (start at 0)
		Vector2 whiteRook1BoardPos = new Vector2(0,0);
		Vector2 whiteKnight1BoardPos = new Vector2(1,0);
		Vector2 whiteBishop1BoardPos = new Vector2(2,0);
		Vector2 whiteQueenBoardPos = new Vector2(3,0);
		Vector2 whiteKingBoardPos = new Vector2(4,0);
		Vector2 whiteBishop2BoardPos = new Vector2(5,0);
		Vector2 whiteKnight2BoardPos = new Vector2(6,0);
		Vector2 whiteRook2BoardPos = new Vector2(7,0);

		GameObject whiteRook1 = (GameObject) Instantiate(BlackRook_Prefab, 
													new Vector3(whiteRook1BoardPos.x * squareSize.x + boardOffset.x,
														-BlackRook_Prefab.transform.Find("Base").position.y + boardOffset.y,
														whiteRook1BoardPos.z * squareSize.y + boardOffset.z), 
													Quaternion.identity);

		chessPieces.Add(whiteRook1);
		*/
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