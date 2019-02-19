using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu_Manager : MonoBehaviour
{

    public void Start_New_Game()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit_Game()
    {
        Application.Quit();
    }
}
