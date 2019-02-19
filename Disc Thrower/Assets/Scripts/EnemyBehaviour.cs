using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{

    private NavMeshAgent navAgent;
    private Transform target;
    public float baseDamage = 1;
    [HideInInspector]
    public RoomScript roomScript;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        target = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        navAgent.SetDestination(target.position);
    }

    /// <summary>
    /// Kills an enemy and updates the room script
    /// </summary>
    public void Die()
    {
        GetComponent<NavMeshAgent>().enabled = false;
        roomScript.EnemyKilled();
        this.enabled = false;
    }
}
