using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack_Ranged : MonoBehaviour {

    public Enemy_AI local_Enemy_AI;

    public int bullets_Per_Attack;
    public float time_Between_Bullets;
    public float bullet_Speed;
     int curr_Bullet = 0;
    bool shooting = false;

    public GameObject[] bullets;

    [SerializeField]
    private LayerMask ignore_Layers;


    /// <summary>
    /// Sends out a raycast to identify the player. If in range and in sight then fires projectiles at their current position
    /// </summary>
    public void Shoot_At_Player()
    {

        RaycastHit p_Hit;
        if (Physics.Raycast(transform.position, local_Enemy_AI.player_Position.transform.position - transform.position, out p_Hit, Mathf.Infinity, ignore_Layers))
        {
            if (p_Hit.collider.gameObject.layer == 14)
            {
                local_Enemy_AI.enemy_State = Enemy_AI.State.PICKING_LOCATION_TO_MOVE;
                local_Enemy_AI.StartCoroutine("Pick_Move_Location");
            }
            else
            {
                shooting = true;
                StartCoroutine("Turn_To_Face_Enemy");

                StartCoroutine("Wait_Between_Bullets");

                local_Enemy_AI.self_Navmesh_Agent.SetDestination(local_Enemy_AI.player_Position.transform.position);

            }
        }
    }

    /// <summary>
    /// Ensures the enemy is always accurately facing the player before firing
    /// </summary>
    /// <returns></returns>
    IEnumerator Turn_To_Face_Enemy()
    {


        while (shooting)
        {
            Vector3 direction = (local_Enemy_AI.player_Position.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * local_Enemy_AI.turn_Speed);
            yield return new WaitForSeconds(.002f);
        }

    }

    /// <summary>
    /// Pauses firing between shots based on the enemy attack speed
    /// </summary>
    /// <returns></returns>
    IEnumerator Wait_Between_Bullets()
    {
        yield return new WaitForSeconds(.25f);
        for(int i=0; i < bullets_Per_Attack; i++)
        {



            if (curr_Bullet >= bullets.Length)
            {
                curr_Bullet = 0;
            }

            //play a sound
            local_Enemy_AI.attack_SFX.Play();
            bullets[curr_Bullet].transform.position = transform.position;
          //  bullets[curr_Bullet].transform.LookAt(local_Enemy_AI.player_Position.transform);
            bullets[curr_Bullet].SetActive(true);
            bullets[curr_Bullet].GetComponent<Enemy_Projectile>().damage = local_Enemy_AI.attack_Damage;
            bullets[curr_Bullet].GetComponent<Enemy_Projectile>().projectile_Speed = bullet_Speed;
            bullets[curr_Bullet].GetComponent<Enemy_Projectile>().StartCoroutine("Move_Towards_Target", local_Enemy_AI.player_Position.transform);
            curr_Bullet++;
            yield return new WaitForSeconds(time_Between_Bullets);
        }
        shooting = false;
        local_Enemy_AI.enemy_State = Enemy_AI.State.RESTING;
        local_Enemy_AI.StartCoroutine("Rest", local_Enemy_AI.rest_Duration);

    }

    
}
