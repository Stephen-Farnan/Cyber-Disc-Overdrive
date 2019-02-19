using System.Collections;
using UnityEngine;

public class Enemy_Attack_Basic : MonoBehaviour
{

    public Enemy_AI local_Enemy_AI;

    /// <summary>
    /// Starts the call to move towards the player and sets the new speed
    /// </summary>
    public void Attack_Player()
    {
        StartCoroutine("Chase_Player");
        local_Enemy_AI.self_Navmesh_Agent.speed = local_Enemy_AI.movement_Speed;
    }



    /// <summary>
    /// On collision with the player deal damage
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            local_Enemy_AI.enemy_State = Enemy_AI.State.RESTING;
            local_Enemy_AI.self_Navmesh_Agent.speed = 0f;
            StopAllCoroutines();
            local_Enemy_AI.StartCoroutine("Rest", local_Enemy_AI.rest_Duration);

        }
    }


    /// <summary>
    /// Over time move towards the current player location
    /// </summary>
    /// <returns></returns>
    IEnumerator Chase_Player()
    {
        while (true)
        {
            local_Enemy_AI.self_Navmesh_Agent.destination = local_Enemy_AI.player_Position.transform.position;
            yield return new WaitForSeconds(.002f);
        }

    }
}
