using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multi_Disc_Room_Colliders : MonoBehaviour {


    Multi_Disc_Boss_Manager local_Multi_Disc_Boss_Manager;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Disc")
        {
            local_Multi_Disc_Boss_Manager.number_Of_Discs--;
            if(local_Multi_Disc_Boss_Manager.number_Of_Discs <= 0)
            {
                local_Multi_Disc_Boss_Manager.all_Discs_Are_Destroyed = true;
            }
            //destroy the disc and return it to the pool to be used again once all discs are destroyed, also check here whether the discs are all gone yet or not then call reset discs coroutine
            if (local_Multi_Disc_Boss_Manager.all_Discs_Are_Destroyed)
            {
                //reset the discs
                local_Multi_Disc_Boss_Manager.StartCoroutine("Reset_Discs");
            }
        }
    }
}
