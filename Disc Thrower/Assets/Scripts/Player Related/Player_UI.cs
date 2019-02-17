using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_UI : MonoBehaviour {

	public Image[] health = new Image[20];

    public void Update_Health(int curr_Health)
    {
		for (int i = 0; i < health.Length; i++)
		{
			if (i < curr_Health)
			{
				health[i].enabled = true;
			}
			else {
				health[i].enabled = false;
			}
		}
    }
}
