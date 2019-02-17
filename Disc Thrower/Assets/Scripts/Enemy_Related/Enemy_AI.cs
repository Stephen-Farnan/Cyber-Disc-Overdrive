using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy_AI : MonoBehaviour
{

    public float attack_Cooldown = 1f;
    public float rest_Duration = 1.5f;
    public float spawn_Duration = 1f;
    float movement_Smoothing = .002f;
    public bool can_Move_And_Attack = false;
    public bool aggressive = false;
    public float max_Movement_Range;
    public float min_Movement_Range;
    public float max_Range_Distance_To_Player = 15f;
    public float movement_Speed = 5f;
    public int contact_Damage = 1;
    public int attack_Damage = 1;
    public float turn_Speed = 1f;
    public int score_Worth = 100;
    public GameObject player_Position;
    public Enemy_Attack_Charger local_Enemy_Attack_Charger;
    public Enemy_Attack_Ranged local_Enemy_Attack_Ranged;
    public Enemy_Attack_Basic local_Enemy_Basic_Attack;
    public Enemy_Attack_Support local_Enemy_Attack_Support;
    public Enemy_Attack_Crowd_Control local_Enemy_Attack_Crowd_Control;
    public Enemy_Attack_Specialist local_Enemy_Attack_Specialist;
    public Multi_Disc_Boss_Manager local_Multi_Disc_Boss_Attack;
    public NavMeshAgent self_Navmesh_Agent;
    public AudioSource attack_SFX;
    public GameObject score_Text;
	public GameObject deathParticle;
    public GameObject support_Shield;
    public bool is_Shielded;
    Rigidbody rb;

    bool attack_Available = true;
    private float destination_Proximity_Amount = 2f;
    public GameObject move_Destination;
    public Transform temp_Rotation_Transform;
    bool is_At_Destination = true;
    bool moving = false;

    [SerializeField]
    private LayerMask ignore_Layers;



    [HideInInspector]
	public RoomScript roomScript;

    public enum enemy_Type
    {
        RANGED,
        CHARGER,
        BASIC,
        SUPPORT,
        CROWD_CONTROL,
        SPECIALIST,
        MULTI_DISC_BOSS
    }

    public enemy_Type local_Enemy_Type;

    public enum State
    {
        SPAWNING,
        PICKING_LOCATION_TO_MOVE,
        MOVING,
        ATTACKING,
        MOVING_AND_ATTACKING,
        RESTING
    }

    public State enemy_State;

    private void Start()
    {
        StartCoroutine("Check_State");
		player_Position = GameObject.FindWithTag("Player");
        score_Text = GameObject.FindGameObjectWithTag("Score_Text");
        rb = gameObject.GetComponent<Rigidbody>();
        destination_Proximity_Amount = .2f;

    }



    void Check_State()
    {

        switch (enemy_State)
        {
            case State.SPAWNING:
                StartCoroutine("Initialise_At_Spawn");
                break;

            case State.PICKING_LOCATION_TO_MOVE:
                StartCoroutine(Pick_Move_Location());
                break;

            case State.MOVING:
                if (!moving)
                {
                    StartCoroutine("Move_To_Destination");
                }
                if (is_At_Destination)
                {
                    enemy_State = State.ATTACKING;
                    StartCoroutine("Attack");
                }
                break;

            case State.ATTACKING:
                if (attack_Available)
                {
                    StartCoroutine("Attack");

                }
                break;

            case State.MOVING_AND_ATTACKING:

                break;

            case State.RESTING:

                break;
        }
        // yield return new WaitForSeconds(decision_Making_Wait_Time);
    }

    IEnumerator Initialise_At_Spawn()
    {
        //play particle effect for spawning in and possibly a startup animation
        //delay and then set the state to be another


        yield return new WaitForSeconds(spawn_Duration);
        enemy_State = State.PICKING_LOCATION_TO_MOVE;
        StartCoroutine(Pick_Move_Location());
    }

    IEnumerator Attack()
    {
        Debug.Log("here");
        if (attack_Available)
        {
            Debug.Log("attack");
            switch (local_Enemy_Type)
            {
                case enemy_Type.CHARGER:
                    local_Enemy_Attack_Charger.Charge_At_Player();
                    break;

                case enemy_Type.RANGED:
                    local_Enemy_Attack_Ranged.Shoot_At_Player();
                    break;

                case enemy_Type.BASIC:
                    local_Enemy_Basic_Attack.Attack_Player();
                    break;

                case enemy_Type.SUPPORT:
                    local_Enemy_Attack_Support.Attack();
                    break;

                case enemy_Type.CROWD_CONTROL:
                    local_Enemy_Attack_Crowd_Control.Attack();
                    break;

                case enemy_Type.SPECIALIST:
                    local_Enemy_Attack_Specialist.Attack();
                    break;

                case enemy_Type.MULTI_DISC_BOSS:
                    local_Multi_Disc_Boss_Attack.Attack();
                    break;
            }
            attack_Available = false;
            enemy_State = State.ATTACKING;
            //if applicabale, go to rest state
            yield return new WaitForSeconds(attack_Cooldown);
            attack_Available = true;
        }

        else
        {
            StartCoroutine("Pick_Move_Location");
        }
        
    }


    IEnumerator Pick_Move_Location()
    {
        enemy_State = State.PICKING_LOCATION_TO_MOVE;
        bool found_Valid_Location = false;
        int layerMask_For_Enemies = ~(1 << 12);
        RaycastHit hit;
        RaycastHit p_Hit;
        //calculate valid position to move and set move_Destination to it, then change state


        while (!found_Valid_Location)
        {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 

            temp_Rotation_Transform.position = transform.position;
            Vector3 euler = temp_Rotation_Transform.eulerAngles;
            temp_Rotation_Transform.rotation = Quaternion.Euler(0f, (System.DateTime.Now.Second * (Random.Range(0, 1000))) % 360, 0f);

            float movement_Amount = Random.Range(min_Movement_Range, max_Movement_Range);

            temp_Rotation_Transform.Translate(Vector3.forward * movement_Amount);
            move_Destination.transform.position = temp_Rotation_Transform.position;

            //validates the position found
            if (!Physics.Raycast(transform.position, (move_Destination.transform.position - transform.position), out hit, Vector3.Distance(transform.position, move_Destination.transform.position), ignore_Layers))
            {
           //     if(hit.collider.gameObject.layer != 14)
               // {
                    NavMeshHit w_hit;
                    if (NavMesh.FindClosestEdge(move_Destination.transform.position, out w_hit, NavMesh.AllAreas))
                    {
                        if (w_hit.distance > 3f)
                        {
                            if (Vector3.Distance(player_Position.transform.position, move_Destination.transform.position) < Vector3.Distance(player_Position.transform.position, transform.position))
                            {
                                if (!Physics.Raycast(transform.position, (move_Destination.transform.position - transform.position), out hit, Vector3.Distance(transform.position, move_Destination.transform.position) + 2f, ignore_Layers))
                                {

                                    Debug.Log("1");
                                    found_Valid_Location = true;
                                    //Debug.Log(hit.collider.gameObject);
                                    StartCoroutine("Move_To_Destination");
                                    break;


                                }


                                else
                                {
                                    Debug.Log("2");
                                    found_Valid_Location = true;
                                    //Debug.Log(hit.collider.gameObject);
                                    StartCoroutine("Move_To_Destination");
                                    break;
                                }

                            }
                        }

                    }
               // }
                
               
            }



            yield return new WaitForSeconds(.02f);
        }


        is_At_Destination = false;



        StartCoroutine("Check_State");


    }

    IEnumerator Movement_Limiter()
    {
        yield return new WaitForSeconds(4f);
        StopCoroutine("Move_To_Destination");
        StartCoroutine("Pick_Move_Location");
    }

    IEnumerator Move_To_Destination()
    {
        StartCoroutine("Movement_Limiter");
        self_Navmesh_Agent.speed = movement_Speed;
        enemy_State = State.MOVING;
        moving = true;
        is_At_Destination = false; 
        if (self_Navmesh_Agent.isActiveAndEnabled)
        {
 
            self_Navmesh_Agent.SetDestination(move_Destination.transform.position);
            self_Navmesh_Agent.speed = movement_Speed;
            while (!is_At_Destination)
            {
                //move a bit, yield, and loop again until destination reached
                if (Vector3.Distance(transform.position, move_Destination.transform.position) < destination_Proximity_Amount)
                {
                    Debug.Log("here");
                    is_At_Destination = true;
                    moving = false;
                    self_Navmesh_Agent.speed = 0f;
                    StopCoroutine("Movement_Limiter");
                    StartCoroutine("Attack");


                    //stop moving here
                }

                yield return new WaitForSeconds(movement_Smoothing);
            }


            
        }


    }

    IEnumerator Rest()
    {
        //stay in rest state for a time
        yield return new WaitForSeconds(rest_Duration);
        if (aggressive)
        {
            enemy_State = State.ATTACKING;
            switch (local_Enemy_Type)
            {
                case enemy_Type.CHARGER:
                    local_Enemy_Attack_Charger.Charge_At_Player();
                    break;

                case enemy_Type.RANGED:
                    local_Enemy_Attack_Ranged.Shoot_At_Player();
                    break;

                case enemy_Type.BASIC:
                    local_Enemy_Basic_Attack.Attack_Player();
                    break;

                case enemy_Type.SUPPORT:
                    
                    break;

                case enemy_Type.CROWD_CONTROL:
                    
                    break;

                case enemy_Type.SPECIALIST:
                    
                    break;
            }
        }

        else
        {
            enemy_State = State.PICKING_LOCATION_TO_MOVE;
            StartCoroutine(Pick_Move_Location());
        }
    }

    void Despawn_Enemy()
    {
        StopCoroutine("Move_To_Destination");
        GetComponent<NavMeshAgent>().enabled = false;
        if(roomScript != null) roomScript.EnemyKilled();
        if (local_Enemy_Type == enemy_Type.CHARGER)
        {
            local_Enemy_Attack_Charger.StopAllCoroutines();
        }
        else if (local_Enemy_Type == enemy_Type.RANGED)
        {
            local_Enemy_Attack_Ranged.StopAllCoroutines();
        }

        else if(local_Enemy_Type == enemy_Type.SUPPORT)
        {

        }

        else if (local_Enemy_Type == enemy_Type.CROWD_CONTROL)
        {

        }

        else if (local_Enemy_Type == enemy_Type.SPECIALIST)
        {

        }

        else
        {
            local_Enemy_Basic_Attack.StopAllCoroutines();
        }

        score_Text.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 3.8f, gameObject.transform.position.z);
        score_Text.SetActive(true);
        //need to redo this properly later
        int y = (player_Position.gameObject.GetComponent<Player_Score>().Add_To_Combo_Score(player_Position.gameObject.GetComponent<MouseShooting>().currentParryCombo + 1) * score_Worth * player_Position.gameObject.GetComponent<Player_Score>().current_Multiplier);
        score_Text.gameObject.GetComponent<Text>().text = y.ToString();
        player_Position.gameObject.GetComponent<Player_Score>().Add_To_Combo_Score(y);
        self_Navmesh_Agent.enabled = false;

		Instantiate(deathParticle, transform.position, transform.rotation);
    }

    public IEnumerator Die()
    {

        Despawn_Enemy();
        yield return new WaitForSeconds(.5f);



        score_Text.transform.position = new Vector3(2615f, 3.8f, 0f);


        //add to the temp enemies killed in the score system here*-
        gameObject.transform.parent.gameObject.SetActive(false);

        StopAllCoroutines();



    }

    public void Knockback(Vector3 t)
    {

        StartCoroutine("Wait_For_Knockback", t);
    }

    IEnumerator Wait_For_Knockback(Vector3 t)
    {
        yield return new WaitForSeconds(.005f);
        self_Navmesh_Agent.speed = 0f;
        //self_Navmesh_Agent.velocity = new Vector3(0,0,0);
        //self_Navmesh_Agent.enabled = false;

        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        Vector3 tranformDir = t - transform.position;
        rb.AddForce(tranformDir.normalized * -20f, ForceMode.Impulse);
        yield return new WaitForSeconds(.05f);
        self_Navmesh_Agent.speed = movement_Speed;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void Turn_On_Shield_From_Support()
    {
        //shield is turned on, start coroutine to turn it off
        support_Shield.SetActive(true);
        is_Shielded = true;
        StartCoroutine("Turn_Off_Shield");
    }

    IEnumerator Turn_Off_Shield()
    {
        yield return new WaitForSeconds(3f);
        support_Shield.SetActive(false);
        is_Shielded = false;
    }

    public void Heal_From_Support()
    {
        //turn on healing from support for a couple seconds
        Debug.Log("healed");
    }
}
