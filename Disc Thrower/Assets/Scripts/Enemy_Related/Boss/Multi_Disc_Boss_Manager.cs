using System.Collections;
using UnityEngine;

public class Multi_Disc_Boss_Manager : MonoBehaviour
{

    public bool all_Discs_Are_Destroyed = false;
    public float wait_Time_To_Reset_Discs = 5f;
    public int number_Of_Discs = 6;
    public Enemy_AI local_Enemy_Ai;
    public int curr_Number_Of_Discs = 5;
    public int max_Number_Of_Discs = 5;
    //this number can be increased as the fight goes on

    public HealthScript local_Healthscript;


    /// <summary>
    /// check all the discs have been destroyed, if they have, fire them out again in a certain pattern
    /// </summary>
    public void Attack()
    {
        if (all_Discs_Are_Destroyed)
        {
            //start attack phase coroutine, then alter the state in Enemy AI
            StartCoroutine(Fire_Discs());
        }

        else
        {
            //do nothing and move onto the next target location and update Enemy Ai
            local_Enemy_Ai.enemy_State = Enemy_AI.State.PICKING_LOCATION_TO_MOVE;
            local_Enemy_Ai.StartCoroutine("Rest");
        }
    }

    /// <summary>
    /// Recast the discs again once the player has destroyed the current ones
    /// </summary>
    /// <returns></returns>
    public IEnumerator Reset_Discs()
    {
        yield return new WaitForSeconds(wait_Time_To_Reset_Discs);
        all_Discs_Are_Destroyed = false;
        curr_Number_Of_Discs = max_Number_Of_Discs;
    }

    /// <summary>
    /// Fire out all of the discs avaialble to the boss and set them as active for collisions
    /// </summary>
    /// <returns></returns>
    public IEnumerator Fire_Discs()
    {
        int remaining_Discs = max_Number_Of_Discs;
        while (remaining_Discs > 0)
        {
            //turn on a disc
            all_Discs_Are_Destroyed = false;
            remaining_Discs--;
            yield return new WaitForSeconds(.2f);
        }

        //fire discs out at fire rate, then wait, and rest
        yield return new WaitForSeconds(1f);
        local_Enemy_Ai.enemy_State = Enemy_AI.State.PICKING_LOCATION_TO_MOVE;
        local_Enemy_Ai.StartCoroutine("Rest");

    }


    /// <summary>
    /// Handles taking damage from the player
    /// </summary>
    /// <param name="amount">The amount of damage to be taken</param>
    public void Take_Damage(int amount)
    {
        local_Healthscript.LoseHealth(amount);
    }
}
