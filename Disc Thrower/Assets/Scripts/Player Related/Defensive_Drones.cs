using UnityEngine;

public class Defensive_Drones : MonoBehaviour
{


    public GameObject player;
    public GameObject Parent;
    public float speed = 2f;



    // Update is called once per frame
    void Update()
    {
        Parent.transform.position = player.transform.position;
        transform.RotateAround(player.transform.position, Vector3.up, speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy_Projectile" || other.gameObject.tag == "Enemy")
        {

            gameObject.SetActive(false);
            if (other.gameObject.tag == "Enemy_Projectile")
            {
                other.gameObject.SetActive(false);
            }

            else
            {
                other.gameObject.GetComponent<HealthScript>().LoseHealth(2);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("here");
    }
}
