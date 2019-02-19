using UnityEngine;

public class Music_Initialiser : MonoBehaviour
{


    /// <summary>
    /// Stops the music player from despawning from level to level
    /// </summary>
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

}
