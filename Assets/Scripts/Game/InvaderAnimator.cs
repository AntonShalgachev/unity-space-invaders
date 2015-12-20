using UnityEngine;
using System.Collections;

public class InvaderAnimator : MonoBehaviour
{
	public GameObject[] frames;
	public float animationTime;

	int nFrames;
	int currentFrame;

	void Start ()
	{
		if(frames == null || frames.Length == 0)
		{
			Debug.LogError("No frames were passed");
		}

		nFrames = frames.Length;
		currentFrame = 0;

		//StartCoroutine(RunAnimation());
	}

	IEnumerator RunAnimation()
	{
		while(true)
		{
			for(int i = 0; i < nFrames; i++)
			{
				frames[i].SetActive(false);
			}
			frames[currentFrame].SetActive(true);

			currentFrame = (currentFrame + 1) % nFrames;

			yield return new WaitForSeconds(animationTime);
		}
	}

	public void NextFrame()
	{
		currentFrame = (currentFrame + 1) % nFrames;

		for(int i = 0; i < nFrames; i++)
		{
			frames[i].SetActive(false);
		}
		frames[currentFrame].SetActive(true);
	}
}
