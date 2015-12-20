using UnityEngine;
using System.Collections;

public class AnimationEnd : MonoBehaviour
{
	public GameObject gameController;

	GameController gc;	

	void Start ()
	{
		float targetAspect = 16.0f / 9.0f;
		float windowAspect = (float)Screen.width / (float)Screen.height;
		float heightScale = windowAspect / targetAspect;

		Camera camera = GetComponent<Camera>();

		if(heightScale < 1.0f)
		{
			Rect rect = camera.rect;

			rect.width = 1.0f;
			rect.height = heightScale;
			rect.x = 0.0f;
			rect.y = (1.0f - heightScale) / 2.0f;

			camera.rect = rect;
		}
		else
		{
			float widthScale = 1.0f / heightScale;
			Rect rect = camera.rect;

			rect.width = widthScale;
			rect.height = 1.0f;
			rect.x = (1.0f - widthScale) / 2.0f;
			rect.y = 0.0f;

			camera.rect = rect;
		}

		gc = gameController.GetComponent<GameController>();
	}
	
	public void OnAnimationEnd()
	{
		gc.OnAnimationEnd();
	}
}
