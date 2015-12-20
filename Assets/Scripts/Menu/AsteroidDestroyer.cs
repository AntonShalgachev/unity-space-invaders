using UnityEngine;
using System.Collections;

public class AsteroidDestroyer : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnTriggerExit(Collider other)
	{
		if(other.tag == "Asteroid")
		{
			Destroy(other.gameObject);
		}
	}
}
