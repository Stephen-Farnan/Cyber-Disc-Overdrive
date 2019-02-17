using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossRoomDoor : MonoBehaviour
{
	private Transform player;
	private CameraFollow cam;
	private MouseShooting shootingScript;
	private CharacterNavMeshMovement moveScript;
    public GameObject boss_GO;
    int random_Num = 0;

    public enum Boss_Type
    {
        LASER, MULTI_DISC
    }

    public Boss_Type boss_Type;



    public GameObject boss_Pos;


    private void Start(){

        //spawns laser boss
        boss_GO = GameObject.Find("Laser_Boss").gameObject;
        boss_Pos = GameObject.FindGameObjectWithTag("Laser_Pos");
        // boss_GO.transform.SetParent(GameObject.FindGameObjectWithTag("Boss_Room").transform);
        boss_GO.SetActive(false);
        boss_GO.transform.position = new Vector3(365.5667f, -18.6f, 65.15f);
        boss_GO.gameObject.SetActive(true);
        random_Num = Random.Range(0, 2);
        random_Num = 1;
        if(random_Num == 1)
        {
            boss_Type = Boss_Type.LASER;
        }

        else
        {
            boss_Type = Boss_Type.MULTI_DISC;

        }
        player = GameObject.FindWithTag("Player").transform;
		cam = GameObject.FindWithTag("MainCamera").GetComponent<CameraFollow>();
		shootingScript = player.GetComponent<MouseShooting>();
		moveScript = player.GetComponent<CharacterNavMeshMovement>();
 
	}

	//When the player gets far enough away from the door, check if they are in a room
	void OnTriggerExit(Collider col){
		if (col.tag == "Player"){
			CheckPlayerIsInRoom();
		}
	}

	void CheckPlayerIsInRoom() {
		if (player.position.z > transform.position.z)
		{
			SealRoom();
		}
	}

	void SealRoom() {
		foreach (BoxCollider boxCol in GetComponents<BoxCollider>())
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
		GetComponent<MeshRenderer>().enabled = true;
		GetComponent<NavMeshObstacle>().enabled = true;
        if(boss_Type == Boss_Type.LASER)
        {
            //spawns laser boss
            boss_GO = GameObject.Find("Laser_Boss").gameObject;
            boss_Pos = GameObject.FindGameObjectWithTag("Laser_Pos");
            // boss_GO.transform.SetParent(GameObject.FindGameObjectWithTag("Boss_Room").transform);
            boss_GO.SetActive(false);
            boss_GO.transform.position = new Vector3(365.5667f, -18.6f, 65.15f);
            boss_GO.gameObject.SetActive(true);

            //  boss_GO.transform.position = GameObject.FindGameObjectWithTag("Laser_Pos").transform.position;
            boss_GO.GetComponent<Laser_Boss_Spawn>().Spawn();
        }

        else
        {
            //spawns multi disc boss             
        }
        
      



        // boss_GO.SetActive(true);
        //Putting Door on Environment layer
        gameObject.layer = 14;

		cam.SetNewTarget(transform, (Vector3.forward * 15f));

		StartCoroutine(shootingScript.Recall());
		moveScript.LockHover(true);
	}
} 