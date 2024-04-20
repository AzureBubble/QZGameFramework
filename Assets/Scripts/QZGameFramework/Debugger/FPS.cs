using UnityEngine;

public class FPS : MonoBehaviour
{
    private float deltaTime = 0.0f;

    private GUIStyle mStyle;

    private void Awake()
    {
        mStyle = new GUIStyle();
        mStyle.alignment = TextAnchor.UpperLeft;
        mStyle.normal.background = null;
        mStyle.fontSize = 20;
        mStyle.normal.textColor = Color.green;
    }

    private void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        Rect rect = new Rect(0, 0, 500, 300);
        float fps = 1.0f / deltaTime;
        mStyle.normal.textColor = fps < 60 ? Color.red : Color.green;
        string text = string.Format(" FPS:{0:N0} ", fps);
        GUI.Label(rect, text, mStyle);
    }
}