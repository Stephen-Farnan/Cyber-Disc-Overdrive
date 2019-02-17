using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_EnemyHealthScript : MonoBehaviour
{

	public float maxHealth;
	private float currentHealth;

	void Awake() {
		currentHealth = maxHealth;
	}

	public void LoseHealth(float damage)
	{
		currentHealth -= damage;

		if (currentHealth <= 0)
		{
			Die();
			return;
		}
	}

	void Die()
	{
		GetComponent<CapsuleCollider>().enabled = false;
		GetComponent<MeshRenderer>().enabled = false;
		GetComponent<EnemyBehaviour>().Die();
	}
}
