using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{

    public float baseChanceToSpawnHealthRegen = 20;
    [HideInInspector]
    public float chanceToSpawnHealthRegen;
    private int healthRegenSpawns = 0;

    private bool levelCompleted = false;
    private bool hasBossKey = false;

    public bool HasBossKey
    {
        get
        {
            return hasBossKey;
        }
        set
        {
            hasBossKey = value;
        }
    }

    private Text messageText;

    void Awake()
    {
        messageText = GameObject.Find("Message Text").GetComponent<Text>();
        chanceToSpawnHealthRegen = baseChanceToSpawnHealthRegen;
    }

    /// <summary>
    /// Updates text displayed to the screen
    /// </summary>
    /// <param name="message">What text needs to be shown to the player</param>
    /// <returns></returns>
    public IEnumerator DisplayMessage(string message)
    {
        messageText.text = message;
        messageText.enabled = true;

        yield return new WaitForSeconds(2);

        if (!levelCompleted)
        {
            messageText.enabled = false;
        }
    }

    /// <summary>
    /// Increase the amount of health regen we can spawn in the level
    /// </summary>
    public void AddHealthRegen()
    {
        healthRegenSpawns++;
        chanceToSpawnHealthRegen /= 2;
    }
}
