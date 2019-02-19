using UnityEngine;
using UnityEngine.AI;


public class Player_Augments : MonoBehaviour
{
    #region properties
    public CharacterNavMeshMovement local_CharacterNavMeshMovement;
    public NavMeshAgent local_Nav_Agent;
    public HealthScript local_HealthScript;
    public GameObject defensive_Drones;

    public float moveSpeed_Increase_Amount = 5f;
    public int health_Regain_Amount = 1;

    public enum Player_Augments_List
    {
        INCREASE_HEALTH,
        EXTRA_MOVESPEED,
        DEFENSIVE_DRONES
    }

    public Player_Augments_List local_Player_Augments_List;
    #endregion

    /// <summary>
    /// Adds specified augment to the player
    /// </summary>
    /// <param name="selected_Augment">Augment to add to the player</param>
    public void Add_Augment_To_Player(Player_Augments_List selected_Augment)
    {
        switch (selected_Augment)
        {
            case Player_Augments_List.DEFENSIVE_DRONES:
                Add_Defensive_Drones();
                break;
            case Player_Augments_List.EXTRA_MOVESPEED:
                Add_Extra_Movespeed();
                break;
            case Player_Augments_List.INCREASE_HEALTH:
                Add_Increase_Health();
                break;

        }
    }


    void Add_Defensive_Drones()
    {
        Debug.Log("added drones");
        defensive_Drones.transform.position = transform.position;
        defensive_Drones.SetActive(true);
    }

    void Add_Extra_Movespeed()
    {
        local_CharacterNavMeshMovement.targetSpeed += moveSpeed_Increase_Amount;
        local_Nav_Agent.speed += moveSpeed_Increase_Amount;
    }

    void Add_Increase_Health()
    {

        if (local_HealthScript.currentHealth < local_HealthScript.maxHealth)
        {
            local_HealthScript.RegainHealth(health_Regain_Amount);
        }

        else
        {
            local_HealthScript.maxHealth++;
            local_HealthScript.RegainHealth(health_Regain_Amount);
        }
    }
}
