using UnityEngine;
using System.Collections;

public class AsteroidMover : MonoBehaviour
{
	public Vector3 velocity;
	public float angVelocity;

	public GameObject explosion;

	Vector3 axis;

	void Awake ()
	{
		axis = new Vector3(Random.value, Random.value, Random.value);
	}
	
	void Update ()
	{
		transform.position += velocity * Time.deltaTime;
		transform.RotateAround(transform.position, axis, angVelocity * Time.deltaTime);
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Asteroid")
		{
			Instantiate(explosion, transform.position, transform.rotation);
			Destroy(other.gameObject);
			Destroy(gameObject);
		}
	}
}
