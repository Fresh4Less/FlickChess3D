using UnityEngine;
using System.Collections;

public class ChessPiece : MonoBehaviour {

	public AudioClip[] collisionSounds;
	public AudioClip collisionSlowMotionSound;
	public GameObject fallTrail;

	public int team;
	private Renderer modelRenderer;
	private float freefallTime;
	private float freefallDuration;
	private Color fallColor;
	private Color initialColor;
	private bool hasTrail;
	private bool destroyedModel;

	// Use this for initialization
	void Start () {
		//colors!
		//transform.Find("Model").GetChild(0).GetChild(0).renderer.material.color = new Color(Random.value, Random.value, Random.value);
		modelRenderer = transform.Find("Model").GetChild(0).GetChild(0).renderer;
		freefallTime = 0.0f;
		freefallDuration = 3.0f;
		hasTrail = false;
		destroyedModel = false;
		initialColor = modelRenderer.material.color;
		fallColor = new Color(1.0f, 0.0f, 0.0f);
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.position.y < -20)
		{
			freefallTime += Time.deltaTime;
			
			if(freefallTime < freefallDuration)
			{
				modelRenderer.material.color = Color.Lerp(initialColor, fallColor, freefallTime / freefallDuration);

				if(!hasTrail)
				{
					GameObject obj = (GameObject) Instantiate(fallTrail,transform.position, Quaternion.identity);
					obj.transform.parent = transform;
					rigidbody.useGravity = false;
				}
			}

			if(freefallTime > 6 && !destroyedModel)
			{
				destroyedModel = true;
				Destroy(transform.Find("Model").gameObject);
			}

			if(freefallTime > 15)
			{
				Destroy(gameObject);
			}
		}
	
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
