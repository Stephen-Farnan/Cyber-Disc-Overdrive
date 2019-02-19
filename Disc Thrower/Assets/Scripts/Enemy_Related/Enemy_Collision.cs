using UnityEngine;

public class Enemy_Collision : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //hurt player
        }
    }
}
