using System.Collections;
using UnityEngine;

public class RemoveMask : MonoBehaviour
{

    public GameObject mask;
    public float maskClearSpeed = 1;

    private bool hasBeenTriggered = false;

    void OnTriggerEnter(Collider col)
    {
        if (!hasBeenTriggered)
        {
            if (col.tag == "Player")
            {
                StartCoroutine(FadeOutMask());
                hasBeenTriggered = true;
            }
        }
    }

    /// <summary>
    /// Removes Mask over level over time
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeOutMask()
    {
        Material mat = mask.GetComponent<MeshRenderer>().material;
        float baseRad = mat.GetFloat("_radius");
        float alph = mat.GetFloat("_multiplier");
        float rad = baseRad;
        while (alph > 0)
        {
            alph -= Time.deltaTime * maskClearSpeed;
            mat.SetFloat("_multiplier", alph);

            rad += Time.deltaTime * maskClearSpeed;
            mat.SetFloat("_radius", rad);
            yield return null;
        }
    }
}
