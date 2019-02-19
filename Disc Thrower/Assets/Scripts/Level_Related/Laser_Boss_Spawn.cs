using System.Collections;
using UnityEngine;

public class Laser_Boss_Spawn : MonoBehaviour
{

    public GameObject go;

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
