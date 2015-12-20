using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	public float movementDeltaX;
	public float movementDeltaY;
	public float leftBorder;
	public float rightBorder;

	public GameObject invader1Prefab;
	public GameObject invader2Prefab;
	public GameObject invader3Prefab;
	public float layoutDeltaX;
	public float layoutDeltaY;

	public int invadersInARow;

	public float posYMin;
	public float posYMax;

	public float movementDeltaTime;
	public float enemyDeltaTime;

	public GameObject gameOverBanner;
	public GameObject nextLevelText;
	public GameObject loseBanner;
	public GameObject restartText;

	public float bannerCooldownTime;

	public Text scoreText;
	public Text highscoreText;
	public Text levelText;

	public GameObject mysteryPrefab;
	public float mysteryY;
	public float mysteryXLeft;
	public float mysteryXRight;
	public float mysteryDelayMin;
	public float mysteryDelayMax;
	public float mysteryVelocity;

	public float deadlineY;
	public float invadersDeltaTimeMin;

	public GameObject basePrefab;
	public float basePosY;
	public float[] basePosXs;

	public GameObject menuCanvas;
	public GameObject gameCanvas;

	public GameObject playerPrefab;
	public float playerPosY;

	public float respawnTime;

	public GameObject liveIndicatorPrefab;
	public float indicatorPosY;
	public float indicatorPosLeft;
	public float indicatorOffset;

	public int livesMax;

	public AudioClip transitionClip;
	public AudioClip gameClip;

	//public AudioClip[] invaderMovements;

	//int currentMovementClip;

	Animator camAnimator;

	int level;

	List<GameObject> invaders;
	List<InvaderAnimator> invaderAnimators;

	GameObject invaderParent;

	GameObject player;

	float movementDirection;

	float stepTime;

	bool levelComplete;
	bool youLose;
	float cooldownRemaining;

	int score;
	int highscore;

	int lives;

	bool paused;
	bool inMainMenu;

	AudioSource audioSource;

	void Awake ()
	{
		// Cursor.lockState = CursorLockMode.Locked;
		// Cursor.visible = false;

		audioSource = GetComponent<AudioSource>();
		inMainMenu = true;
		camAnimator = Camera.main.GetComponent<Animator>();
		menuCanvas.SetActive(true);
		gameCanvas.SetActive(false);

		InitGame();
	}

	void UpdateScore()
	{
		highscore = Mathf.Max(score, highscore);
		PlayerPrefs.SetInt("Highscore", highscore);

		scoreText.text = "Score\n" + score;
		highscoreText.text = "Highscore\n" + highscore;
	}

	void UpdateLevelText()
	{
		levelText.text = "Level\n" + level;
	}

	void SpawnBases()
	{
		GameObject[] bases = GameObject.FindGameObjectsWithTag("Base");
		foreach(GameObject b in bases)
		{
			Destroy(b);
		}

		foreach(float basePosX in basePosXs)
		{
			Instantiate(basePrefab, new Vector3(basePosX, basePosY, 0.0f), Quaternion.Euler(90.0f, 0.0f, 0.0f));
		}
	}

	void SpawnPlayer()
	{
		GameObject oldPlayer = GameObject.FindGameObjectWithTag("Player");
		if(oldPlayer != null)
			Destroy(oldPlayer);

		player = Instantiate(playerPrefab, new Vector3(0f, playerPosY, 0f), Quaternion.identity) as GameObject;
	}

	void SpawnIndicators()
	{
		GameObject[] inds = GameObject.FindGameObjectsWithTag("Indicator");
		foreach(GameObject i in inds)
		{
			Destroy(i);
		}

		for(int i = 0; i < lives; i++)
		{
			float posX = indicatorPosLeft + i * indicatorOffset;
			float posY = indicatorPosY;

			Instantiate(liveIndicatorPrefab, new Vector3(posX, posY, 0.0f), Quaternion.identity);
		}
	}

	void InitGame()
	{
		level = 1;
		score = 0;
		lives = 3;
		highscore = PlayerPrefs.GetInt("Highscore", 0);
		UpdateScore();

		InitLevel();

		SpawnBases();
		SpawnPlayer();

		StopAllCoroutines();
		StartCoroutine(StartMovement());
		StartCoroutine(MysterySpawner());

		Pause(true);
	}

	void InitLevel()
	{
		//currentMovementClip = 0;

		UpdateLevelText();
		
		SpawnInvaders();

		float posY = Mathf.Lerp(posYMax, posYMin, (level - 1) * 0.1f);
		invaderParent.transform.position = Vector3.up * posY;

		movementDirection = movementDeltaX;
		stepTime = 1f;
		cooldownRemaining = 0.0f;
		levelComplete = false;
		youLose = false;

		gameOverBanner.SetActive(false);
		loseBanner.SetActive(false);
	}

	void StartGame()
	{
		Pause(false);
	}
	
	void Update ()
	{
		if(Input.GetKeyUp(KeyCode.Space) && inMainMenu)
		{
			menuCanvas.SetActive(false);
			camAnimator.SetTrigger("StartGame");
			audioSource.Stop();
			audioSource.clip = gameClip;
			audioSource.Play();
			audioSource.PlayOneShot(transitionClip);
			inMainMenu = false;
		}

		if(levelComplete)
		{
			if(cooldownRemaining > 0.0f)
			{
				cooldownRemaining -= Time.deltaTime;
			}
			else
			{
				nextLevelText.SetActive(true);

				if(Input.GetKeyUp(KeyCode.Space))
				{
					level++;
					lives++;
					if(lives > livesMax)
						lives = livesMax;

					SpawnIndicators();

					InitLevel();

					Pause(false);
				}
			}
		}

		if(youLose)
		{
			if(cooldownRemaining > 0.0f)
			{
				cooldownRemaining -= Time.deltaTime;
			}
			else
			{
				restartText.SetActive(true);

				if(Input.GetKeyUp(KeyCode.Space))
				{
					InitGame();
					SpawnIndicators();
					StartGame();
				}
			}
		}
	}

	void SpawnInvaders()
	{
		GameObject toRemove = GameObject.Find("Invaders");
		Destroy(toRemove);

		invaderParent = new GameObject("Invaders");

		invaders = new List<GameObject>();
		invaderAnimators = new List<InvaderAnimator>();

		float rowWidth = (invadersInARow - 1) * layoutDeltaX;
		float offsetX = - rowWidth / 2.0f;

		float colHeight = 4 * layoutDeltaY;
		float offsetY = - colHeight / 2.0f;

		for(int i = 0; i < invadersInARow; i++)
		{
			float posX = offsetX + i * layoutDeltaX;

			float posY1 = offsetY;
			float posY2 = offsetY + layoutDeltaY;
			float posY3 = offsetY + 2 * layoutDeltaY;
			float posY4 = offsetY + 3 * layoutDeltaY;
			float posY5 = offsetY + 4 * layoutDeltaY;

			GameObject invader1 = Instantiate(invader3Prefab, new Vector3(posX, posY1, 0.0f), Quaternion.identity) as GameObject;
			GameObject invader2 = Instantiate(invader3Prefab, new Vector3(posX, posY2, 0.0f), Quaternion.identity) as GameObject;
			GameObject invader3 = Instantiate(invader2Prefab, new Vector3(posX, posY3, 0.0f), Quaternion.identity) as GameObject;
			GameObject invader4 = Instantiate(invader2Prefab, new Vector3(posX, posY4, 0.0f), Quaternion.identity) as GameObject;
			GameObject invader5 = Instantiate(invader1Prefab, new Vector3(posX, posY5, 0.0f), Quaternion.identity) as GameObject;

			invader1.transform.parent = invaderParent.transform;
			invader2.transform.parent = invaderParent.transform;
			invader3.transform.parent = invaderParent.transform;
			invader4.transform.parent = invaderParent.transform;
			invader5.transform.parent = invaderParent.transform;

			invaders.Add(invader1);
			invaders.Add(invader2);
			invaders.Add(invader3);
			invaders.Add(invader4);
			invaders.Add(invader5);

			invaderAnimators.Add(invader1.GetComponent<InvaderAnimator>());
			invaderAnimators.Add(invader2.GetComponent<InvaderAnimator>());
			invaderAnimators.Add(invader3.GetComponent<InvaderAnimator>());
			invaderAnimators.Add(invader4.GetComponent<InvaderAnimator>());
			invaderAnimators.Add(invader5.GetComponent<InvaderAnimator>());
		}
	}

	IEnumerator StartMovement()
	{
		//while(!levelComplete && !youLose)
		while(true)
		{
			yield return new WaitForSeconds(stepTime);

			if(!paused)
				MoveInvaders();
		}
	}

	IEnumerator MysterySpawner()
	{
		//while(!levelComplete && !youLose)
		while(true)
		{
			float delay = Random.Range(mysteryDelayMin, mysteryDelayMax);
			yield return new WaitForSeconds(delay);

			if(!paused)
				SpawnMystery();
		}

	}

	void SpawnMystery()
	{
		Debug.Log("Here comes the mystery ship");

		float[] posXs = {mysteryXLeft, mysteryXRight};
		float[] velocities = {mysteryVelocity, -mysteryVelocity};
		int index = Random.Range(0, 2);
		float posX = posXs[index];
		float posY = mysteryY;

		GameObject mystery = Instantiate(mysteryPrefab, new Vector3(posX, posY, 0.0f), Quaternion.identity) as GameObject;

		MyteryController mc = mystery.GetComponent<MyteryController>();
		mc.velocity = velocities[index];
		mc.maxDistance = Mathf.Abs(posX);
	}

	void MoveInvaders()
	{
		invaderParent.transform.position += Vector3.right * movementDirection;

		for(int i = 0; i < invaders.Count; i++)
		{
			invaderAnimators[i].NextFrame();

			if(invaders[i].transform.position.x < leftBorder || invaders[i].transform.position.x > rightBorder)
			{
				invaderParent.transform.position -= Vector3.right * movementDirection;
				invaderParent.transform.position += Vector3.down * movementDeltaY;

				movementDirection = -movementDirection;

				DecreaseStepTime();
			}

			if(invaders[i].transform.position.y < deadlineY)
			{
				OnGameover();
			}
		}

		//audioSource.PlayOneShot(invaderMovements[currentMovementClip]);
		//currentMovementClip = (currentMovementClip + 1) % invaderMovements.Length;
	}

	void OnGameover()
	{
		//StopAllCoroutines();

		restartText.SetActive(false);
		loseBanner.SetActive(true);
		youLose = true;
		cooldownRemaining = bannerCooldownTime;

		Pause(true);

		GameObject[] mysteries = GameObject.FindGameObjectsWithTag("Mystery");
		foreach(GameObject m in mysteries)
		{
			Destroy(m);
		}

		//StopAllCoroutines();
	}

	public void DecreaseStepTime(bool enemy = false)
	{
		float delta;
		if(enemy)
			delta = enemyDeltaTime;
		else
			delta = movementDeltaTime;

		stepTime *= (1.0f - Mathf.Log(level + 1) * delta);
		stepTime = Mathf.Max(stepTime, invadersDeltaTimeMin);
	}

	public void OnInvaderDead(GameObject invader, int scorePerInvader)
	{
		int pos = invaders.IndexOf(invader);

		invaders.RemoveAt(pos);
		invaderAnimators.RemoveAt(pos);

		Destroy(invader);

		score += scorePerInvader;
		UpdateScore();

		if(invaders.Count == 0)
		{
			GameObject[] mysteries = GameObject.FindGameObjectsWithTag("Mystery");
			foreach(GameObject m in mysteries)
			{
				Destroy(m);
			}

			GameObject[] shotsToDelete = GameObject.FindGameObjectsWithTag("Shot");
			foreach(GameObject s in shotsToDelete)
			{
				Destroy(s);
			}

			shotsToDelete = GameObject.FindGameObjectsWithTag("InvaderShot");
			foreach(GameObject s in shotsToDelete)
			{
				Destroy(s);
			}
			
			nextLevelText.SetActive(false);
			gameOverBanner.SetActive(true);
			levelComplete = true;
			cooldownRemaining = bannerCooldownTime;
			//StopAllCoroutines();
			Pause(true);
		}
	}

	public void OnMysteryDead(int scorePerInvader)
	{
		score += scorePerInvader;
		UpdateScore();
	}

	public void OnPlayerHit()
	{
		lives--;

		Pause(true);

		Destroy(player);

		GameObject[] shots = GameObject.FindGameObjectsWithTag("InvaderShot");
		foreach(GameObject shot in shots)
			Destroy(shot);
		
		shots = GameObject.FindGameObjectsWithTag("Shot");
		foreach(GameObject shot in shots)
			Destroy(shot);

		if(lives < 0)
		{
			OnGameover();
			return;
		}

		Invoke("RespawnPlayer", respawnTime);
	}

	void Pause(bool pause)
	{
		PlayerController pc = player.GetComponent<PlayerController>();
		pc.Pause(pause);

		foreach(GameObject inv in invaders)
		{
			InvaderController ic = inv.GetComponent<InvaderController>();
			ic.Pause(pause);
		}

		GameObject[] mysteries = GameObject.FindGameObjectsWithTag("Mystery");
		foreach(GameObject m in mysteries)
		{
			MyteryController mc = m.GetComponent<MyteryController>();
			if(mc != null)
				mc.Pause(pause);
			else
				Debug.Log("Mystery ship is in What-The-Fuck state");
		}

		paused = pause;
	}

	public void OnAnimationEnd()
	{
		gameCanvas.SetActive(true);
		SpawnIndicators();

		StartGame();
	}

	void RespawnPlayer()
	{
		SpawnPlayer();
		SpawnIndicators();
		Pause(false);
	}
}
