using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollision : MonoBehaviour {

	private CharacterNavMeshMovement charController;
	private MouseShooting shootScript;
	private Rigidbody rb;

	public int baseDamage = 1;
	private int currentDamage;
	public int parryDamageMultiplier = 1;
	public GameObject hitEffect;
	public float effectSpawnDistance = 1;
    public Laser_Boss_AI local_Laser_Boss_AI;
    public Multi_Disc_Boss_Manager local_Multi_Boss;
	private ParticleSystem comboParticles;

	[Header("Audio")]
	[Tooltip("0: Throw, 1: Wall Hit")]
	public AudioClip[] audioClips;
	[HideInInspector]
	public AudioSource audioSource;

	//private Material mat;

	void Awake() {
		charController = GameObject.FindWithTag("Player").GetComponent<CharacterNavMeshMovement>();
		shootScript = charController.GetComponent<MouseShooting>();
		rb = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
		//mat = GetComponent<MeshRenderer>().material;
		currentDamage = baseDamage;
		comboParticles = GetComponentInChildren<ParticleSystem>();
	}

	public CharacterNavMeshMovement GetCharController() {
		return charController;
	}

	public int GetCurrentDamage() {
		return currentDamage;
	}

	void OnCollisionEnter(Collision col) {

		Collider coll = col.collider;

		if (coll.tag == "Player")
		{
			charController.TakeDamage(this.transform, 1);
		}

		if (coll.tag == "Enemy")
		{


            coll.GetComponent<Enemy_AI>().Knockback(gameObject.transform.position);
            coll.GetComponent<HealthScript>().LoseHealth(currentDamage);

			charController.gameObject.GetComponent<Player_Score>().Add_To_Multiplier();
			charController.gameObject.GetComponent<Player_Score>().current_Total_Score = charController.gameObject.GetComponent<Player_Score>().current_Total_Score + (25 * charController.gameObject.GetComponent<Player_Score>().current_Multiplier);
			//add to the combo total here, display the individual score over the enemy
		}

        //boss specific collisions

        if(coll.tag == "Laser_Boss_Main_Part")
        {
            local_Laser_Boss_AI.Take_Damage(currentDamage, 0);
        }

        if (coll.tag == "Laser_Boss_First_Part")
        {
            local_Laser_Boss_AI.Take_Damage(currentDamage, 1);

        }

        if (coll.tag == "Laser_Boss_Second_Part")
        {
            local_Laser_Boss_AI.Take_Damage(currentDamage, 2);

        }

        if(coll.tag == "Multi_Disc_Boss")
        {
            local_Multi_Boss.Take_Damage(currentDamage);
        }

        if (col.collider.gameObject.layer  == LayerMask.NameToLayer("Environment"))
        {
            //add combo score here if any, to the total, reset combo
            charController.gameObject.GetComponent<Player_Score>().Add_To_Total_Score();

			//Getting point of collision and normal direction
			Vector3 point = col.contacts[0].point;
			Vector3 dir = col.contacts[0].normal;
			SpawnHitEffect(point, dir);
		}

        if(coll.tag == "Shield")
        {
            if (coll.GetComponentInParent<Enemy_AI>().is_Shielded)
            {
                coll.GetComponentInParent<Enemy_AI>().support_Shield.SetActive(false);
            }
        }

		PlayWallHitSound();

		//Ensuring the disc doesn't change speed by multiplying current direction by desired magnitude
		FixSpeedAndDirection();
	}

	void FixSpeedAndDirection() {
		//Desired speed
		float oldSpeed = shootScript.CalculateProjectileSpeed();
		//If disc is moving parallel next to a wall, move slightly away
		if (Physics.Raycast(transform.position, transform.right, 0.3f))
		{
			rb.AddForce(transform.right * (-0.1f));
		}
		if (Physics.Raycast(transform.position, transform.right * -1, 0.3f))
		{
			rb.AddForce(transform.right * 0.1f);
		}
		//Setting speed to desiored speed
		rb.velocity = rb.velocity.normalized * oldSpeed;
	}

	//Setting disc damage based on current parry combo
	public void UpdateCurrentDamage(int currentParryCombo) {
		currentDamage = baseDamage + ((currentParryCombo * parryDamageMultiplier) * baseDamage);

		//Updating particles
		//comboParticles.Stop();
		var em = comboParticles.emission;
		em.SetBurst (0, new ParticleSystem.Burst (0, currentParryCombo + 1));
		//comboParticles.Play();
	}

	void PlayWallHitSound() {
		audioSource.clip = audioClips[1];
		audioSource.Play();
	}

	public void PlayThrowFX() {
		audioSource.clip = audioClips[0];
		audioSource.Play();

		comboParticles.Play();
	}

	void SpawnHitEffect(Vector3 point, Vector3 dir) {
		Vector3 spawnPoint = point + dir * effectSpawnDistance;
		GameObject effect = Instantiate(hitEffect, spawnPoint, Quaternion.LookRotation(Vector3.up, dir));
	}

	public void RecallDisc() {
		StartCoroutine (shootScript.Recall());
	}

	public void StopParticles() {
		comboParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}
}
