using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour {

	public float fadeTime = 2;
	private bool fading = false;
	private Image fadeImage;

	void Start ()
	{
		fadeImage = GetComponent<Image>();
	}

	public float GetFadeTime() {
		return fadeTime;
	}

	public IEnumerator FadeIn()
	{
		if (fading)
		{
			yield break;
		}

		fading = true;
		Color col = fadeImage.color;
		while (fading)
		{
			col.a -= Time.deltaTime / fadeTime;
			fadeImage.color = col;
			if (col.a <= 0)
			{
				fading = false;
			}
			yield return null;
		}
	}

	public IEnumerator FadeOut()
	{
		if (fading)
		{
			yield break;
		}

		fading = true;
		Color col = fadeImage.color;
		while (fading)
		{
			col.a += Time.deltaTime / fadeTime;
			fadeImage.color = col;
			if (col.a >= 1)
			{
				fading = false;
			}
			yield return null;
		}
	}
}
