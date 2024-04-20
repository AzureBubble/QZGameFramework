using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class LogEditor
{
    [MenuItem("GameTool/DebuggerSystem/OpenDebuggerSystem")]
    public static void LoadReport()
    {
        ScriptingDefineSymbols.AddScriptingDefineSymbol("OPEN_LOG");
        GameObject reporterObj = GameObject.Find("Reporter");
        if (reporterObj == null)
        {
            reporterObj = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ThirdPackages/Common Collections/UnityDebuger/Unity-Logs-Viewer/Reporter.prefab"));
            reporterObj.name = "Reporter";
            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            AssetDatabase.Refresh();
            Debug.Log("Open Log Finish!");
            Debug.Log("Open Log Finish!");
        }
    }

    [MenuItem("GameTool/DebuggerSystem/CloseDebuggerSystem")]
    public static void CloseReport()
    {
        ScriptingDefineSymbols.RemoveScriptingDefineSymbol("OPEN_LOG");
        GameObject reporterObj = GameObject.Find("Reporter");
        if (reporterObj != null)
        {
            GameObject.DestroyImmediate(reporterObj);
            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            AssetDatabase.Refresh();
            Debug.Log("Close Log Finish!");
        }
    }
}