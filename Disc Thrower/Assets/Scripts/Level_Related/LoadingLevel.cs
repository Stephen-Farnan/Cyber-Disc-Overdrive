using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingLevel : MonoBehaviour {

	public LevelGenerator levGen;
	public Transform loadingBar;

	void Start() {
		StartCoroutine(StartLoading());
	}

	IEnumerator StartLoading() {
		yield return null;

		bool whileTrigger = true;
		float x = 0;

		loadingBar.parent.gameObject.SetActive(true);
		levGen.BeginProcess();

		while (whileTrigger)
		{
			x = levGen.totalNoOfRoomsGenerated / levGen.totalNoOfRooms;
			loadingBar.localScale = new Vector3(x, loadingBar.localScale.y, loadingBar.localScale.z);

			if (x >= 1)
			{
				whileTrigger = false;
			}
			yield return null;
		}

		yield return new WaitForSeconds(0.5f);

		loadingBar.parent.gameObject.SetActive(false);
	}
}
