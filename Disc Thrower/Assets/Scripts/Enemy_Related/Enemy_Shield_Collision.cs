using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Shield_Collision : MonoBehaviour {


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject);
        if(collision.gameObject.tag == "Disc")
        {
            Debug.Log("hit");
            gameObject.SetActive(false);
        }
    }


}
