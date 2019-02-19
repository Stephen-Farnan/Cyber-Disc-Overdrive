using System.Collections;
using UnityEngine;

public class Enemy_Attack_Charger : MonoBehaviour
{

    public GameObject player_Position;
    public Enemy_AI local_Enemy_Ai;
    public float charge_Accuracy;
    public float charge_Speed = 35f;
    public float delay_Before_Charging = .5f;
    bool charging = false;
    bool turning = false;

    [SerializeField]
    public LayerMask ignore_Layers;

    void Start()
    {
        player_Position = GameObject.FindWithTag("Player");
    }

    /// <summary>
    /// If the player is in range and in sight then calls the enemy to start moving towards them
    /// </summary>
    public void Charge_At_Player()
    {
        RaycastHit p_Hit;
        if (Physics.Raycast(transform.position, player_Position.transform.position - transform.position, out p_Hit, Mathf.Infinity, ignore_Layers))
        {
            if (p_Hit.collider.gameObject.layer == 14)
            {
                StopCoroutine("Dash");

                local_Enemy_Ai.StartCoroutine("Pick_Move_Location");

            }
            else if (p_Hit.collider.gameObject.layer == 10)
            {
                StartCoroutine("Dash");

            }
        }

    }

    /// <summary>
    /// Stops the enemy turning as the charge starts
    /// </summary>
    /// <returns></returns>
    IEnumerator Stop_Turning()
    {
        yield return new WaitForSeconds(delay_Before_Charging);
        turning = false;
    }

    /// <summary>
    /// Handles the movement of the enemy towards the player position
    /// </summary>
    /// <returns></returns>
    IEnumerator Dash()
    {
        turning = true;
        StartCoroutine("Stop_Turning");
        while (turning)
        {
            Vector3 direction = (local_Enemy_Ai.player_Position.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * local_Enemy_Ai.turn_Speed);

            yield return new WaitForSeconds(.002f);
        }
        Set_Speed();

        yield return new WaitForSeconds(.02f);
        while (charging)
        {

            if (local_Enemy_Ai.self_Navmesh_Agent.remainingDistance < 0.2f)
            {
                charging = false;
                local_Enemy_Ai.self_Navmesh_Agent.speed = local_Enemy_Ai.movement_Speed;
                local_Enemy_Ai.attack_SFX.Stop();
                local_Enemy_Ai.enemy_State = Enemy_AI.State.RESTING;
                local_Enemy_Ai.StartCoroutine("Rest", local_Enemy_Ai.rest_Duration);

                break;
            }
            yield return new WaitForSeconds(.02f);
        }

        Vector3 temp_Destination = player_Position.transform.position;
    }

    /// <summary>
    /// Intializes the speed and base values for the enemy when it starts to charge
    /// </summary>
    void Set_Speed()
    {
        local_Enemy_Ai.self_Navmesh_Agent.speed = charge_Speed;
        local_Enemy_Ai.self_Navmesh_Agent.SetDestination(player_Position.transform.position);
        charging = true;
        local_Enemy_Ai.attack_SFX.Play();

    }





}
