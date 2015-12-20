using UnityEngine;
using System.Collections;

public class ShotController : MonoBehaviour
{
	public Vector3 velocity;

	Rigidbody rb;

	void Start ()
	{
		rb = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate ()
	{
		Vector3 newPosition = transform.position + velocity * Time.deltaTime;

		//transform.position = newPosition;
		rb.MovePosition(newPosition);
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "ShotBorder")
			Destroy(gameObject);
	}
}
