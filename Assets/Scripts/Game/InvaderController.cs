using UnityEngine;
using System.Collections;

public class InvaderController : MonoBehaviour
{
	public int score;

	public GameObject shotPrefab;
	public Transform shotSpawn;
	public float shotVelocity;

	public float shotDelayMin;
	public float shotDelayMax;

	public GameObject explosion;

	public AudioClip shotClip;

	public int aimThreshold;
	public float aimCooldown;

	GameController gc;

	bool paused;

	void Awake()
	{
		GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
		gc = gameController.GetComponent<GameController>();

		paused = false;

		StartCoroutine(ShotSpawner());
	}

	IEnumerator ShotSpawner()
	{
		bool aimToPlayer = false;
		while(true)
		{
			aimToPlayer = Random.Range(0, 100) < aimThreshold;
			if(!aimToPlayer)
			{
				float delay = Random.Range(shotDelayMin, shotDelayMax);

				yield return new WaitForSeconds(delay);

				if(!paused)
				{
					RaycastHit hitInfo;
					Physics.Raycast(transform.position, Vector3.down, out hitInfo);
					if(hitInfo.collider.tag != "Invader1" && hitInfo.collider.tag != "Invader2" && hitInfo.collider.tag != "Invader3")
					{
						Fire();
					}
				}
			}
			else
			{
				while(paused || (Physics.Raycast(transform.position, Vector3.down, Mathf.Infinity, 1 << 8) == false))
				{
					yield return null;
				}

				RaycastHit hitInfo;
				Physics.Raycast(transform.position, Vector3.down, out hitInfo);
				if(hitInfo.collider.tag != "Invader1" && hitInfo.collider.tag != "Invader2" && hitInfo.collider.tag != "Invader3")
				{
					Debug.Log("Raycast fire!");
					Fire();
				}

				yield return new WaitForSeconds(aimCooldown);
			}
		}
	}

	void Fire()
	{
		GameObject shot = Instantiate(shotPrefab, shotSpawn.position, Quaternion.identity) as GameObject;
		shot.tag = "InvaderShot";
		ShotController gc = shot.GetComponent<ShotController>();

		AudioSource source = shot.GetComponent<AudioSource>();
		source.clip = shotClip;
		source.Play();

		gc.velocity = Vector3.down * shotVelocity;
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Shot")
		{
			Instantiate(explosion, transform.position, transform.rotation);
			Destroy(other.gameObject);

			gc.DecreaseStepTime(true);
			gc.OnInvaderDead(gameObject, score);
		}
	}

	public void Pause(bool pause)
	{
		//Debug.Log("Invader is paused:" + pause);
		paused = pause;
	}
}
