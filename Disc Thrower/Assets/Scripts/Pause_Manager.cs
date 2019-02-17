using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pause_Manager : MonoBehaviour {

    bool game_Is_Paused = false;
    float curr_Timescale;
    public GameObject pause_Menu;
    public Player_Input local_Player_Input;
    bool input_Was_Paused = false;

    public void Toggle_Pause()
    {

        if (game_Is_Paused)
        {
            Time.timeScale = curr_Timescale;
            game_Is_Paused = false;
            pause_Menu.SetActive(false);
            if (input_Was_Paused)
            {
                local_Player_Input.input_Enabled = true;
            }
            input_Was_Paused = false;
        }

        else
        {
            curr_Timescale = Time.timeScale;
            Time.timeScale = 0;
            if (local_Player_Input.input_Enabled)
            {
                local_Player_Input.input_Enabled = false;
                input_Was_Paused = true;
            }
            game_Is_Paused = true;
            pause_Menu.SetActive(true);
        }
    }

    public void Quit_To_Menu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Quit_To_Windows()
    {
        Application.Quit();
    }

}
