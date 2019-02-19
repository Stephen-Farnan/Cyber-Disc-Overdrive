using System.Collections;
using UnityEngine;

public class Enemy_Projectile : MonoBehaviour
{

    public int damage = 1;
    public float projectile_Speed;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Moves the gameobject this is attached to towards a given target
    /// </summary>
    /// <param name="target">destination to move towards over time</param>
    /// <returns></returns>
    IEnumerator Move_Towards_Target(Transform target)
    {
        Vector3 temp_Dir;
        temp_Dir = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.LookAt(temp_Dir);

        while (true)
        {
            rb.velocity = transform.forward * projectile_Speed * 100f;
            yield return new WaitForSeconds(.002f);
        }

    }

    /// <summary>
    /// Deals with collision with player or environment
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<CharacterNavMeshMovement>().TakeDamage(transform, damage);
            StopAllCoroutines();
            gameObject.SetActive(false);
        }

        else
        {
            StopAllCoroutines();
            gameObject.SetActive(false);

        }
    }
}
