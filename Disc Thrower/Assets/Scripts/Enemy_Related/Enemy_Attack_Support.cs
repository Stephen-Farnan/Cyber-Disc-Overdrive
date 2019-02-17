using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack_Support : MonoBehaviour
{


    public int number_Of_Shields;
    public Enemy_AI local_Enemy_AI;


    public enum Support_Type
    {
        SHIELDER,
        HEALER
    }

    public Support_Type local_Support_Type;

    public void Attack()
    {

        Find_Targets();
    }

    void Find_Targets()
    {
        GameObject[] curr_Enemies;
        curr_Enemies = Get_Curr_Enemies();
        int loop_Length;
        if (number_Of_Shields < curr_Enemies.Length)
        {
            loop_Length = number_Of_Shields;
        }

        else
        {
            loop_Length = curr_Enemies.Length;
        }
        for (int j = 0; j < loop_Length; j++)
        {
            if (curr_Enemies[j] != gameObject && curr_Enemies[j] != null)
            {
                if (curr_Enemies[j].activeInHierarchy)
                {
                    if (curr_Enemies[j].GetComponent<Enemy_AI>() != null)
                    {
                        switch (local_Support_Type)
                        {
                            case Support_Type.HEALER:

                                curr_Enemies[j].GetComponent<Enemy_AI>().Heal_From_Support();
                                break;

                            case Support_Type.SHIELDER:
                                curr_Enemies[j].GetComponent<Enemy_AI>().Turn_On_Shield_From_Support();
                                break;

                            default:

                                break;
                        }
                    }
                    

                }
            }

        }

        local_Enemy_AI.StartCoroutine("Rest", local_Enemy_AI.rest_Duration);
    }


    GameObject[] Get_Curr_Enemies()
    {
        GameObject[] all_Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] curr_Enemies = new GameObject[all_Enemies.Length];
        int i = 0;
        if (all_Enemies != null)
        {
            foreach (GameObject go in all_Enemies)
            {
                if (go.activeInHierarchy && go != gameObject)
                {
                    curr_Enemies[i] = go;
                    i++;
                }
            }
        }

        return curr_Enemies;


    }

}
