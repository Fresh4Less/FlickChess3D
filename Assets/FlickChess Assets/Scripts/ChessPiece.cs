using UnityEngine;
using System.Collections;

public class ChessPiece : MonoBehaviour {

	public AudioClip[] collisionSounds;
	public AudioClip collisionSlowMotionSound;

	public int team;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionEnter(Collision collision)
	{
		if(collision.relativeVelocity.magnitude > 2)
		{
			if(GameManager.slowMotionEnabled())
				audio.PlayOneShot(collisionSlowMotionSound, 0.3f);
			else
			{
				audio.PlayOneShot(collisionSounds[Random.Range(0,collisionSounds.Length - 1)]);
			}
		}
	}
}
