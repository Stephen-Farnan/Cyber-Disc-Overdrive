using System.Collections;
using UnityEngine;

public class Enemy_Attack_Crowd_Control : MonoBehaviour
{
    #region
    public float attack_Duration = 3f;
    public float stun_Duration = 2f;
    bool radius_Is_Growing;
    public bool have_Stunned = false;
    public bool outer_Ring_Triggered;
    public bool inner_Ring_Triggered;
    public ParticleSystem local_Particle_System;
    public SphereCollider outer_Collider;
    public SphereCollider inner_Collider;
    public Enemy_AI local_Enemy_AI;
    float inner_Sphere_Radius;
    float outer_Sphere_Radius;
    float particle_Sphere_Radius;
    public float growth_Amount = .2f;
    #endregion

    private void Start()
    {
        inner_Sphere_Radius = inner_Collider.radius;
        outer_Sphere_Radius = outer_Collider.radius;
        particle_Sphere_Radius = local_Particle_System.shape.radius;
    }

    /// <summary>
    /// Starts an attack and calls the stun radius to start expanding
    /// </summary>
    public void Attack()
    {

        radius_Is_Growing = true;
        local_Particle_System.Play();
        StartCoroutine("Move_Radius_Out");
        StartCoroutine("Reset_Radius");
    }

    /// <summary>
    /// Expands the radius of the stun collision to expand over time
    /// </summary>
    /// <returns></returns>
    IEnumerator Move_Radius_Out()
    {
        while (radius_Is_Growing)
        {
            //increase the radius' of both spheres
            inner_Collider.radius = inner_Collider.radius + (growth_Amount * .7f);
            outer_Collider.radius += growth_Amount;
            ParticleSystem new_Rad = local_Particle_System;
            ParticleSystem.ShapeModule shape = new_Rad.shape;
            shape.radius += (growth_Amount * 2f);
            //   new_Rad.shape.radius += growth_Amount;


            yield return new WaitForSeconds(.2f);
        }

    }

    /// <summary>
    /// Sets the radius of the stun collision back to its default state
    /// </summary>
    /// <returns></returns>
    IEnumerator Reset_Radius()
    {
        yield return new WaitForSeconds(attack_Duration);
        radius_Is_Growing = false;
        local_Particle_System.Stop();
        inner_Collider.radius = inner_Sphere_Radius;
        outer_Collider.radius = outer_Sphere_Radius;
        ParticleSystem new_Rad = local_Particle_System;
        ParticleSystem.ShapeModule shape = new_Rad.shape;
        shape.radius = particle_Sphere_Radius;
        local_Enemy_AI.enemy_State = Enemy_AI.State.RESTING;
        local_Enemy_AI.StartCoroutine("Rest");

    }

    private void OnTriggerEnter(Collider other)
    {
        outer_Ring_Triggered = true;
        if (outer_Ring_Triggered && !inner_Ring_Triggered)
        {
            if (other.gameObject.tag == "Player" && radius_Is_Growing)
            {
                if (!other.gameObject.GetComponent<CharacterNavMeshMovement>().isDashing)
                {
                    other.gameObject.GetComponent<Player_Input>().input_Enabled = false;
                    if (!have_Stunned)
                    {
                        have_Stunned = true;
                        other.gameObject.GetComponent<PlayerStatus>().StartStun(stun_Duration);
                        other.gameObject.GetComponent<Player_Input>().End_Stun_Public(stun_Duration, gameObject.GetComponent<Enemy_Attack_Crowd_Control>());
                    }
                }


            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            outer_Ring_Triggered = false;
        }
    }


}
