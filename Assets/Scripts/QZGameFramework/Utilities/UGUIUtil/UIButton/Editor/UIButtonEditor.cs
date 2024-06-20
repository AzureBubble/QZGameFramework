using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QZGameFramework.Utilities.UGUIUtil
{
    public class UIButtonEditor
    {
        [MenuItem("GameObject/UI/UI Button", priority = 32)]
        public static void CreateTextPro()
        {
            // 创建 UI Text 物体
            GameObject root = new GameObject("UI Button", typeof(RectTransform), typeof(UIButton));
            // 设置 UI Text 作为 Canvas 的子物体
            UnityEditorUtility.ResetInCanvasFor((RectTransform)root.transform);
        }
    }
}