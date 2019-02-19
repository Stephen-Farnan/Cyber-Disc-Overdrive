//This script controls in-game room functions such as un/locking rooms, spawning enemies, etc

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoomScript : MonoBehaviour
{

    #region properties
    public GameObject[] walls;
    [HideInInspector]
    public List<GameObject> roomDoors = new List<GameObject>();
    [HideInInspector]
    public List<Transform> enemySpawnPoints = new List<Transform>();
    private int pointsPerRoom;
    private int noOfEnemies;
    //Stores enemies to be spawned data
    List<int> enemyList = new List<int>();
    //Stores enemy gameobjects
    List<GameObject> enemies = new List<GameObject>();
    List<Transform> selectedSpawnPoints = new List<Transform>();

    public GameObject spawnParticleEffect;

    public GameObject roomMask;
    public float maskClearSpeed;
    [HideInInspector]
    public bool playerIsInRoom = false;
    private bool roomActivated = false;
    public bool roomCleared = false;
    private bool maskCleared = false;
    private Transform layout;

    private bool isBossDoorRoom = false;
    private bool isBossKeyRoom = false;
    private bool isChestRoom = false;
    private bool isChallengeRoom = false;
    private bool isLargeRoom = false;

    private RoomGenerator roomGen;
    private EnemyManager enMan;
    private LevelGenerator levGen;
    private LevelManager levMan;
    private CameraFollow cam;
    private MouseShooting shootingScript;
    private CharacterNavMeshMovement moveScript;
    #endregion


    void Start()
    {
        roomGen = transform.parent.GetComponent<RoomGenerator>();
        GameObject gO = GameObject.Find("Level Generator");
        enMan = gO.GetComponent<EnemyManager>();
        levGen = gO.GetComponent<LevelGenerator>();
        levMan = gO.GetComponent<LevelManager>();
        cam = GameObject.FindWithTag("MainCamera").GetComponent<CameraFollow>();
        gO = GameObject.FindWithTag("Player");
        shootingScript = gO.GetComponent<MouseShooting>();
        moveScript = gO.GetComponent<CharacterNavMeshMovement>();

        pointsPerRoom = enMan.levelDifficulty;
        if (DebugHandler.debugEnabled)
        {
            roomMask.GetComponent<MeshRenderer>().enabled = DebugHandler.roomMasksEnabled;
        }
    }

    void Update()
    {
        if (DebugHandler.debugEnabled)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (roomActivated)
                {
                    ClearRoom();
                }
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                RemoveRoomMask();
            }
        }
    }

    public void SetRoomGenerator(RoomGenerator roomGenerator)
    {
        roomGen = roomGenerator;
    }

    /// <summary>
    /// Sets enemies to spawn into room
    /// </summary>
    /// <returns></returns>
    public IEnumerator SelectEnemies()
    {
        yield return null;

        bool containsZero = false;

        for (int i = 0; i < enMan.enemyValue.Length; i++)
        {
            if (enMan.enemyValue[i] <= 0)
            {
                containsZero = true;
            }
        }

        if (enMan.enemyPrefabs.Length > enMan.enemyValue.Length || containsZero)
        {
            Debug.LogError("Enemy Manager arrays 'Enemy Prefabs' and 'Enemy Value' must be the same length and 'Enemy Value' must not contain any numbers below 1");
        }

        int variation;
        if (isLargeRoom)
        {
            variation = Random.Range(2, 5);
        }
        else
        {
            variation = Random.Range(-2, 3);
        }

        //Chance to increase or decrease difficulty of room
        if (levGen.Roll(80))
        {
            pointsPerRoom += variation;
            //Update Enemy Manager difficulty so we can debug it afterwards
            enMan.EditActualLevelDifficulty(variation);
        }

        //The sum of the value of all enemies currently slected in this room
        int currentRoomValue = 0;
        int minEnemyValue = Mathf.Min(enMan.enemyValue);

        while (currentRoomValue < pointsPerRoom)
        {
            int enemySelection;
            if (minEnemyValue + currentRoomValue >= pointsPerRoom)
            {
                enemySelection = System.Array.IndexOf(enMan.enemyValue, minEnemyValue);
                //Add the enemy index selected to the enemies list
                enemyList.Add(enemySelection);
                currentRoomValue += enMan.enemyValue[enemySelection];
            }
            else
            {
                enemySelection = Random.Range(0, enMan.enemyPrefabs.Length);
                if (currentRoomValue + enMan.enemyValue[enemySelection] <= pointsPerRoom)
                {
                    //Add the enemy index selected to the enemies list
                    enemyList.Add(enemySelection);
                    currentRoomValue += enMan.enemyValue[enemySelection];
                }
            }
        }

        noOfEnemies = enemyList.Count;
        PrepareSpawnPoints();
    }

    /// <summary>
    /// Finds and sets Room Door References for all doors in the level
    /// </summary>
    public void SetRoomScriptRefOnDoors()
    {
        for (int i = 0; i < walls.Length; i++)
        {
            if (roomGen.CheckForDoor(i))
            {
                roomDoors.Add(walls[i].transform.GetChild(0).GetChild(0).gameObject);
            }
        }

        foreach (GameObject go in roomDoors)
        {
            if (go.GetComponent<BossDoorScript>() == null)
            {
                go.GetComponent<RoomDoorScript>().roomScript = this;
            }
        }
    }

    /// <summary>
    /// Sets the enemy spawn points for the room based on the child of the layout parent
    /// </summary>
    /// <param name="layoutTransform"></param>
    public void SetEnemySpawnPoints(Transform layoutTransform)
    {
        layout = layoutTransform;

        //Relies on the spawn points being children of child[0] of the layout parent!!!
        //enemySpawnPoints = new Transform[layout.GetChild(0).childCount];
        int noOfSpawnPoints = layout.GetChild(0).childCount;

        for (int i = 0; i < noOfSpawnPoints; i++)
        {
            enemySpawnPoints.Add(layout.GetChild(0).GetChild(i));
        }
    }

    //Acknowledging enemy kill and checking for all enemies cleared to unlock room
    public void EnemyKilled()
    {
        //Debug.Log("Enemy Killed");
        noOfEnemies--;
        if (noOfEnemies == 0)
        {
            UnlockRoom();
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            playerIsInRoom = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            playerIsInRoom = false;
        }
    }

    //Used to be coroutine
    void PrepareSpawnPoints()
    {
        if (noOfEnemies > enemySpawnPoints.Count)
        {
            Debug.Log("Not enough spawn points to spawn all enemies");
            return;
        }

        foreach (int enemySelection in enemyList)
        {
            bool allDone = false;

            while (!allDone)
            {
                //Choose a random number within the count of the spawn point list
                int spawnIndex = Random.Range(0, enemySpawnPoints.Count);
                //Add the transform at that index to the chosen list
                selectedSpawnPoints.Add(enemySpawnPoints[spawnIndex]);
                //Remove that point from the available list
                enemySpawnPoints.Remove(enemySpawnPoints[spawnIndex]);
                //If we've chosen a spawn point for each enemy to spawn
                if (selectedSpawnPoints.Count >= noOfEnemies)
                {
                    allDone = true;
                }
            }
        }
    }

    IEnumerator SpawnEnemies()
    {
        float waitTime = 1.0f;

        yield return new WaitForSeconds(1);

        for (int i = 0; i < enemyList.Count; i++)
        {
            //Instantiate a spawn effect
            Instantiate(spawnParticleEffect, selectedSpawnPoints[i].position, Quaternion.identity);
            yield return new WaitForSeconds(waitTime);
            //Spawn enemy
            GameObject gO = Instantiate(enMan.enemyPrefabs[enemyList[i]], selectedSpawnPoints[i].position, selectedSpawnPoints[i].rotation);
            gO.transform.GetChild(0).gameObject.GetComponent<Enemy_AI>().roomScript = this;
            enemies.Add(gO);
            gO.transform.GetChild(0).gameObject.GetComponent<CapsuleCollider>().enabled = true;
        }
    }

    //Locking the player into a room and spawning enemies
    void SealRoom()
    {
        roomActivated = true;

        foreach (GameObject go in roomDoors)
        {
            foreach (BoxCollider boxCol in go.GetComponents<BoxCollider>())
            {
                if (boxCol.isTrigger)
                {
                    boxCol.enabled = false;
                }
                else
                {
                    boxCol.enabled = true;
                }
            }
            go.GetComponent<MeshRenderer>().enabled = true;
            go.GetComponent<NavMeshObstacle>().enabled = true;
            go.GetComponent<RoomDoorScript>().enabled = false;
            //Putting Door on Environment layer
            go.layer = 14;
        }

        cam.SetNewTarget(transform, (Vector3.forward * -5));
        //Deactivate the room mask revealer
        ShaderPositionUpdate.SetRevealer(false);

        StartCoroutine(shootingScript.Recall());
        moveScript.LockHover(true);

        StartCoroutine(SpawnEnemies());
    }

    //Clear Room For Testing
    void ClearRoom()
    {
        foreach (GameObject gO in enemies)
        {
            StartCoroutine(gO.transform.GetChild(0).GetComponent<Enemy_AI>().Die());
        }
    }

    //Unlocking the room after all enemies defeated
    void UnlockRoom()
    {

        foreach (GameObject go in roomDoors)
        {
            go.GetComponent<MeshRenderer>().enabled = false;
            go.GetComponent<BoxCollider>().enabled = false;
            go.GetComponent<NavMeshObstacle>().enabled = false;
            //Putting Door on Ignore Raycast layer
            go.layer = 2;
        }

        SetRoomCleared();

        cam.SetTargetToPlayer();
        //Reactivate the room mask revealer
        ShaderPositionUpdate.SetRevealer(true);

        StartCoroutine(shootingScript.Recall());
        moveScript.LockHover(false);

        if (isBossKeyRoom)
        {
            roomGen.SpawnChest(true);
        }
        else if (isChestRoom)
        {
            roomGen.SpawnChest(false);
        }
        else
        {
            if (levGen.Roll(levMan.chanceToSpawnHealthRegen))
            {
                roomGen.SpawnHealthRegen();
                levMan.AddHealthRegen();
            }
        }
    }

    public void SetRoomCleared()
    {
        roomActivated = false;
        roomCleared = true;
    }

    //Check that the player is in the room and hasn't cleared it already. Lock room if true
    public void CheckPlayerIsInRoom()
    {
        if (playerIsInRoom)
        {
            if (!maskCleared)
            {
                StartCoroutine(RemoveRoomMask());
            }
            if (!roomCleared && !roomActivated)
            {
                SealRoom();
            }
        }
    }

    public void SetAsSpecialRoom(int specialType)
    {
        switch (specialType)
        {
            case 0:
                isBossDoorRoom = true;
                SetRoomCleared();
                break;

            case 1:
                isBossKeyRoom = true;
                break;

            case 2:
                isChestRoom = true;
                break;

            case 3:
                isChallengeRoom = true;
                SetRoomCleared();
                break;

            default:
                break;
        }
    }

    public void SetAsLargeRoom()
    {
        isLargeRoom = true;
    }

    public IEnumerator RemoveRoomMask()
    {
        Material mat = roomMask.GetComponent<MeshRenderer>().material;
        float baseRad = mat.GetFloat("_radius");
        float alph = mat.GetFloat("_multiplier");
        float rad = baseRad;
        while (alph > 0)
        {
            alph -= Time.deltaTime * maskClearSpeed;
            mat.SetFloat("_multiplier", alph);

            rad += Time.deltaTime * maskClearSpeed;
            mat.SetFloat("_radius", rad);
            yield return null;
        }
        //mat.SetFloat("_radius", baseRad);
        maskCleared = true;
    }

    public void PrepareForDeletion()
    {
        StopAllCoroutines();
        roomDoors.Clear();
    }
}
