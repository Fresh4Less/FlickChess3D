using UnityEngine;
using System.Collections;

public class SpawnPieces : MonoBehaviour {


	public GameObject trailPrefab;
	private int timer;
	// Use this for initialization
	void Start () {
		timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		

	}

	void FixedUpdate () {
		timer++;
		if(timer % 10 != 0)
			return;
		GameObject objPrefab = /*trailPrefab;*/gameObject.GetComponent<GameManager>().chessPiecePrefabs[Random.Range(1, gameObject.GetComponent<GameManager>().chessPiecePrefabs.Count - 1)];

		Vector3 randomPosition = new Vector3(Random.Range(-10,10),10,Random.Range(-10,10));
		GameObject obj = (GameObject) Instantiate(objPrefab, randomPosition, Quaternion.identity);
		Vector3 randomDirection = new Vector3(Random.Range(-359, 359),Random.Range(-359, 359),Random.Range(-359, 359));
		Vector3 randomTorque = new Vector3(Random.Range(-359, 359),Random.Range(-359, 359),Random.Range(-359, 359));
		obj.GetComponent<Rigidbody>().AddForce(randomDirection * (1.0f/Time.timeScale));
		//obj.rigidbody.AddTorque(randomTorque * 1000 * (1.0f/Time.timeScale));
	}
}
