using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser_Collision : MonoBehaviour {



    /// <summary>
    /// hit the player and call damage
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<HealthScript>().LoseHealth(1);
        }
    }
}
