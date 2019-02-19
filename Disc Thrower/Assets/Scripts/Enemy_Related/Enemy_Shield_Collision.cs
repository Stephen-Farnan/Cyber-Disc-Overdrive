using UnityEngine;

public class Enemy_Shield_Collision : MonoBehaviour
{

    /// <summary>
    /// This class operates on enemy gameobjects and simply sets an active shield to inactive if it gets hit by the disc
    /// </summary>
    /// <param name="collision"></param>

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Disc")
        {
            gameObject.SetActive(false);
        }
    }


}
