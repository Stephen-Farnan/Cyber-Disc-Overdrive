using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseShooting : MonoBehaviour {

	//Projectile/Firing Variables
	public GameObject projectile;
	public ParticleSystem parryParticleSystem;
    [HideInInspector]
    public Rigidbody projectileRB;
	public float projectileForce = 5;
	[HideInInspector]
	public bool projectileFired = false;
	[HideInInspector]
	public bool safetyBufferOn = true;
	[Tooltip("Amount of time that player is safe from being hit by the projectile after firing it")]
	public float safetyBufferTime = 0.2f;
	//Variables for make sure the player doesn't get hit by the disc when throwing
	private float safetyTimer;
	//private bool safetyTimerActive = false;
	private bool canFire = true;

	//Parrying Variables
	public int currentParryCombo = 0;
	public int maxParryCombo = 10;
	[Tooltip("How much will be added onto the speed of the projectile after each successive deflection")]
	public float parrySpeedMultiplier = 0.5f;
	[Tooltip("Amount of time after pressing Parry Button within which parrying will happen")]
	public float parryTime = 1;
	public float parryFireRate = 1;
	public float recallCooldown = 2;
	public GameObject recallEffectPrefab;
	[Tooltip("Radius within which the projectile must be to register deflection")]
	public float parryDistance = 1.5f;
	private float parryTimer = 0;
	//Using canParry for both parrying and catching
	[HideInInspector]
	public bool canParry = true;
	[Tooltip("The angle from forward (+ or -) that the disc must be within to parry")]
	public float maxParryAngle = 90;

	//Catching Variables
	//private float catchTimer = 0;

	//Misc
	public LayerMask parryLayerMask;
	private ProjectileCollision projCol;
	private Animator anim;
    public AudioSource parry_SFX;

	void Start() {
		projectileRB = projectile.GetComponent<Rigidbody>();
		projCol = projectile.GetComponent<ProjectileCollision>();
		anim = GetComponentInChildren<Animator>(false);
	}

	/*
	void Update() {
		if (Input.GetButtonDown("Fire1"))
		{
			ThrowCatch();
		}

		if (Input.GetButtonDown("Fire2"))
		{
			if (canParry)
			{
				StartParry();
			}
		}
	}*/

	public void ThrowCatch() {
		if (!projectileFired)
		{
			if (canFire)
			{
				FireProjectile();
				anim.SetTrigger("Throw");
			}
		}
		else if (canParry)
		{
			StartCoroutine(Recall());

			//Call recall effects here
		}
	}

	void FireProjectile() {
		canFire = false;
		StartCoroutine(TriggerParryCooldown());
		//ActivateSafetyBuffer();
		//StartCoroutine(DeactivateSafetyBuffer());
		projectile.transform.position = gameObject.transform.position + transform.forward * 2.0f + transform.up * 2;
		projectile.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
		projectile.GetComponent<CapsuleCollider>().enabled = true;
		projectile.GetComponent<TrailRenderer>().enabled = true;

		projCol.PlayThrowFX();

		float force;
		if (projectileFired)
		{
			force = CalculateProjectileSpeed();
		}
		else {
			force = projectileForce;
		}
		projectileRB.Sleep();
		projectileRB.AddForce((transform.forward * force), ForceMode.VelocityChange);

		projectileFired = true;
	}

	public float CalculateProjectileSpeed()
	{
		float newSpeed = projectileForce + ((parrySpeedMultiplier * currentParryCombo) * projectileForce);
		return newSpeed;
	}

	public float CalculateAngleToProjectile() {
		Vector3 toProjectile = projectile.transform.position - transform.position;
		float angle = Vector3.Angle(transform.forward, toProjectile);
		return angle;
	}

	public void StartParry() {
		if (canParry)
		{
			canParry = false;
			parryTimer = parryTime;
			StartCoroutine(HoldParry());

			anim.SetTrigger("Parry");
			parryParticleSystem.Play();
		}
	}

	IEnumerator HoldParry() {
		//Debug.Log("Hold Parry");
		while (parryTimer > 0) {
			int isProjectileInRange = Physics.OverlapSphere(transform.position, parryDistance, parryLayerMask).Length;
			if (isProjectileInRange > 0)
			{
				if (CalculateAngleToProjectile() < maxParryAngle)
				{
                    //PARRIED!

                    SuccessfulParry(transform.forward);
				}
			}
			parryTimer -= Time.deltaTime;
			yield return null;
		}
		StartCoroutine(TriggerParryCooldown());
		//Debug.Log("End Parry");
	}

	public void SuccessfulParry(Vector3 dir) {
        parry_SFX.Play();
        if (currentParryCombo < maxParryCombo)
		{
			currentParryCombo++;
			projCol.UpdateCurrentDamage(currentParryCombo);
		}
		projectileRB.Sleep();
		projectileRB.AddForce((dir * CalculateProjectileSpeed()), ForceMode.VelocityChange);
	}

	IEnumerator TriggerParryCooldown() {
		canParry = false;

		yield return new WaitForSeconds(parryFireRate);

		canParry = true;
		//Debug.Log("Can Parry");
	}

	//Catching
	/*
	void StartCatch() {
		canParry = false;
		catchTimer = parryTime;
		StartCoroutine(HoldCatch());
	}

	IEnumerator HoldCatch()
	{
		while (catchTimer > 0)
		{
			int isProjectileInRange = Physics.OverlapSphere(transform.position, parryDistance, parryLayerMask).Length;
			if (isProjectileInRange > 0)
			{
				//CAUGHT!
				currentParryCombo = 0;
				projCol.UpdateCurrentDamage(currentParryCombo);
				projectileFired = false;
				//ActivateSafetyBuffer();

				projectile.GetComponent<MeshRenderer>().enabled = false;
				projectile.GetComponent<CapsuleCollider>().enabled = false;
				projectile.GetComponent<TrailRenderer>().enabled = false;


				yield return new WaitForSeconds(parryFireRate);

				canParry = true;
				//Debug.Log("Catch Available");
				yield break;
			}
			catchTimer -= Time.deltaTime;
			yield return null;
		}
		canParry = true;
		//Debug.Log("Catch Available");
	}*/

	public IEnumerator Recall() {
		canFire = false;
		canParry = false;

		currentParryCombo = 0;
		projCol.UpdateCurrentDamage(currentParryCombo);
		//ActivateSafetyBuffer();

		projectile.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
		projectile.GetComponent<Rigidbody>().Sleep();
		projectile.GetComponent<CapsuleCollider>().enabled = false;
		projectile.GetComponent<TrailRenderer>().enabled = false;
		projCol.StopParticles();


		if (projectileFired)
		{
			DiscRecallEffect effect =
				Instantiate(recallEffectPrefab, projectile.transform.position, projectile.transform.rotation).GetComponent<DiscRecallEffect>();
			effect.SetPlayerTransform(transform);
		}

		projectileFired = false;

		yield return new WaitForSeconds(recallCooldown);

		canParry = true;
		canFire = true;
	}

	/*
	void ActivateSafetyBuffer() {
		//Put player on safety layer
		gameObject.layer = 11;
		safetyBufferOn = true;
	}

	IEnumerator DeactivateSafetyBuffer() {
		if (!safetyTimerActive)
		{
			safetyTimerActive = true;
			safetyTimer = safetyBufferTime;
			while (safetyTimer > 0)
			{
				safetyTimer -= Time.deltaTime;
				yield return null;
			}
			safetyBufferOn = false;
			//Put player on player layer
			gameObject.layer = 10;
			safetyTimerActive = false;
		}
	}

	void ResetSafetyTimer() {
		safetyTimer = safetyBufferTime;
	}*/
}
