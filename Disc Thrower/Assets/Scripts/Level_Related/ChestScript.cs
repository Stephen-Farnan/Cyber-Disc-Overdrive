using System.Collections;
using UnityEngine;

public class ChestScript : MonoBehaviour
{

    [HideInInspector]
    public LevelManager levMan;
    [Tooltip("0: Spawn, 1: Open")]
    public AudioClip[] clips;
    private AudioSource audSource;
    private Material mat;
    public bool containsBossKey = false;

    void Awake()
    {
        levMan = GameObject.Find("Level Generator").GetComponent<LevelManager>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        mat.SetFloat("_deathSlider", 0);
        audSource = GetComponent<AudioSource>();
        if (clips[0] != null)
        {
            audSource.clip = clips[0];
        }
    }

    void Start()
    {
        audSource.Play();
        StartCoroutine(PhaseIn());
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            StartCoroutine(PhaseOut());
            GetComponent<BoxCollider>().enabled = false;

            audSource.clip = clips[1];
            audSource.Play();

            if (containsBossKey)
            {
                AcquireBossKey();
            }
            else
            {
                //Get upgrade
                AcquireUpgrade(col);
            }
        }
    }

    public void SetAsBossKey()
    {
        containsBossKey = true;
    }

    void AcquireBossKey()
    {
        levMan.HasBossKey = true;
        StartCoroutine(levMan.DisplayMessage("Key Acquired"));
    }

    /// <summary>
    /// Chooses a random upgrade for the player when they open a chest
    /// </summary>
    /// <param name="col"></param>
    void AcquireUpgrade(Collider col)
    {

        int random_Num = Random.Range(0, 3);
        Player_Augments local_Player_Augments = col.gameObject.GetComponent<Player_Augments>();
        switch (random_Num)
        {
            case 0:
                local_Player_Augments.Add_Augment_To_Player(Player_Augments.Player_Augments_List.DEFENSIVE_DRONES);
                StartCoroutine(levMan.DisplayMessage("Drones Acquired"));
                break;

            case 1:
                local_Player_Augments.Add_Augment_To_Player(Player_Augments.Player_Augments_List.INCREASE_HEALTH);
                StartCoroutine(levMan.DisplayMessage("Added Health Acquired"));
                break;


            case 2:
                local_Player_Augments.Add_Augment_To_Player(Player_Augments.Player_Augments_List.EXTRA_MOVESPEED);
                StartCoroutine(levMan.DisplayMessage("Extra Movespeed Acquired"));
                break;

            default:
                local_Player_Augments.Add_Augment_To_Player(Player_Augments.Player_Augments_List.DEFENSIVE_DRONES);
                StartCoroutine(levMan.DisplayMessage("Drones Acquired"));
                break;
        }
    }

    IEnumerator PhaseIn()
    {
        float deathTime = 0.5f;
        float timer = deathTime;
        while (timer > 0)
        {
            mat.SetFloat("_deathSlider", timer / deathTime);
            timer -= Time.deltaTime;
            yield return null;
        }
        mat.SetFloat("_deathSlider", 0);
    }

    IEnumerator PhaseOut()
    {
        float deathTime = 0.5f;
        float timer = deathTime;
        while (timer > 0)
        {
            mat.SetFloat("_deathSlider", 1 - timer / deathTime);
            timer -= Time.deltaTime;
            yield return null;
        }
        mat.SetFloat("_deathSlider", 1);
    }
}