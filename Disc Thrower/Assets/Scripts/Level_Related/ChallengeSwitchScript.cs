using System.Collections;
using UnityEngine;

public class ChallengeSwitchScript : MonoBehaviour
{

    #region properties
    public bool recallOnHit = true;
    public Material activatedMaterial;
    private Material startMaterial;
    [Tooltip("Spawner: Is there a key in the chest?, Destroyer: Do the destroyables respawn?, Pressure Plate: Must be set to false")]
    public bool parameter = false;
    [Tooltip("If objects can respawn, how long does that take?")]
    public float timeTilRespawn = 0;

    private bool triggered = false;
    private string colliderTag;

    delegate void ChestSpawnDelegate(bool isKey);
    ChestSpawnDelegate triggerDelegate;

    public enum SwitchType { spawner, destroyer, pressurePlate }
    public SwitchType switchType;

    [Space]
    [Tooltip("GOs to destroy if the switch type is set to 'Destroyer'")]
    public GameObject[] destroyables;

    private AudioSource audSource;
    public AudioClip[] audioClips;
    #endregion

    void Start()
    {
        //Search for the room generator associated with the room this button is in
        Transform parentTransform = transform.parent.parent.parent.parent;
        startMaterial = GetComponent<MeshRenderer>().material;
        audSource = GetComponent<AudioSource>();

        colliderTag = "Disc";

        //Setting our loacal delegate to be equal to the desired function
        switch (switchType)
        {
            case SwitchType.spawner:
                triggerDelegate = parentTransform.GetComponent<RoomGenerator>().SpawnChest;
                break;
            case SwitchType.destroyer:
                triggerDelegate = DeactivateGO;
                break;
            case SwitchType.pressurePlate:
                triggerDelegate = DeactivateGO;
                colliderTag = "Player";
                break;
            default:
                break;
        }

        triggerDelegate += ActivateSwitch;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == colliderTag)
        {
            if (!triggered)
            {
                triggerDelegate(false);
            }
        }

        if (col.collider.tag == "Disc")
        {
            //Recall Disc
            if (recallOnHit)
                col.collider.GetComponent<ProjectileCollision>().RecallDisc();
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (switchType == SwitchType.pressurePlate)
        {
            if (col.collider.tag == colliderTag && triggered)
            {
                StartCoroutine(RespawnDestroyables());
                ResetSwitch();
            }
        }
    }

    void DeactivateGO(bool respawn)
    {
        for (int i = 0; i < destroyables.Length; i++)
        {
            destroyables[i].GetComponent<MeshRenderer>().enabled = false;
            if (destroyables[i].GetComponent<MeshCollider>() != null)
            {
                destroyables[i].GetComponent<MeshCollider>().enabled = false;
            }
            else
            {
                destroyables[i].GetComponent<BoxCollider>().enabled = false;
            }
        }

        if (respawn)
        {
            StartCoroutine(RespawnDestroyables());
        }
    }

    IEnumerator RespawnDestroyables()
    {
        yield return new WaitForSeconds(timeTilRespawn);

        for (int i = 0; i < destroyables.Length; i++)
        {
            destroyables[i].GetComponent<MeshRenderer>().enabled = true;
            if (destroyables[i].GetComponent<MeshCollider>() != null)
            {
                destroyables[i].GetComponent<MeshCollider>().enabled = true;
            }
            else
            {
                destroyables[i].GetComponent<BoxCollider>().enabled = true;
            }
        }
    }

    void ActivateSwitch(bool trigger)
    {
        GetComponent<MeshRenderer>().material = activatedMaterial;
        triggered = true;
        audSource.clip = audioClips[0];
        audSource.Play();
    }

    void ResetSwitch()
    {
        triggered = false;
        GetComponent<MeshRenderer>().material = startMaterial;

        audSource.clip = audioClips[0];
        audSource.Play();
    }
}