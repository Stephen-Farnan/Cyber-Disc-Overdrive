using System.Collections;
using UnityEngine;

public class Enemy_Attack_Specialist : MonoBehaviour
{

    public bool can_Parry = true;
    public bool disc_Is_In_Range;
    public float parry_Cooldown = 1.5f;

    private MouseShooting shootScript;

    void Start()
    {
        shootScript = GameObject.FindWithTag("Player").GetComponent<MouseShooting>();
    }

    /// <summary>
    /// Starts an attack and sets a target location
    /// </summary>
    public void Attack()
    {
        if (can_Parry)
        {
            StartCoroutine("Cooldown_From_Attack");
            //then parry the disc here and add the force, update the combo level of the disc
            Vector3 dir = Vector3.Normalize(Vector3.Scale(shootScript.transform.position - transform.position, new Vector3(1, 0, 1)));
            shootScript.SuccessfulParry(dir);
        }
    }

    /// <summary>
    /// Sets the enemy on cooldown before being able to attack again
    /// </summary>
    /// <returns></returns>
    IEnumerator Cooldown_From_Attack()
    {
        can_Parry = false;
        yield return new WaitForSeconds(parry_Cooldown);
        can_Parry = true;
        if (disc_Is_In_Range)
        {
            Attack();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Disc")
        {
            disc_Is_In_Range = true;
            Attack();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Disc")
        {
            disc_Is_In_Range = false;
        }
    }
}
