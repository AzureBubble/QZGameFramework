using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class CustomHierarchy : MonoBehaviour
{
    private static StaticEditorFlags selectedFlags;

    static CustomHierarchy()
    {
        // 监听Hierarchy窗口绘制事件
        EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
    }

    private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (gameObject != null)
        {
            // 计算复选框的位置
            float checkboxPosX = selectionRect.x + selectionRect.width - 20f; // 120
            Rect checkboxRect = new Rect(checkboxPosX, selectionRect.y, 20f, selectionRect.height);

            // 获取物体的Active状态
            bool isActive = gameObject.activeSelf;

            // 绘制复选框，并根据其状态设置物体的Active状态
            EditorGUI.BeginChangeCheck();
            isActive = EditorGUI.Toggle(checkboxRect, isActive);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(gameObject, "Toggle Active State");
                gameObject.SetActive(isActive);
            }

            // 绘制StaticEditorFlags的下拉框
            //Rect dropdownRect = new Rect(selectionRect.x + selectionRect.width - 100f, selectionRect.y, 100f, selectionRect.height);
            //EditorGUI.BeginChangeCheck();
            //selectedFlags = (StaticEditorFlags)EditorGUI.EnumFlagsField(dropdownRect, selectedFlags);

            //// 检查下拉框是否有变化
            //if (EditorGUI.EndChangeCheck())
            //{
            //    Undo.RecordObject(gameObject, "Change Static Flags");
            //    GameObjectUtility.SetStaticEditorFlags(gameObject, selectedFlags);
            //}
        }
    }
}