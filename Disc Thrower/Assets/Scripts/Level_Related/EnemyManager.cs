using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

	public GameObject[] enemyPrefabs;
	public int[] enemyValue;
	[Tooltip("Number of points to spend on enemies per room")]
	public int levelDifficulty;
	private int actualLevelDifficulty;

	void Awake() {
		actualLevelDifficulty = levelDifficulty * 9;
	}

	void Start() {
		Invoke("DebugDifficulty", 2);
	}

	public void EditActualLevelDifficulty (int x) {
		actualLevelDifficulty += x;
	}

	void DebugDifficulty() {
		Debug.Log("Level Difficulty: " + actualLevelDifficulty);
	}
}
