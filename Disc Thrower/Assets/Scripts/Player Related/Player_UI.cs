using UnityEngine;
using UnityEngine.UI;

public class Player_UI : MonoBehaviour
{

    public Image[] health = new Image[20];

    /// <summary>
    /// Matches health segments in the ui to player health value
    /// </summary>
    /// <param name="curr_Health">The players current health</param>
    public void Update_Health(int curr_Health)
    {
        for (int i = 0; i < health.Length; i++)
        {
            if (i < curr_Health)
            {
                health[i].enabled = true;
            }
            else
            {
                health[i].enabled = false;
            }
        }
    }
}
