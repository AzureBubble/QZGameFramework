using QZGameFramework.GameTool;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 重绘 DialogueData_SO 类型数据的 Inspector 窗口
/// </summary>
[CustomEditor(typeof(DialogueData_SO))]
public class DialogueCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open In Editor"))
        {
            // target 表示当前选择的物体
            //GameTool.OpenDialogueToolWindow((DialogueData_SO)target);
        }
        base.OnInspectorGUI();
    }
}