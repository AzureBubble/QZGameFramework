using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace QZGameFramework.Utilities.UGUIUtil
{
    public static class UIButtonDrawEditor
    {
        [MenuItem("GameObject/UI/UI Button", priority = 32)]
        public static void CreateTextPro()
        {
            // 创建 UI Button 物体
            RectTransform root = new GameObject("UI Button", typeof(RectTransform), typeof(UIImage), typeof(UIButton)).GetComponent<RectTransform>();
            Text text = new GameObject("UI Text", typeof(RectTransform), typeof(UIText)).GetComponent<Text>();
            // 设置 UI Button 作为 Canvas 的子物体
            UnityEditorUtility.ResetInCanvasFor(root);
            text.transform.SetParent(root);
            text.transform.localPosition = Vector3.zero;
            text.transform.localRotation = Quaternion.identity;
            text.transform.localScale = Vector3.one;
            text.color = Color.black;
            text.fontSize = 24;
            text.alignment = TextAnchor.MiddleCenter;
            text.raycastTarget = false;
            text.supportRichText = false;
            text.text = "UI Button";
            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMax = Vector2.one;
            textRect.anchorMin = Vector2.zero;
            root.sizeDelta = new Vector2(163, 50);
            root.localPosition = Vector3.zero;
        }

        public static void DrawClickProtectGUI(string title, ref bool panelOpen, SerializedProperty isUseClickProtect, SerializedProperty protectTime)
        {
            UnityEditorUtility.LayoutFrameBox(() =>
            {
                EditorGUILayout.PropertyField(isUseClickProtect, new GUIContent("开启连点保护模式"));
                if (isUseClickProtect.boolValue)
                {
                    EditorGUILayout.PropertyField(protectTime, new GUIContent("连点保护时间"));
                }
            }, title, ref panelOpen, true);
        }

        public static void DrawDoubleClickGUI(string title, ref bool panelOpen, SerializedProperty isUseDoubleClick, SerializedProperty clickInterval, SerializedProperty callback)
        {
            UnityEditorUtility.LayoutFrameBox(() =>
            {
                EditorGUILayout.PropertyField(isUseDoubleClick, new GUIContent("开启双击模式"));
                if (isUseDoubleClick.boolValue)
                {
                    EditorGUILayout.PropertyField(clickInterval, new GUIContent("有效间隔时间"));
                    EditorGUILayout.PropertyField(callback, new GUIContent("双击触发回调"));
                }
            }, title, ref panelOpen, true);
        }

        public static void DrawLongPressGUI(string title, ref bool panelOpen, SerializedProperty isUseLongPress, SerializedProperty duration, SerializedProperty isLoopLongPress, SerializedProperty interval, SerializedProperty callback)
        {
            UnityEditorUtility.LayoutFrameBox(() =>
            {
                EditorGUILayout.PropertyField(isUseLongPress, new GUIContent("开启长按模式"));
                if (isUseLongPress.boolValue)
                {
                    EditorGUILayout.PropertyField(duration, new GUIContent("长按触发时间"));
                    EditorGUILayout.PropertyField(isLoopLongPress, new GUIContent("开启循环触发模式"));
                    if (isLoopLongPress.boolValue)
                    {
                        EditorGUILayout.PropertyField(interval, new GUIContent("长按触发间隔"));
                    }
                    EditorGUILayout.PropertyField(callback, new GUIContent("长按触发回调"));
                }
            }, title, ref panelOpen, true);
        }

        public static void DrawClickScaleGUI(string title, ref bool panelOpen, SerializedProperty isUseClickScale, SerializedProperty normalScale, SerializedProperty clickScale)
        {
            UnityEditorUtility.LayoutFrameBox(() =>
            {
                EditorGUILayout.PropertyField(isUseClickScale, new GUIContent("开启点击缩放"));
                if (isUseClickScale.boolValue)
                {
                    EditorGUILayout.PropertyField(normalScale, new GUIContent("默认缩放"));
                    EditorGUILayout.PropertyField(clickScale, new GUIContent("按下缩放"));
                }
            }, title, ref panelOpen, true);
        }

        public static void DrawClickSoundGUI(string title, ref bool panelOpen, SerializedProperty isUseClickSound, SerializedProperty clickSoundPath)
        {
            UnityEditorUtility.LayoutFrameBox(() =>
            {
                EditorGUILayout.PropertyField(isUseClickSound, new GUIContent("开启音效"));
                if (isUseClickSound.boolValue)
                {
                    EditorGUILayout.BeginHorizontal();
                    //EditorGUILayout.LabelField("音效路径", GUILayout.Width(125));
                    clickSoundPath.stringValue = EditorGUILayout.TextField(new GUIContent("音效路径"), clickSoundPath.stringValue);
                    EditorGUILayout.EndHorizontal();
                }
            }, title, ref panelOpen, true);
        }
    }
}