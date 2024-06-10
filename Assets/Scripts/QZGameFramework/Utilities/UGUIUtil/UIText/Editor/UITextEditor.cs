using QZGameFramework.Utilities.UGUIUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace QZGameFramework.Utilities.UGUIUtil
{
    public class UITextEditor
    {
        [MenuItem("GameObject/UI/UI Text", priority = 0)]
        public static void CreateTextPro()
        {
            // 创建 UI Text 物体
            GameObject root = new GameObject("UI Text", typeof(RectTransform), typeof(UIText));
            // 设置 UI Text 作为 Canvas 的子物体
            UnityEditorUtility.ResetInCanvasFor((RectTransform)root.transform);
        }
    }
}