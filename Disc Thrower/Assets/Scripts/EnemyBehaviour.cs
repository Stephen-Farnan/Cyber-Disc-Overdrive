using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour {

	private NavMeshAgent navAgent;
	private Transform target;
	public float baseDamage = 1;
	[HideInInspector]
	public RoomScript roomScript;

	void Start () {
		navAgent = GetComponent<NavMeshAgent>();
		target = GameObject.FindWithTag("Player").transform;
	}
	
	void Update () {
		navAgent.SetDestination(target.position);
	}

	public void Die()
	{
		GetComponent<NavMeshAgent>().enabled = false;
		roomScript.EnemyKilled();
		this.enabled = false;
	}
}
