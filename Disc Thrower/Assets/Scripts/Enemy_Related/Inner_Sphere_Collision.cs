using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inner_Sphere_Collision : MonoBehaviour {


    public Enemy_Attack_Crowd_Control local_Enemy_Attack_Crowd_Control;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            local_Enemy_Attack_Crowd_Control.inner_Ring_Triggered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            local_Enemy_Attack_Crowd_Control.inner_Ring_Triggered = false;
        }
    }
}
