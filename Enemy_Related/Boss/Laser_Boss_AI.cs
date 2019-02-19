using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Laser_Boss_AI : MonoBehaviour
{
    #region properties
    public enum Grid_Position
    {
        A1, A2, A3, A4, A5,
        B1, B2, B3, B4, B5,
        C1, C2, C3, C4, C5,
        D1, D2, D3, D4, D5,
        E1, E2, E3, E4, E5
    }

    public Grid_Position Main_Boss_Position;
    public Grid_Position first_Part_Position;
    public Grid_Position second_Part_Position;

    public Grid_Position Main_Boss_Target_Position;
    public Grid_Position first_Part_Target_Position;
    public Grid_Position second_Part_Target_Position;

    public NavMeshAgent main_Boss_Nav_Agent;
    public NavMeshAgent first_Part_Nav_Agent;
    public NavMeshAgent second_Part_Nav_Agent;

    public GameObject main_Boss_Gameobject;
    public GameObject first_Part_Gameobject;
    public GameObject second_Part_Gameobject;

    public Text success_Text;

    bool is_A1_Free = true;
    bool is_A2_Free = true;
    bool is_A3_Free = true;
    bool is_A4_Free = true;
    bool is_A5_Free = true;
    bool is_B1_Free = true;
    bool is_B2_Free = true;
    bool is_B3_Free = true;
    bool is_B4_Free = true;
    bool is_B5_Free = true;
    bool is_C1_Free = true;
    bool is_C2_Free = true;
    bool is_C3_Free = true;
    bool is_C4_Free = true;
    bool is_C5_Free = true;
    bool is_D1_Free = true;
    bool is_D2_Free = true;
    bool is_D3_Free = true;
    bool is_D4_Free = true;
    bool is_D5_Free = true;
    bool is_E1_Free = true;
    bool is_E2_Free = true;
    bool is_E3_Free = true;
    bool is_E4_Free = true;
    bool is_E5_Free = true;

    public Transform location_A1;
    public Transform location_A2;
    public Transform location_A3;
    public Transform location_A4;
    public Transform location_A5;
    public Transform location_B1;
    public Transform location_B2;
    public Transform location_B3;
    public Transform location_B4;
    public Transform location_B5;
    public Transform location_C1;
    public Transform location_C2;
    public Transform location_C3;
    public Transform location_C4;
    public Transform location_C5;
    public Transform location_D1;
    public Transform location_D2;
    public Transform location_D3;
    public Transform location_D4;
    public Transform location_D5;
    public Transform location_E1;
    public Transform location_E2;
    public Transform location_E3;
    public Transform location_E4;
    public Transform location_E5;

    bool main_can_update = true;
    bool first_can_update = true;
    bool second_can_update = true;

    bool main_Stopped = true;
    bool first_Stopped = true;
    bool second_Stopped = true;

    bool attacking = false;
    public float attack_Delay = 2f;
    public float spin_Speed = 10f;
    public float speed_Increase = 5f;
    int rand_dir_a = 1;
    int rand_dir_b = 1;
    int rand_dir_c = 1;

    public int max_Health = 40;
    public int curr_Health = 0;
    int first_Part_Health_Max = 10;
    int first_Part_Health_Curr = 0;
    int second_Part_Health_Max = 10;
    int second_Part_Health_Curr = 0;

    public float invulnerability_Duration = .05f;
    bool invulnerable = false;
    float current_Lerp_Time = 0f;
    public float lerp_Time = 1f;

    Quaternion target_Rotation_a;
    Quaternion target_Rotation_b;
    Quaternion target_Rotation_c;

    bool first_Part_Alive = true;
    bool second_Part_Alive = true;
    public float destination_Threshold = .15f;
    public float rest_Duration = 1f;

    public bool is_Awake = false;

    public Player_Input local_Player_Input;
    public GameObject disc;
    public GameObject Score;

    #endregion


    private void Update()
    {
        if (is_Awake)
        {
            if (main_Boss_Nav_Agent.remainingDistance <= destination_Threshold && main_can_update)
            {

                main_can_update = false;
                StartCoroutine(Rest(0));

            }

            if (first_Part_Nav_Agent.remainingDistance <= destination_Threshold && first_can_update && first_Part_Alive)
            {
                first_can_update = false;
                StartCoroutine(Rest(1));

            }

            if (second_Part_Nav_Agent.remainingDistance <= destination_Threshold && second_can_update && second_Part_Alive)
            {
                second_can_update = false;
                StartCoroutine(Rest(2));
            }

            Vector3 boss_New_Pos = new Vector3(main_Boss_Nav_Agent.transform.position.x, main_Boss_Gameobject.transform.position.y, main_Boss_Nav_Agent.transform.position.z);
            main_Boss_Gameobject.transform.position = boss_New_Pos;

            if (first_Part_Alive)
            {
                Vector3 new_Pos = new Vector3(first_Part_Nav_Agent.transform.position.x, first_Part_Gameobject.transform.position.y, first_Part_Nav_Agent.transform.position.z);
                first_Part_Gameobject.transform.position = new_Pos;
            }

            if (second_Part_Alive)
            {
                Vector3 new_Pos = new Vector3(second_Part_Nav_Agent.transform.position.x, second_Part_Gameobject.transform.position.y, second_Part_Nav_Agent.transform.position.z);
                second_Part_Gameobject.transform.position = new_Pos;
            }

            if (attacking)
            {
                current_Lerp_Time += Time.deltaTime;
                if (current_Lerp_Time > lerp_Time)
                {
                    current_Lerp_Time = lerp_Time;
                }

                float t = current_Lerp_Time / lerp_Time;
                t = Mathf.Sin(t * Mathf.PI * 0.5f);
                main_Boss_Gameobject.transform.rotation = Quaternion.Lerp(main_Boss_Gameobject.transform.rotation, target_Rotation_a, t);

                if (first_Part_Alive)
                {
                    first_Part_Gameobject.transform.rotation = Quaternion.Lerp(first_Part_Gameobject.transform.rotation, target_Rotation_b, t);

                }

                if (second_Part_Alive)
                {
                    second_Part_Gameobject.transform.rotation = Quaternion.Lerp(second_Part_Gameobject.transform.rotation, target_Rotation_c, t);
                }

                if (current_Lerp_Time >= 3f)
                {
                    current_Lerp_Time = 0;
                    attacking = false;
                }

            }
        }
    }

    /// <summary>
    /// Deals damage to the relevant part hit
    /// </summary>
    /// <param name="amount">How much damage to deal</param>
    /// <param name="part_Hit">Which part to take the damage</param>
    public void Take_Damage(int amount, int part_Hit)
    {
        if (!invulnerable)
        {
            curr_Health -= amount;
            //change the ui fill amount to be equal to the new amount of curr health here
            if (curr_Health <= 0)
            {
                StartCoroutine(Handle_Death());
            }

            switch (part_Hit)
            {
                case 0:
                    //do nothing different
                    break;

                case 1:
                    //take health away from the first part
                    first_Part_Health_Curr -= amount;
                    if (first_Part_Health_Curr <= 0)
                    {
                        first_Part_Alive = false;
                        first_Part_Gameobject.SetActive(false);
                        main_Boss_Nav_Agent.speed += speed_Increase;
                        second_Part_Nav_Agent.speed += speed_Increase;
                    }
                    break;

                case 2:
                    //take health away from the second part
                    second_Part_Health_Curr -= amount;
                    if (second_Part_Health_Curr <= 0)
                    {
                        second_Part_Alive = false;
                        second_Part_Gameobject.SetActive(false);
                        main_Boss_Nav_Agent.speed += speed_Increase;
                        first_Part_Nav_Agent.speed += speed_Increase;
                    }
                    break;
            }
            invulnerable = true;
            StartCoroutine(Invulnerability_Cooldown());

        }
    }

    /// <summary>
    /// Turns off Invulernability after a set amount of time
    /// </summary>
    /// <returns></returns>
    public IEnumerator Invulnerability_Cooldown()
    {
        yield return new WaitForSeconds(invulnerability_Duration);
        invulnerable = false;
    }

    /// <summary>
    /// Disables Boss Components and ends the current level
    /// </summary>
    /// <returns></returns>
    public IEnumerator Handle_Death()
    {
        local_Player_Input.input_Enabled = false;
        main_Boss_Gameobject.SetActive(false);
        first_Part_Gameobject.SetActive(false);
        second_Part_Gameobject.SetActive(false);
        disc.SetActive(false);
        Score.SetActive(false);
        yield return new WaitForSeconds(.2f);
        //display you won, thanks for playing the demo
        success_Text.text = "Well done, thanks for playing the demo";
        success_Text.enabled = true;
        yield return new WaitForSeconds(3f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Randomly sets the rotation direction of each boss laser section for the next attack
    /// </summary>
    public void Attack_Loop()
    {
        if (!attacking)
        {
            int a = Random.Range(0, 9);
            switch (a)
            {
                case 0:
                    target_Rotation_a *= Quaternion.AngleAxis(90, Vector3.forward);
                    target_Rotation_b *= Quaternion.AngleAxis(90, Vector3.forward);
                    target_Rotation_c *= Quaternion.AngleAxis(90, Vector3.forward);
                    break;

                case 1:
                    target_Rotation_a *= Quaternion.AngleAxis(90, Vector3.forward);
                    target_Rotation_b *= Quaternion.AngleAxis(90, Vector3.forward);
                    target_Rotation_c *= Quaternion.AngleAxis(90, Vector3.back);
                    break;

                case 2:
                    target_Rotation_a *= Quaternion.AngleAxis(90, Vector3.forward);
                    target_Rotation_b *= Quaternion.AngleAxis(90, Vector3.back);
                    target_Rotation_c *= Quaternion.AngleAxis(90, Vector3.back);
                    break;

                case 3:
                    target_Rotation_a *= Quaternion.AngleAxis(90, Vector3.back);
                    target_Rotation_b *= Quaternion.AngleAxis(90, Vector3.forward);
                    target_Rotation_c *= Quaternion.AngleAxis(90, Vector3.back);
                    break;

                case 4:
                    target_Rotation_a *= Quaternion.AngleAxis(90, Vector3.forward);
                    target_Rotation_b *= Quaternion.AngleAxis(90, Vector3.back);
                    target_Rotation_c *= Quaternion.AngleAxis(90, Vector3.forward);
                    break;

                case 5:
                    target_Rotation_a *= Quaternion.AngleAxis(90, Vector3.back);
                    target_Rotation_b *= Quaternion.AngleAxis(90, Vector3.back);
                    target_Rotation_c *= Quaternion.AngleAxis(90, Vector3.forward);
                    break;

                case 6:
                    target_Rotation_a *= Quaternion.AngleAxis(90, Vector3.back);
                    target_Rotation_b *= Quaternion.AngleAxis(90, Vector3.forward);
                    target_Rotation_c *= Quaternion.AngleAxis(90, Vector3.forward);
                    break;

                case 7:
                    target_Rotation_a *= Quaternion.AngleAxis(90, Vector3.back);
                    target_Rotation_b *= Quaternion.AngleAxis(90, Vector3.back);
                    target_Rotation_c *= Quaternion.AngleAxis(90, Vector3.back);
                    break;

            }

            attacking = true;
        }
    }

    /// <summary>
    /// After a short delay starts a new attack
    /// </summary>
    /// <returns></returns>
    public IEnumerator start_New_Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(attack_Delay);
            Attack_Loop();
        }
    }

    /// <summary>
    /// After a short delay sets the specified part to rest and after a delay allows it to be move or attack agaiin on the next cycle
    /// </summary>
    /// <param name="part_Number">Identifies whcih boss part to rest</param>
    /// <returns></returns>
    public IEnumerator Rest(int part_Number)
    {
        yield return new WaitForSeconds(rest_Duration);

        switch (part_Number)
        {
            case 0:
                Update_Current_Pos(ref Main_Boss_Position, ref Main_Boss_Target_Position);
                Update_Target_Location(ref Main_Boss_Position, ref Main_Boss_Target_Position, main_Boss_Nav_Agent, 0);

                yield return new WaitForSeconds(.5f);
                main_can_update = true;
                break;

            case 1:
                Update_Current_Pos(ref first_Part_Position, ref first_Part_Target_Position);
                Update_Target_Location(ref first_Part_Position, ref first_Part_Target_Position, first_Part_Nav_Agent, 1);

                yield return new WaitForSeconds(.5f);
                first_can_update = true;
                break;

            case 2:
                Update_Current_Pos(ref second_Part_Position, ref second_Part_Target_Position);
                Update_Target_Location(ref second_Part_Position, ref second_Part_Target_Position, second_Part_Nav_Agent, 2);

                yield return new WaitForSeconds(.5f);
                second_can_update = true;
                break;
        }
    }


    private void Start()
    {
        StartCoroutine(Wake());
    }

    /// <summary>
    /// Initialises variables and sets each part with a valid position to move to
    /// </summary>
    /// <returns></returns>
    IEnumerator Wake()
    {
        yield return new WaitForSeconds(1.5f);

        curr_Health = max_Health;
        first_Part_Health_Curr = first_Part_Health_Max;
        second_Part_Health_Curr = second_Part_Health_Max;
        is_A1_Free = false;
        is_A5_Free = false;
        is_C3_Free = false;
        Update_Target_Location(ref Main_Boss_Position, ref Main_Boss_Target_Position, main_Boss_Nav_Agent, 0);
        Update_Target_Location(ref first_Part_Position, ref first_Part_Target_Position, first_Part_Nav_Agent, 1);
        Update_Target_Location(ref second_Part_Position, ref second_Part_Target_Position, second_Part_Nav_Agent, 2);

        // Rotate_Part(main_Boss_Gameobject);
        target_Rotation_a = main_Boss_Gameobject.transform.rotation;
        target_Rotation_b = first_Part_Gameobject.transform.rotation;
        target_Rotation_c = second_Part_Gameobject.transform.rotation;
        StartCoroutine(start_New_Attack());
        is_Awake = true;

    }

    int rand_Num = 0;

    void Activate_Additional_Lasers()
    {
        //temporarily activate more lasers
    }

    IEnumerator cooldown_Extra_Lasers()
    {
        yield return new WaitForSeconds(5f);
    }

    void Toggle_Lasers()
    {
        //turn lasers on or off
    }

    void Increase_Speed()
    {
        //increase movement speed of all parts
    }

    void Decrease_Speed()
    {
        //decrease movement speed of all parts
    }

    void Rotate_Part(GameObject go)
    {
        main_Boss_Gameobject.transform.Rotate(Vector3.up * Time.deltaTime);
    }

    /// <summary>
    /// Chooses random target locations until a valid free spot is found for the specified part
    /// </summary>
    /// <param name="g_Pos">Current grid position of the part specified</param>
    /// <param name="g_Targ_Pos">Target position on the grid to be set as new destination</param>
    /// <param name="nav_Agent">Nav agent of the specified part</param>
    /// <param name="section_Number">Which boss gameobject is being specified</param>
    void Update_Target_Location(ref Grid_Position g_Pos, ref Grid_Position g_Targ_Pos, NavMeshAgent nav_Agent, int section_Number)
    {
        switch (g_Pos)
        {
            case Grid_Position.A1:
                rand_Num = Random.Range(0, 2);
                switch (rand_Num)
                {
                    case 0:
                        if (is_A2_Free)
                        {
                            g_Targ_Pos = Grid_Position.A2;
                            is_A2_Free = false;
                            nav_Agent.SetDestination(location_A2.position);
                        }
                        break;


                    case 1:
                        if (is_B1_Free)
                        {
                            g_Targ_Pos = Grid_Position.B1;
                            is_B1_Free = false;
                            nav_Agent.SetDestination(location_D1.position);
                        }
                        break;

                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.A2:
                rand_Num = Random.Range(0, 3);
                switch (rand_Num)
                {
                    case 0:
                        if (is_A1_Free)
                        {
                            g_Targ_Pos = Grid_Position.A1;
                            is_A1_Free = false;
                            nav_Agent.SetDestination(location_A1.position);

                        }
                        break;


                    case 1:
                        if (is_A3_Free)
                        {
                            g_Targ_Pos = Grid_Position.A3;
                            is_A3_Free = false;
                            nav_Agent.SetDestination(location_A3.position);
                        }
                        break;

                    case 2:
                        if (is_B2_Free)
                        {
                            g_Targ_Pos = Grid_Position.B2;
                            is_B2_Free = false;
                            nav_Agent.SetDestination(location_B2.position);
                        }
                        break;

                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.A3:
                rand_Num = Random.Range(0, 3);
                switch (rand_Num)
                {
                    case 0:
                        if (is_A2_Free)
                        {
                            g_Targ_Pos = Grid_Position.A2;
                            is_A2_Free = false;
                            nav_Agent.SetDestination(location_A2.position);
                        }
                        break;


                    case 1:
                        if (is_A4_Free)
                        {
                            g_Targ_Pos = Grid_Position.A4;
                            is_A4_Free = false;
                            nav_Agent.SetDestination(location_A4.position);
                        }
                        break;

                    case 2:
                        if (is_B3_Free)
                        {
                            g_Targ_Pos = Grid_Position.B3;
                            is_B3_Free = false;
                            nav_Agent.SetDestination(location_B3.position);
                        }
                        break;

                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.A4:
                rand_Num = Random.Range(0, 3);
                switch (rand_Num)
                {
                    case 0:
                        if (is_A3_Free)
                        {
                            g_Targ_Pos = Grid_Position.A3;
                            is_A3_Free = false;
                            nav_Agent.SetDestination(location_A3.position);
                        }
                        break;


                    case 1:
                        if (is_A5_Free)
                        {
                            g_Targ_Pos = Grid_Position.A5;
                            is_A5_Free = false;
                            nav_Agent.SetDestination(location_A5.position);
                        }
                        break;

                    case 2:
                        if (is_B4_Free)
                        {
                            g_Targ_Pos = Grid_Position.B4;
                            is_B4_Free = false;
                            nav_Agent.SetDestination(location_B4.position);
                        }
                        break;

                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.A5:
                rand_Num = Random.Range(0, 2);
                switch (rand_Num)
                {
                    case 0:
                        if (is_A4_Free)
                        {
                            g_Targ_Pos = Grid_Position.A4;
                            is_A4_Free = false;
                            nav_Agent.SetDestination(location_A4.position);
                        }
                        break;


                    case 1:
                        if (is_B5_Free)
                        {
                            g_Targ_Pos = Grid_Position.B5;
                            is_B5_Free = false;
                            nav_Agent.SetDestination(location_B5.position);
                        }
                        break;


                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.B1:
                rand_Num = Random.Range(0, 3);
                switch (rand_Num)
                {
                    case 0:
                        if (is_A1_Free)
                        {
                            g_Targ_Pos = Grid_Position.A1;
                            is_A1_Free = false;
                            nav_Agent.SetDestination(location_A1.position);
                        }
                        break;


                    case 1:
                        if (is_B2_Free)
                        {
                            g_Targ_Pos = Grid_Position.B2;
                            is_B2_Free = false;
                            nav_Agent.SetDestination(location_B2.position);
                        }
                        break;

                    case 2:
                        if (is_C1_Free)
                        {
                            g_Targ_Pos = Grid_Position.C1;
                            is_C1_Free = false;
                            nav_Agent.SetDestination(location_C1.position);
                        }
                        break;

                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.B2:
                rand_Num = Random.Range(0, 4);
                switch (rand_Num)
                {

                    case 0:
                        if (is_A2_Free)
                        {
                            g_Targ_Pos = Grid_Position.A2;
                            is_A2_Free = false;
                            nav_Agent.SetDestination(location_A2.position);
                        }
                        break;


                    case 1:
                        if (is_B1_Free)
                        {
                            g_Targ_Pos = Grid_Position.B1;
                            is_B1_Free = false;
                            nav_Agent.SetDestination(location_B1.position);
                        }
                        break;

                    case 2:
                        if (is_B3_Free)
                        {
                            g_Targ_Pos = Grid_Position.B3;
                            is_B3_Free = false;
                            nav_Agent.SetDestination(location_B3.position);
                        }
                        break;


                    case 3:
                        if (is_C2_Free)
                        {
                            g_Targ_Pos = Grid_Position.C2;
                            is_C2_Free = false;
                            nav_Agent.SetDestination(location_C2.position);
                        }
                        break;

                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.B3:
                rand_Num = Random.Range(0, 4);
                switch (rand_Num)
                {
                    case 0:
                        if (is_A3_Free)
                        {
                            g_Targ_Pos = Grid_Position.A3;
                            is_A3_Free = false;
                            nav_Agent.SetDestination(location_A3.position);
                        }
                        break;

                    case 1:
                        if (is_B2_Free)
                        {
                            g_Targ_Pos = Grid_Position.B2;
                            is_B2_Free = false;
                            nav_Agent.SetDestination(location_B2.position);
                        }
                        break;

                    case 2:
                        if (is_B4_Free)
                        {
                            g_Targ_Pos = Grid_Position.B4;
                            is_B4_Free = false;
                            nav_Agent.SetDestination(location_B4.position);
                        }
                        break;

                    case 3:
                        if (is_C3_Free)
                        {
                            g_Targ_Pos = Grid_Position.C3;
                            is_C3_Free = false;
                            nav_Agent.SetDestination(location_C3.position);
                        }
                        break;

                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.B4:
                rand_Num = Random.Range(0, 4);
                switch (rand_Num)
                {
                    case 0:
                        if (is_A4_Free)
                        {
                            g_Targ_Pos = Grid_Position.A4;
                            is_A4_Free = false;
                            nav_Agent.SetDestination(location_A4.position);
                        }
                        break;

                    case 1:
                        if (is_B5_Free)
                        {
                            g_Targ_Pos = Grid_Position.B5;
                            is_B5_Free = false;
                            nav_Agent.SetDestination(location_B5.position);
                        }
                        break;

                    case 2:
                        if (is_B3_Free)
                        {
                            g_Targ_Pos = Grid_Position.B3;
                            is_B3_Free = false;
                            nav_Agent.SetDestination(location_B3.position);
                        }
                        break;

                    case 3:
                        if (is_C4_Free)
                        {
                            g_Targ_Pos = Grid_Position.C4;
                            is_C4_Free = false;
                            nav_Agent.SetDestination(location_C4.position);
                        }
                        break;

                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.B5:
                rand_Num = Random.Range(0, 3);
                switch (rand_Num)
                {
                    case 0:
                        if (is_A5_Free)
                        {
                            g_Targ_Pos = Grid_Position.A5;
                            is_A5_Free = false;
                            nav_Agent.SetDestination(location_A5.position);
                        }
                        break;

                    case 1:
                        if (is_B4_Free)
                        {
                            g_Targ_Pos = Grid_Position.B4;
                            is_B4_Free = false;
                            nav_Agent.SetDestination(location_B4.position);
                        }
                        break;

                    case 3:
                        if (is_C5_Free)
                        {
                            g_Targ_Pos = Grid_Position.C5;
                            is_C5_Free = false;
                            nav_Agent.SetDestination(location_C5.position);
                        }
                        break;

                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.C1:
                rand_Num = Random.Range(0, 3);
                switch (rand_Num)
                {
                    case 0:
                        if (is_B1_Free)
                        {
                            g_Targ_Pos = Grid_Position.B1;
                            is_B1_Free = false;
                            nav_Agent.SetDestination(location_B1.position);
                        }
                        break;

                    case 1:
                        if (is_C2_Free)
                        {
                            g_Targ_Pos = Grid_Position.C2;
                            is_C2_Free = false;
                            nav_Agent.SetDestination(location_C2.position);
                        }
                        break;

                    case 2:
                        if (is_D1_Free)
                        {
                            g_Targ_Pos = Grid_Position.D1;
                            is_D1_Free = false;
                            nav_Agent.SetDestination(location_D1.position);
                        }
                        break;

                    default:
                        //do nothing
                        break;

                }
                break;

            case Grid_Position.C2:
                rand_Num = Random.Range(0, 4);
                switch (rand_Num)
                {
                    case 0:
                        if (is_B2_Free)
                        {
                            g_Targ_Pos = Grid_Position.B2;
                            is_B2_Free = false;
                            nav_Agent.SetDestination(location_B2.position);
                        }
                        break;

                    case 1:
                        if (is_C1_Free)
                        {
                            g_Targ_Pos = Grid_Position.C1;
                            is_C1_Free = false;
                            nav_Agent.SetDestination(location_C1.position);
                        }
                        break;

                    case 2:
                        if (is_C3_Free)
                        {
                            g_Targ_Pos = Grid_Position.C3;
                            is_C3_Free = false;
                            nav_Agent.SetDestination(location_C3.position);
                        }
                        break;

                    case 3:
                        if (is_D2_Free)
                        {
                            g_Targ_Pos = Grid_Position.D2;
                            is_D2_Free = false;
                            nav_Agent.SetDestination(location_D2.position);
                        }
                        break;



                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.C3:
                rand_Num = Random.Range(0, 4);
                switch (rand_Num)
                {
                    case 0:
                        if (is_B3_Free)
                        {
                            g_Targ_Pos = Grid_Position.B3;
                            is_B3_Free = false;
                            nav_Agent.SetDestination(location_B3.position);
                        }
                        break;

                    case 1:
                        if (is_C2_Free)
                        {
                            g_Targ_Pos = Grid_Position.C2;
                            is_C2_Free = false;
                            nav_Agent.SetDestination(location_C2.position);
                        }
                        break;

                    case 2:
                        if (is_C4_Free)
                        {
                            g_Targ_Pos = Grid_Position.C4;
                            is_C4_Free = false;
                            nav_Agent.SetDestination(location_C4.position);
                        }
                        break;

                    case 3:
                        if (is_D3_Free)
                        {
                            g_Targ_Pos = Grid_Position.D3;
                            is_D3_Free = false;
                            nav_Agent.SetDestination(location_D3.position);
                        }
                        break;
                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.C4:
                rand_Num = Random.Range(0, 4);
                switch (rand_Num)
                {
                    case 0:
                        if (is_B4_Free)
                        {
                            g_Targ_Pos = Grid_Position.B4;
                            is_B4_Free = false;
                            nav_Agent.SetDestination(location_B4.position);
                        }
                        break;

                    case 1:
                        if (is_C3_Free)
                        {
                            g_Targ_Pos = Grid_Position.C3;
                            is_C3_Free = false;
                            nav_Agent.SetDestination(location_C3.position);
                        }
                        break;

                    case 2:
                        if (is_C5_Free)
                        {
                            g_Targ_Pos = Grid_Position.C5;
                            is_C5_Free = false;
                            nav_Agent.SetDestination(location_C5.position);
                        }
                        break;

                    case 3:
                        if (is_D4_Free)
                        {
                            g_Targ_Pos = Grid_Position.D4;
                            is_D4_Free = false;
                            nav_Agent.SetDestination(location_D4.position);
                        }
                        break;

                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.C5:
                rand_Num = Random.Range(0, 3);
                switch (rand_Num)
                {
                    case 0:
                        if (is_B5_Free)
                        {
                            g_Targ_Pos = Grid_Position.B5;
                            is_B5_Free = false;
                            nav_Agent.SetDestination(location_B5.position);
                        }
                        break;

                    case 1:
                        if (is_C4_Free)
                        {
                            g_Targ_Pos = Grid_Position.C4;
                            is_C4_Free = false;
                            nav_Agent.SetDestination(location_C4.position);
                        }
                        break;


                    case 2:
                        if (is_D5_Free)
                        {
                            g_Targ_Pos = Grid_Position.D5;
                            is_D5_Free = false;
                            nav_Agent.SetDestination(location_D5.position);
                        }
                        break;

                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.D1:
                rand_Num = Random.Range(0, 3);
                switch (rand_Num)
                {
                    case 0:
                        if (is_C1_Free)
                        {
                            g_Targ_Pos = Grid_Position.C1;
                            is_C1_Free = false;
                            nav_Agent.SetDestination(location_C1.position);
                        }
                        break;

                    case 1:
                        if (is_E1_Free)
                        {
                            g_Targ_Pos = Grid_Position.E1;
                            is_E1_Free = false;
                            nav_Agent.SetDestination(location_E1.position);
                        }
                        break;


                    case 2:
                        if (is_D2_Free)
                        {
                            g_Targ_Pos = Grid_Position.D2;
                            is_D2_Free = false;
                            nav_Agent.SetDestination(location_D2.position);
                        }
                        break;

                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.D2:
                rand_Num = Random.Range(0, 4);
                switch (rand_Num)
                {
                    case 0:
                        if (is_C2_Free)
                        {
                            g_Targ_Pos = Grid_Position.C2;
                            is_C2_Free = false;
                            nav_Agent.SetDestination(location_C2.position);
                        }
                        break;

                    case 1:
                        if (is_D1_Free)
                        {
                            g_Targ_Pos = Grid_Position.D1;
                            is_D1_Free = false;
                            nav_Agent.SetDestination(location_D1.position);
                        }
                        break;

                    case 2:
                        if (is_D3_Free)
                        {
                            g_Targ_Pos = Grid_Position.D3;
                            is_D3_Free = false;
                            nav_Agent.SetDestination(location_D3.position);
                        }
                        break;

                    case 3:
                        if (is_E2_Free)
                        {
                            g_Targ_Pos = Grid_Position.E2;
                            is_E2_Free = false;
                            nav_Agent.SetDestination(location_E2.position);
                        }
                        break;
                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.D3:
                rand_Num = Random.Range(0, 4);
                switch (rand_Num)
                {
                    case 0:
                        if (is_C3_Free)
                        {
                            g_Targ_Pos = Grid_Position.C3;
                            is_C3_Free = false;
                            nav_Agent.SetDestination(location_C3.position);
                        }
                        break;

                    case 1:
                        if (is_D2_Free)
                        {
                            g_Targ_Pos = Grid_Position.D2;
                            is_D2_Free = false;
                            nav_Agent.SetDestination(location_D2.position);
                        }
                        break;

                    case 2:
                        if (is_D4_Free)
                        {
                            g_Targ_Pos = Grid_Position.D4;
                            is_D4_Free = false;
                            nav_Agent.SetDestination(location_D4.position);
                        }
                        break;

                    case 3:
                        if (is_E3_Free)
                        {
                            g_Targ_Pos = Grid_Position.E3;
                            is_E3_Free = false;
                            nav_Agent.SetDestination(location_E3.position);
                        }
                        break;

                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.D4:
                rand_Num = Random.Range(0, 4);
                switch (rand_Num)
                {
                    case 0:
                        if (is_C4_Free)
                        {
                            g_Targ_Pos = Grid_Position.C4;
                            is_C4_Free = false;
                            nav_Agent.SetDestination(location_C4.position);
                        }
                        break;

                    case 1:
                        if (is_D5_Free)
                        {
                            g_Targ_Pos = Grid_Position.D5;
                            is_D5_Free = false;
                            nav_Agent.SetDestination(location_D5.position);
                        }
                        break;

                    case 2:
                        if (is_D3_Free)
                        {
                            g_Targ_Pos = Grid_Position.D3;
                            is_D3_Free = false;
                            nav_Agent.SetDestination(location_D3.position);
                        }
                        break;

                    case 3:
                        if (is_E4_Free)
                        {
                            g_Targ_Pos = Grid_Position.E4;
                            is_E4_Free = false;
                            nav_Agent.SetDestination(location_E4.position);
                        }
                        break;

                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.D5:
                rand_Num = Random.Range(0, 3);
                switch (rand_Num)
                {
                    case 0:
                        if (is_C5_Free)
                        {
                            g_Targ_Pos = Grid_Position.C5;
                            is_C5_Free = false;
                            nav_Agent.SetDestination(location_C5.position);
                        }
                        break;

                    case 1:
                        if (is_D4_Free)
                        {
                            g_Targ_Pos = Grid_Position.D4;
                            is_D4_Free = false;
                            nav_Agent.SetDestination(location_D4.position);
                        }
                        break;

                    case 2:
                        if (is_E5_Free)
                        {
                            g_Targ_Pos = Grid_Position.E5;
                            is_E5_Free = false;
                            nav_Agent.SetDestination(location_E5.position);
                        }
                        break;

                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.E1:
                rand_Num = Random.Range(0, 2);
                switch (rand_Num)
                {
                    case 0:
                        if (is_D1_Free)
                        {
                            g_Targ_Pos = Grid_Position.D1;
                            is_D1_Free = false;
                            nav_Agent.SetDestination(location_D1.position);
                        }
                        break;

                    case 1:
                        if (is_E2_Free)
                        {
                            g_Targ_Pos = Grid_Position.E2;
                            is_E2_Free = false;
                            nav_Agent.SetDestination(location_E2.position);
                        }
                        break;

                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.E2:
                rand_Num = Random.Range(0, 3);
                switch (rand_Num)
                {
                    case 0:
                        if (is_E1_Free)
                        {
                            g_Targ_Pos = Grid_Position.E1;
                            is_E1_Free = false;
                            nav_Agent.SetDestination(location_E1.position);
                        }
                        break;

                    case 1:
                        if (is_D2_Free)
                        {
                            g_Targ_Pos = Grid_Position.D2;
                            is_D2_Free = false;
                            nav_Agent.SetDestination(location_D2.position);
                        }
                        break;

                    case 2:
                        if (is_E3_Free)
                        {
                            g_Targ_Pos = Grid_Position.E3;
                            is_E3_Free = false;
                            nav_Agent.SetDestination(location_E3.position);
                        }
                        break;

                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.E3:
                rand_Num = Random.Range(0, 3);
                switch (rand_Num)
                {
                    case 0:
                        if (is_E2_Free)
                        {
                            g_Targ_Pos = Grid_Position.E2;
                            is_E2_Free = false;
                            nav_Agent.SetDestination(location_E2.position);
                        }
                        break;

                    case 1:
                        if (is_D3_Free)
                        {
                            g_Targ_Pos = Grid_Position.D3;
                            is_D3_Free = false;
                            nav_Agent.SetDestination(location_D3.position);
                        }
                        break;

                    case 2:
                        if (is_E4_Free)
                        {
                            g_Targ_Pos = Grid_Position.E4;
                            is_E4_Free = false;
                            nav_Agent.SetDestination(location_E4.position);
                        }
                        break;

                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.E4:
                rand_Num = Random.Range(0, 3);
                switch (rand_Num)
                {
                    case 0:
                        if (is_E5_Free)
                        {
                            g_Targ_Pos = Grid_Position.E5;
                            is_E5_Free = false;
                            nav_Agent.SetDestination(location_E5.position);
                        }
                        break;

                    case 1:
                        if (is_D4_Free)
                        {
                            g_Targ_Pos = Grid_Position.D4;
                            is_D4_Free = false;
                            nav_Agent.SetDestination(location_D4.position);
                        }
                        break;

                    case 2:
                        if (is_E3_Free)
                        {
                            g_Targ_Pos = Grid_Position.E3;
                            is_E3_Free = false;
                            nav_Agent.SetDestination(location_E3.position);
                        }
                        break;

                    default:
                        //do nothing
                        break;
                }
                break;

            case Grid_Position.E5:
                rand_Num = Random.Range(0, 2);
                switch (rand_Num)
                {
                    case 0:
                        if (is_E4_Free)
                        {
                            g_Targ_Pos = Grid_Position.E4;
                            is_E4_Free = false;
                            nav_Agent.SetDestination(location_E4.position);
                        }
                        break;

                    case 1:
                        if (is_D5_Free)
                        {
                            g_Targ_Pos = Grid_Position.D5;
                            is_D5_Free = false;
                            nav_Agent.SetDestination(location_D5.position);
                        }
                        break;

                    default:
                        //do nothing
                        break;
                }
                break;
        }

        if (g_Pos != g_Targ_Pos)
        {
            switch (section_Number)
            {
                case 0:
                    main_can_update = true;
                    break;

                case 1:
                    first_can_update = true;
                    break;

                case 2:
                    second_can_update = true;
                    break;
            }

        }
    }

    /// <summary>
    /// Updates which grid positions are free after moving
    /// </summary>
    /// <param name="g_Pos">The position the object moved from to be set as free</param>
    /// <param name="g_Targ_Position">The new target position to be set as occupied</param>
    void Update_Current_Pos(ref Grid_Position g_Pos, ref Grid_Position g_Targ_Position)
    {

        switch (g_Pos)
        {
            case Grid_Position.A1:
                is_A1_Free = true;
                break;

            case Grid_Position.A2:
                is_A2_Free = true;
                break;

            case Grid_Position.A3:
                is_A3_Free = true;
                break;

            case Grid_Position.A4:
                is_A4_Free = true;
                break;

            case Grid_Position.A5:
                is_A5_Free = true;
                break;

            case Grid_Position.B1:
                is_B1_Free = true;
                break;

            case Grid_Position.B2:
                is_B2_Free = true;
                break;

            case Grid_Position.B3:
                is_B3_Free = true;
                break;

            case Grid_Position.B4:
                is_B4_Free = true;
                break;

            case Grid_Position.B5:
                is_B5_Free = true;
                break;

            case Grid_Position.C1:
                is_C1_Free = true;
                break;

            case Grid_Position.C2:
                is_C2_Free = true;
                break;

            case Grid_Position.C3:
                is_C3_Free = true;
                break;

            case Grid_Position.C4:
                is_C4_Free = true;
                break;

            case Grid_Position.C5:
                is_C5_Free = true;
                break;

            case Grid_Position.D1:
                is_D1_Free = true;
                break;

            case Grid_Position.D2:
                is_D2_Free = true;
                break;

            case Grid_Position.D3:
                is_D3_Free = true;
                break;

            case Grid_Position.D4:
                is_D4_Free = true;
                break;

            case Grid_Position.D5:
                is_D5_Free = true;
                break;

            case Grid_Position.E1:
                is_E1_Free = true;
                break;

            case Grid_Position.E2:
                is_E2_Free = true;
                break;

            case Grid_Position.E3:
                is_E3_Free = true;
                break;

            case Grid_Position.E4:
                is_E4_Free = true;
                break;

            case Grid_Position.E5:
                is_E5_Free = true;
                break;
        }

        switch (g_Targ_Position)
        {
            case Grid_Position.A1:
                g_Pos = Grid_Position.A1;
                break;

            case Grid_Position.A2:
                g_Pos = Grid_Position.A2;
                break;

            case Grid_Position.A3:
                g_Pos = Grid_Position.A3;
                break;

            case Grid_Position.A4:
                g_Pos = Grid_Position.A4;
                break;

            case Grid_Position.A5:
                g_Pos = Grid_Position.A5;
                break;

            case Grid_Position.B1:
                g_Pos = Grid_Position.B1;
                break;

            case Grid_Position.B2:
                g_Pos = Grid_Position.B2;
                break;

            case Grid_Position.B3:
                g_Pos = Grid_Position.B3;
                break;

            case Grid_Position.B4:
                g_Pos = Grid_Position.B4;
                break;

            case Grid_Position.B5:
                g_Pos = Grid_Position.B5;
                break;

            case Grid_Position.C1:
                g_Pos = Grid_Position.C1;
                break;

            case Grid_Position.C2:
                g_Pos = Grid_Position.C2;
                break;

            case Grid_Position.C3:
                g_Pos = Grid_Position.C3;
                break;

            case Grid_Position.C4:
                g_Pos = Grid_Position.C4;
                break;

            case Grid_Position.C5:
                g_Pos = Grid_Position.C5;
                break;

            case Grid_Position.D1:
                g_Pos = Grid_Position.D1;
                break;

            case Grid_Position.D2:
                g_Pos = Grid_Position.D2;
                break;

            case Grid_Position.D3:
                g_Pos = Grid_Position.D3;
                break;

            case Grid_Position.D4:
                g_Pos = Grid_Position.D4;
                break;

            case Grid_Position.D5:
                g_Pos = Grid_Position.D5;
                break;

            case Grid_Position.E1:
                g_Pos = Grid_Position.E1;
                break;

            case Grid_Position.E2:
                g_Pos = Grid_Position.E2;
                break;

            case Grid_Position.E3:
                g_Pos = Grid_Position.E3;
                break;

            case Grid_Position.E4:
                g_Pos = Grid_Position.E4;
                break;

            case Grid_Position.E5:
                g_Pos = Grid_Position.E5;
                break;
        }

    }
}
