using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	public float speed;
	public float shotSpeed;

	public float leftBorder;
	public float rightBorder;

	public GameObject shot;
	public Transform spawn;

	public bool debugCheat;
	public float shotDelay;

	public GameObject explosion;

	public AudioClip shotClip;

	bool canShoot;

	Rigidbody rb;
	GameController gc;

	bool paused;

	void Awake ()
	{
		rb = GetComponent<Rigidbody>();
		if(rb == null)
		{
			Debug.LogError("Rigidbody is missing");
		}

		canShoot = true;
		paused = false;

		GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
		gc = gameController.GetComponent<GameController>();
	}
	
	void Update ()
	{
		if(paused)
		{
			return;
		}

		float horizontalAxis = Input.GetAxisRaw("Horizontal");

		Vector3 velocity = Vector3.right * horizontalAxis * speed;
		Vector3 newPosition = transform.position + velocity * Time.deltaTime;
		newPosition.x = Mathf.Clamp(newPosition.x, leftBorder, rightBorder);

		transform.position = newPosition;

		if(Input.GetButton("Jump"))
		{
			if(canShoot)
			{
				if(debugCheat || GameObject.FindGameObjectsWithTag("Shot").Length == 0)
				{
					GameObject obj = Instantiate(shot, spawn.position, Quaternion.identity) as GameObject;
					ShotController sc = obj.GetComponent<ShotController>();

					sc.velocity = Vector3.up * shotSpeed;

					AudioSource source = obj.GetComponent<AudioSource>();
					source.clip = shotClip;
					source.Play();

					canShoot = false;
					Invoke("EnableGun", shotDelay);
				}
			}
		}
	}

	void EnableGun()
	{
		canShoot = true;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "InvaderShot")
		{
			Instantiate(explosion, transform.position, transform.rotation);
			gc.OnPlayerHit();
		}
	}

	public void Pause(bool pause)
	{
		paused = pause;
	}
}
