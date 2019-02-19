using System.Collections;
using UnityEngine;

/// <summary>
/// This component script handles health management on whatever gameobject it is attached to
/// </summary>

public class HealthScript : MonoBehaviour
{

    public float maxHealth;
    public int currentHealth;
    public float invulnerabilityTime;
    public float enemyFlashSpeed = 1;
    public float enemyFlashTime = 2;
    private bool isInvulnerable = false;

    [Header("Audio")]
    private AudioSource audioSource;
    public AudioSource damage_SFX;
    [Tooltip("0: Hit, 1: Death")]
    public AudioClip[] audioClips;
    public Death_Manager local_Death_Manager;
    private Material mat;
    private Coroutine damageCoroutine;


    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (gameObject.tag == "Enemy")
        {
            mat = GetComponentsInChildren<MeshRenderer>()[1].material;
        }
    }

    //Attempting to get octagonal colliders working
    /*
	void OnCollisionEnter(Collision col)
	{
		if (col.collider.tag == "Disc")
		{
			ProjectileCollision projCol = col.collider.GetComponent<ProjectileCollision>();
			CharacterNavMeshMovement charController = projCol.GetCharController();
			if (gameObject.tag == "Player")
			{
				charController.TakeDamage(this.transform, 1);
			}

			if (gameObject.tag == "Enemy")
			{
				Debug.Log(gameObject.name);
				gameObject.GetComponent<HealthScript>().LoseHealth(projCol.GetCurrentDamage());
				charController.gameObject.GetComponent<Player_Score>().Add_To_Multiplier();
				charController.gameObject.GetComponent<Player_Score>().current_Total_Score = charController.gameObject.GetComponent<Player_Score>().current_Total_Score + (25 * charController.gameObject.GetComponent<Player_Score>().current_Multiplier);
				//add to the combo total here, display the individual score over the enemy
			}
		}
	}*/

    public void LoseHealth(int damage)
    {
        if (gameObject.tag == "Player")
        {
            if (gameObject.GetComponent<PlayerStatus>().isInvulnerable)
            {
                return;
            }

        }
        if (isInvulnerable)
        {
            return;
        }

        //isInvulnerable = true;
        currentHealth -= damage;
        PlayAudioClip(0);
        if (damage_SFX != null)
        {
            damage_SFX.Play();
        }

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        //If this script is attached to the player character, enable invulnerability
        if (gameObject.tag == "Player")
        {
            StartCoroutine(GetComponent<PlayerStatus>().MakeInvulnerable(invulnerabilityTime));
            gameObject.GetComponent<Player_Score>().Reset_Mutliplier();
            gameObject.GetComponent<Player_UI>().Update_Health(currentHealth);

        }

        if (gameObject.tag == "Enemy")
        {
            isInvulnerable = true;
            if (damageCoroutine != null) StopCoroutine(damageCoroutine);
            damageCoroutine = StartCoroutine(DamageFlash());
            Invoke("EndInvulnerability", invulnerabilityTime);
        }
    }

    /// <summary>
    /// Visual feedback on damage taken
    /// </summary>
    /// <returns></returns>
    IEnumerator DamageFlash()
    {
        float timer = enemyFlashTime;
        while (timer > 0)
        {
            mat.SetFloat("_damageSlider", (Mathf.PingPong(Time.time * enemyFlashSpeed, 1)));
            timer -= Time.deltaTime;
            yield return null;
        }
        mat.SetFloat("_damageSlider", 0);
    }

    /// <summary>
    /// Fades enemies out over time after they are killed
    /// </summary>
    /// <returns></returns>
    IEnumerator DeathPhaseOut()
    {
        float deathTime = 0.5f;
        float timer = deathTime;
        while (timer > 0)
        {
            mat.SetFloat("_deathSlider", 1 - timer / deathTime);
            timer -= Time.deltaTime;
            yield return null;
        }
        mat.SetFloat("_deathSlider", 1);
    }

    void EndInvulnerability()
    {
        isInvulnerable = false;
    }

    /// <summary>
    /// Increases Health based on passed in amount
    /// </summary>
    /// <param name="amount">Amount to increase health by</param>
    public void RegainHealth(int amount)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += amount;
            if (gameObject.tag == "Player")
            {
                gameObject.GetComponent<Player_UI>().Update_Health(currentHealth);
            }
        }
    }

    void Die()
    {
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        if (gameObject.tag == "Player")
        {
            GetComponent<MouseShooting>().enabled = false;
            GetComponent<CharacterNavMeshMovement>().enabled = false;
            local_Death_Manager.StartCoroutine("End_Level");
        }
        if (gameObject.tag == "Enemy")
        {
            StartCoroutine(DeathPhaseOut());
            GetComponent<Enemy_AI>().StartCoroutine("Die");
        }

        PlayAudioClip(1);
    }

    void PlayAudioClip(int clipNo)
    {
        audioSource.clip = audioClips[clipNo];
        audioSource.Play();
    }
}
