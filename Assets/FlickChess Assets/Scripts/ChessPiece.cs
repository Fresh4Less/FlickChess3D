using UnityEngine;
using System.Collections;

public class ChessPiece : MonoBehaviour {

	public AudioClip[] collisionSounds;
	public AudioClip collisionSlowMotionSound;
	public GameObject fallTrail;
	public GameObject StarEffect;
	public GameObject MeteorEffect;

	public int team;
	private Renderer modelRenderer;
	private float freefallTime;
	private float freefallDuration1;
	private float freefallDuration2;
	private Color fallColorRed;
	private Color fallColorWhite;
	private Color initialColor;
	private bool hasFallEffects;
	private bool destroyedModel;
	private GameObject star;
	private GameObject meteor;
	private GameObject trail;
	private float yScale;
	private float xzScale;

	// Use this for initialization
	void Start () {
		//colors!
		//transform.Find("Model").GetChild(0).GetChild(0).renderer.material.color = new Color(Random.value, Random.value, Random.value);
		modelRenderer = transform.Find("Model").GetChild(0).GetChild(0).renderer;
		freefallTime = 0.0f;
		freefallDuration1 = 2.0f;
		freefallDuration2 = 3.0f;
		hasFallEffects = false;
		destroyedModel = false;
		initialColor = modelRenderer.material.color;
		fallColorRed = new Color(1.0f, 0.0f, 0.0f);
		fallColorWhite = new Color(1.0f, 1.0f, 1.0f);
		yScale = 0.1f;
		xzScale = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.position.y < -20)
		{
			freefallTime += Time.deltaTime;
			
			if(freefallTime < freefallDuration1)
			{
				//Transition color to red.
				modelRenderer.material.color = Color.Lerp(initialColor, fallColorRed, freefallTime / freefallDuration1);

				//This gets called once, as soon as the chess piece goes into freefall.
				if(!hasFallEffects)
				{
					hasFallEffects = true;
					//Star
					star = (GameObject) Instantiate(StarEffect,transform.position, Quaternion.identity);
					star.transform.parent = transform;
					//Meteor
					meteor = (GameObject) Instantiate(MeteorEffect, transform.position, Quaternion.Euler(1.0f,1.0f,1.0f));
					meteor.transform.localScale = new Vector3(0.0f, 0.1f, 0.0f);

					rigidbody.useGravity = false;
				}
			} 
			else if(freefallTime < freefallDuration2) 
			{
				//Transition the color to white.
				modelRenderer.material.color = Color.Lerp(fallColorRed, fallColorWhite, (freefallTime - freefallDuration1) / freefallDuration2);
				//Trail
				if(!trail) {
					trail = (GameObject) Instantiate(fallTrail,transform.position, Quaternion.identity);
					trail.transform.parent = transform;
				}
			}

			//Destroy the chess piece model if it gets really far away
			if(freefallTime > 10 && !destroyedModel)
			{
				destroyedModel = true;

				Destroy(transform.Find("Model").gameObject);
				
			}

			// Star and Meteor and instatiated at the same time so this works for both.
			if(star)
			{
				//Star Stuff
				star.transform.LookAt(Camera.main.transform.position, -Vector3.up);
				float scale = 0.0f;
				if(freefallTime < 6) 
				{
					scale = transform.position.y * -0.02f * (freefallTime / 6);
				}
				else
				{
					scale = transform.position.y * -0.02f;
				}
				star.transform.localScale = new Vector3(scale,scale,scale);

				//Meteor Stuff
				meteor.transform.position = new Vector3(transform.position.x+this.rigidbody.velocity.normalized.x, transform.position.y-0.7f, transform.position.z+this.rigidbody.velocity.normalized.z);
				meteor.transform.up = -this.rigidbody.velocity.normalized;
				//Fade in the effect



				if(freefallTime < 4) 
				{
					yScale = transform.position.y * -0.02f * (freefallTime / 4);
					if(freefallTime < 2) {
						xzScale = freefallTime;
					}
				}
				meteor.transform.localScale = new Vector3(xzScale, yScale, xzScale);

			}

			if(transform.position.y < -2000)
			{
				Destroy(gameObject);
				Destroy(star);
				Destroy(meteor);
				Destroy(trail);
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
