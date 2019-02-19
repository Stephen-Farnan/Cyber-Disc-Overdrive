using UnityEngine;

public class Inner_Sphere_Collision : MonoBehaviour
{

    /// <summary>
    /// This class exists to attach to the sphere objects caused by the crowd control enemy and handle its collision flags with the player
    /// </summary>

    public Enemy_Attack_Crowd_Control local_Enemy_Attack_Crowd_Control;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
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
