using UnityEngine;
using System.Collections;

public class BaseBrickController : MonoBehaviour
{
	public float radiusMin;
	public float radiusMax;

	public int threshold;
	public int invaderThreshold;

	public float invaderRadiusMul;

	public GameObject explosion;

	void Start ()
	{
		
	}
	
	void Update ()
	{
		
	}

	void OnTriggerEnter(Collider other)
	{
		bool invader = other.tag == "Invader1" || other.tag == "Invader2" || other.tag == "Invader3";
		if(other.tag == "Shot" || other.tag == "InvaderShot" || invader)
		{
			float radMultiplier = (other.tag != "InvaderShot") ? 1.0f : invaderRadiusMul;
			GameObject baseBlast = new GameObject("BaseBlast");
			SphereCollider collider = baseBlast.AddComponent<SphereCollider>();
			collider.center = transform.position;
			collider.radius = Random.Range(radiusMin, radiusMax) * radMultiplier;
			collider.tag = invader ? "BaseBlastInvader" : "BaseBlast";
			collider.isTrigger = true;

			AudioSource source = GetComponent<AudioSource>();
			source.Play();

			if(!invader)
				Destroy(other.gameObject);
			Destroy(baseBlast, 0.1f);

		}
		else if(other.tag == "BaseBlast")
		{
			if(tag != "BaseBlast" && tag != "BaseBlastInvader")
			{
				if(Random.Range(0, 100) < threshold)
				{
					Destroy(gameObject);
					Instantiate(explosion, transform.position, transform.rotation);
				}
			}
		}
		else if(other.tag == "BaseBlastInvader")
		{
			if(tag != "BaseBlast" && tag != "BaseBlastInvader")
			{
				if(Random.Range(0, 100) < invaderThreshold)
				{
					Destroy(gameObject);
					Instantiate(explosion, transform.position, transform.rotation);
				}
			}
		}
	}
}
