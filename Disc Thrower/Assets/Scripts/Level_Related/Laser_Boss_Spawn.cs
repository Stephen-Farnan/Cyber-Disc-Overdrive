using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser_Boss_Spawn : MonoBehaviour {

    public GameObject go;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Spawn()
    {
        StartCoroutine(Spawning());
    }

    IEnumerator Spawning()
    {
        yield return new WaitForSeconds(.1f);
        go.SetActive(true);
    }
}
