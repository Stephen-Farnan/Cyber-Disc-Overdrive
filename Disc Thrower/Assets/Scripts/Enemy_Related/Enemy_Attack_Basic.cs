using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack_Basic : MonoBehaviour {

    public Enemy_AI local_Enemy_AI;

    public void Attack_Player()
    {
        StartCoroutine("Chase_Player");
        local_Enemy_AI.self_Navmesh_Agent.speed = local_Enemy_AI.movement_Speed;
    }



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


    IEnumerator Chase_Player()
    {
        while (true)
        {
            local_Enemy_AI.self_Navmesh_Agent.destination = local_Enemy_AI.player_Position.transform.position;
            yield return new WaitForSeconds(.002f);
        }

    }
}
