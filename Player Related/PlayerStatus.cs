using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{

    [HideInInspector]
    public bool isInvulnerable = false;
    public GameObject stunFX;
    public Transform characterMeshParentTransform;
    //public SkinnedMeshRenderer characterMeshRenderer;
    //private Material[] mats;
    private List<Material> mats = new List<Material>();
    private float invulnerabilityTimer = 0;
    public float flashSpeed = 1;
    public float phaseAmount = 1;
    public float phaseTime = 0.1f;
    private bool isPhasing = false;
    private MeshRenderer laserRend;

    void Awake()
    {
        laserRend = transform.Find("Laser Sight").GetComponent<MeshRenderer>();
        SkinnedMeshRenderer[] rends = characterMeshParentTransform.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer rend in rends)
        {
            for (int i = 0; i < rend.materials.Length; i++)
            {
                mats.Add(rend.materials[i]);
            }
        }
    }

    /*
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			StartCoroutine(Phase(true));
		}
		if (Input.GetKeyDown(KeyCode.U))
		{
			StartCoroutine(Phase(false));
		}
	}*/

    /// <summary>
    /// Sets the player as invulnerable after taking damage or while dashing
    /// </summary>
    /// <param name="invulnerabilityTime"></param>
    /// <returns></returns>
    public IEnumerator MakeInvulnerable(float invulnerabilityTime)
    {
        isInvulnerable = true;
        StartCoroutine(DamageFlash());
        while (invulnerabilityTimer < invulnerabilityTime)
        {
            invulnerabilityTimer += Time.deltaTime;
            yield return null;
        }
        isInvulnerable = false;
        invulnerabilityTimer = 0;
    }

    /// <summary>
    /// Sends visual feedback to the player when hit
    /// </summary>
    /// <returns></returns>
    IEnumerator DamageFlash()
    {
        while (isInvulnerable)
        {
            foreach (Material mat in mats)
            {
                mat.SetFloat("_altColourMult", (Mathf.PingPong(Time.time * flashSpeed, 1)));
            }
            yield return null;
        }
        foreach (Material mat in mats)
        {
            mat.SetFloat("_altColourMult", 0);
        }
    }

    public void SetPlayerMaterialAlpha(float alpha)
    {
        foreach (Material mat in mats)
        {
            //mat.SetFloat("_alpha", alpha);
        }
    }

    /// <summary>
    /// Called when the player dashes forward, to handle collisions and invulnerability status
    /// </summary>
    /// <param name="phaseOut"></param>
    /// <returns></returns>
    public IEnumerator Phase(bool phaseOut)
    {
        //if (isPhasing) yield break;

        float currentPhaseValue;
        isPhasing = true;

        //If phasing out
        if (phaseOut)
        {
            laserRend.enabled = false;
            currentPhaseValue = 0;
            while (currentPhaseValue < phaseAmount)
            {
                currentPhaseValue += Time.deltaTime / phaseTime;
                if (currentPhaseValue > phaseAmount) currentPhaseValue = phaseAmount;

                foreach (Material mat in mats)
                {
                    mat.SetFloat("_phaseAmount", currentPhaseValue);
                }
                yield return null;
            }
            isPhasing = false;
        }
        //If phasing in
        else
        {
            currentPhaseValue = phaseAmount;
            while (currentPhaseValue > 0)
            {
                currentPhaseValue -= Time.deltaTime / phaseTime;
                if (currentPhaseValue < 0) currentPhaseValue = 0;

                foreach (Material mat in mats)
                {
                    mat.SetFloat("_phaseAmount", currentPhaseValue);
                }
                yield return null;
            }
            isPhasing = false;
            laserRend.enabled = true;
        }
    }

    /// <summary>
    /// Called when the player gets stunned
    /// </summary>
    /// <param name="stun_duration"></param>
    public void StartStun(float stun_duration)
    {
        stunFX.SetActive(true);
        StartCoroutine(Stun_Duration(stun_duration));
    }

    IEnumerator Stun_Duration(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndStun();
    }

    public void EndStun()
    {
        stunFX.SetActive(false);
    }
}
