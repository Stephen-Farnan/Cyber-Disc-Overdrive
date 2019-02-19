using UnityEngine;

public class FPS_COUNTER : MonoBehaviour
{
    /// <summary>
    /// This class simply outputs a more accurate fps reading than unitys default reading
    /// </summary>

    #region variables
    [HideInInspector]
    public float deltaTime = 0.0f;
    #endregion

    void Start()
    {


    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;


    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(1f, 1f, 1f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}