using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsteroidSpawner : MonoBehaviour
{
	public GameObject[] asteroidPrefabs;
	public Transform[] spawnPositions;

	public int numberOfAsteroids;

	public float speedMin;
	public float speedMax;

	public float rotSpeedMin;
	public float rotSpeedMax;

	List<GameObject> asteroids;

	void Awake ()
	{
		asteroids = new List<GameObject>();
		if(spawnPositions.Length < 2)
			Debug.LogError("Not enough spawns");
	}
	
	void Update ()
	{
		for(int i = 0; i < asteroids.Count; i++)
		{
			if(asteroids[i] == null)
			{
				asteroids.RemoveAt(i);
			}
		}

		if(asteroids.Count < numberOfAsteroids)
		{
			for(int i = 0; i < numberOfAsteroids - asteroids.Count; i++)
			{
				int transformIndex = Random.Range(0, spawnPositions.Length);
				Transform spawnPosition = spawnPositions[transformIndex];
				GameObject asteroidPrefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];

				GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition.position, Quaternion.identity) as GameObject;
				asteroids.Add(asteroid);

				int destinationIndex;
				do
				{
					destinationIndex = Random.Range(0, spawnPositions.Length);
				}
				while(destinationIndex == transformIndex);
				Transform destination = spawnPositions[destinationIndex];

				Vector3 velocity = destination.position - spawnPosition.position;

				float speed = Random.Range(speedMin, speedMax);
				float rotSpeed = Random.Range(rotSpeedMin, rotSpeedMax);

				AsteroidMover am = asteroid.GetComponent<AsteroidMover>();
				am.velocity = velocity.normalized * speed;
				am.angVelocity = rotSpeed;
			}
		}
	}
}
