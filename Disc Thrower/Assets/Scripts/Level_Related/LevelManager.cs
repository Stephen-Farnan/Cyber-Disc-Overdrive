using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

	public float baseChanceToSpawnHealthRegen = 20;
	[HideInInspector]
	public float chanceToSpawnHealthRegen;
	private int healthRegenSpawns = 0;

	private bool levelCompleted = false;
	private bool hasBossKey = false;

	public bool HasBossKey
	{
		get {
			return hasBossKey;
		}
		set {
			hasBossKey = value;
		}
	}

	private Text messageText;

	void Awake()
	{
		messageText = GameObject.Find("Message Text").GetComponent<Text>();
		chanceToSpawnHealthRegen = baseChanceToSpawnHealthRegen;
	}

	public IEnumerator DisplayMessage(string message) {
		messageText.text = message;
		messageText.enabled = true;

		yield return new WaitForSeconds(2);

		if (!levelCompleted)
		{
			messageText.enabled = false;
		}
	}

	public void AddHealthRegen() {
		healthRegenSpawns++;
		chanceToSpawnHealthRegen /= 2;
	}
}
