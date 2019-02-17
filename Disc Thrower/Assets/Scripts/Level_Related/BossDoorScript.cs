using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossDoorScript : MonoBehaviour {

	[HideInInspector]
	public LevelManager levMan;
	public AudioClip[] clips;
	private AudioSource audSource;
	private ScreenFade fadeScript;


	private Transform bossRoomSpawnPoint;

	void Awake()
	{
		levMan = GameObject.Find("Level Generator").GetComponent<LevelManager>();
		audSource = GetComponent<AudioSource>();
		if (clips[0] != null)
		{
			audSource.clip = clips[0];
		}
		fadeScript = GameObject.FindWithTag("Fade").GetComponent<ScreenFade>();
	}

	void Start()
	{
		audSource.Play();
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player")
		{
			if (levMan.HasBossKey)
			{
				StartCoroutine(OpenBossDoor());
			}
			else {
				StartCoroutine(levMan.DisplayMessage("Key Required"));
			}
		}
	}

	public void SetBossRoomSpawnPoint(Transform spawnPoint) {
		bossRoomSpawnPoint = spawnPoint;
	}

	private void Update(){
		if (DebugHandler.debugEnabled)
		{
			if (Input.GetKeyDown(KeyCode.B))
			{
				StartCoroutine(OpenBossDoor());
			}
		}
	}

	IEnumerator OpenBossDoor() {
		GetComponent<MeshRenderer>().enabled = false;
		GetComponent<BoxCollider>().enabled = false;
		GetComponent<NavMeshObstacle>().enabled = false;

		audSource.clip = clips[1];
		audSource.Play();

		//Deactivate player movement
		GameObject player = GameObject.FindWithTag("Player");
		player.GetComponent<Player_Input>().input_Enabled = false;
		player.GetComponent<NavMeshAgent>().enabled = false;
		player.GetComponent<CapsuleCollider>().enabled = false;

		ShaderPositionUpdate.SetRevealer(false);

		//Fading Out
		StartCoroutine (fadeScript.FadeOut());
		yield return new WaitForSeconds(fadeScript.GetFadeTime());


		//Move player
		player.transform.position = bossRoomSpawnPoint.position;

		player.GetComponent<Player_Input>().input_Enabled = true;
		player.GetComponent<NavMeshAgent>().enabled = true;
		player.GetComponent<CapsuleCollider>().enabled = true;

		Transform cam = GameObject.FindWithTag("MainCamera").transform;
		cam.position = new Vector3 (player.transform.position.x, cam.position.y, player.transform.position.z);

		//Fading In
		StartCoroutine(fadeScript.FadeIn());
		yield return new WaitForSeconds(fadeScript.GetFadeTime());

		//StartCoroutine(levMan.DisplayMessage("Level Completed"));
	}
}
