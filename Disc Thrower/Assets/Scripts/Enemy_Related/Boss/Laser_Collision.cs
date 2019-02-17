using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser_Collision : MonoBehaviour {



    private void OnCollisionEnter(Collision collision)
    {
        //hit the player and call damage
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<HealthScript>().LoseHealth(1);
        }
    }
}
