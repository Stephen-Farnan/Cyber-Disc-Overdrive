using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRegenPickup : MonoBehaviour {

	public int regenAmount = 1;
	public GameObject healthParticleEffect;
	private AudioSource audSource;

	void Start () {
		audSource = GetComponent<AudioSource>();
	}
	
	void OnTriggerEnter (Collider col) {
		if (col.tag == "Player")
		{
			HealthScript hlthScript = col.GetComponent<HealthScript>();
			if (hlthScript.currentHealth < hlthScript.maxHealth)
			{
				hlthScript.RegainHealth(regenAmount);
				GetComponent<SphereCollider>().enabled = false;
				GetComponentInChildren<MeshRenderer>().enabled = false;
				GetComponentInChildren<SpriteRenderer>().enabled = false;
				//When nested prefabs come in, this can be replaced
				Instantiate(healthParticleEffect, hlthScript.transform.position, Quaternion.identity, hlthScript.transform);
				audSource.Play();
				Destroy(gameObject, audSource.clip.length + 0.3f);
			}
		}
	}
}
