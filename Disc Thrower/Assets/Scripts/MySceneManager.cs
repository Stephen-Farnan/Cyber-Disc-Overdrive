using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour {
	
	void Update () {
		if (DebugHandler.debugEnabled)
		{
			if (Input.GetKeyDown(KeyCode.Return))
			{
				RestartScene();
			}
		}
	}

	void RestartScene() {
		Time.timeScale = 1;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
