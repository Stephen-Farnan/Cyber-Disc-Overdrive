using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Abilities : MonoBehaviour
{

    public float ability_Cooldown = 2.5f;
    bool Can_Cast_Ability = true;
    public MouseShooting local_Mouse_Shooting;
    public SphereCollider Explosion_Radius_Col;
    public int explosion_Damage = 8;
    public GameObject explosion_Particle;

    public enum Ability_Type
    {
        DISC_EXPLOSION,
        DISC_STOPPER
    }

    public Ability_Type local_Ability_Type;

    public void Cast_Player_Ability()
    {
        if (Can_Cast_Ability && local_Mouse_Shooting.projectileFired)
        {
            Can_Cast_Ability = false;
            StartCoroutine("Cooldown_Ability");
            switch (local_Ability_Type)
            {
                case Ability_Type.DISC_EXPLOSION:
                    Disc_Explosion_Ability();

                    break;

                case Ability_Type.DISC_STOPPER:
                    Disc_Stopper_Ability();
                    break;
            }
        }
    }

    IEnumerator Cooldown_Ability()
    {

        yield return new WaitForSeconds(ability_Cooldown);
        Can_Cast_Ability = true;
    }

    void Disc_Explosion_Ability()
    {
        Explosion_Radius_Col.gameObject.SetActive(true);
        Collider[] hitColliders = Physics.OverlapSphere(Explosion_Radius_Col.transform.position, Explosion_Radius_Col.radius);
        int i = 0;
        StartCoroutine("Spawn_Explosion");
        while (i < hitColliders.Length)
        {
            if(hitColliders[i].gameObject.tag == "Enemy")
            {
                if(hitColliders[i].GetComponent<Enemy_AI>() != null)
                {
                    hitColliders[i].GetComponent<Enemy_AI>().Knockback(Explosion_Radius_Col.gameObject.transform.position);
                    hitColliders[i].GetComponent<HealthScript>().LoseHealth(explosion_Damage);

                    Debug.Log("Hit an Enemy");
                }

            }
            i++;
        }

        Explosion_Radius_Col.gameObject.SetActive(false);
    }

    void Disc_Stopper_Ability()
    {
        local_Mouse_Shooting.projectileRB.Sleep();
    }

    IEnumerator Spawn_Explosion()
    {
        explosion_Particle.transform.position = local_Mouse_Shooting.projectile.transform.position;
        explosion_Particle.SetActive(true);
        yield return new WaitForSeconds(0.45f);
        explosion_Particle.SetActive(false);
    }
}
