using System.Collections;
using UnityEngine;

public class Death_Manager : MonoBehaviour
{

    private float end_Screen_Text_Duration = 2.5f;
    public GameObject end_Screen_Text;

    /// <summary>
    /// Disables player input and starts to load the next level
    /// </summary>
    /// <returns></returns>
    public IEnumerator End_Level()
    {
        end_Screen_Text.SetActive(true);
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(end_Screen_Text_Duration);
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        end_Screen_Text.SetActive(false);
    }
}
