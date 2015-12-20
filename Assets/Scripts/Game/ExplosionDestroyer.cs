using UnityEngine;
using System.Collections;

public class ExplosionDestroyer : MonoBehaviour
{
	ParticleSystem ps;

	void Start ()
	{
		ps = GetComponent<ParticleSystem>();
		float destroyIn = ps.duration + ps.startLifetime;

		Destroy(gameObject, destroyIn);
	}
}
