using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Score : MonoBehaviour {

	public enum Current_Level
    {
        LEVEL_ONE,
        LEVEL_TWO,
        LEVEL_THREE,
        LEVEL_FOUR,
        LEVEL_FIVE,
        LEVEL_SIX,
        LEVEL_SEVEN,
        LEVEL_EIGHT
    }

    public Current_Level local_Current_Level;
    

    [Range(0, 10)]
    public int max_Multiplier = 5;
    public int current_Multiplier = 0;
    public int current_Total_Score;
    public int current_Combo_Score;
    public int number_Of_Enemies_Needed_To_Increase_Multiplier;
    int temp_Number_Of_Enemies_Killed;
    public Text total_Score;
    public Text current_Combo;

    public void Add_To_Total_Score()
    {
        current_Total_Score += current_Combo_Score;
        current_Combo_Score = 0;
    }

    public int Add_To_Combo_Score(int amount)
    {
        current_Combo_Score = current_Combo_Score * 2;
        current_Combo_Score += amount;

        return current_Combo_Score;
    }

    public void Reset_Mutliplier()
    {
        current_Multiplier = 1;
    }


    private void Update()
    {
        total_Score.text = current_Total_Score.ToString();
    }


    public void Add_To_Multiplier()
    {
        current_Multiplier++;
        if(current_Multiplier > max_Multiplier)
        {
            current_Multiplier = max_Multiplier;
        }
    }


}
