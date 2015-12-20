using UnityEngine;
using System.Collections;

public class MyteryController : MonoBehaviour
{
	public float velocity;
	public float maxDistance;

	public int score;

	public GameObject explosion;

	GameController gc;

	bool paused;

	void Awake()
	{
		GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
		gc = gameController.GetComponent<GameController>();

		paused = false;
	}

	void Update()
	{
		if(paused)
			return;
		
		transform.position += Vector3.right * velocity * Time.deltaTime;

		if(Mathf.Abs(transform.position.x) > maxDistance)
		{
			DestroyObject(gameObject);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Shot")
		{
			Instantiate(explosion, transform.position, transform.rotation);

			Destroy(other.gameObject);
			Destroy(gameObject);

			gc.OnMysteryDead(score);
		}
	}

	public void Pause(bool pause)
	{
		paused = pause;
	}
}
