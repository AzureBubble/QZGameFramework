using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace QZGameFramework.Utilities.UGUIUtil
{
    [CustomEditor(typeof(UIButton), true)]
    [CanEditMultipleObjects]
    public class UIButtonEditor : ButtonEditor
    {
        private static bool m_ClickProtectPanelOpen = false;
        private static bool m_DoubleClickPanelOpen = false;
        private static bool m_LongPressPanelOpen = false;
        private static bool m_ClickScalePanelOpen = false;
        private static bool m_ClickSoundPanelOpen = false;

        private SerializedProperty m_isUseClickProtect;
        private SerializedProperty m_protectTime;

        private SerializedProperty m_isUseDoubleClick;
        private SerializedProperty m_clickInterval;
        private SerializedProperty m_doubleClickedEvent;

        private SerializedProperty m_isUseLongPress;
        private SerializedProperty m_duration;
        private SerializedProperty m_isLoopLongPress;
        private SerializedProperty m_interval;
        private SerializedProperty m_buttonLongPressEvent;

        private SerializedProperty m_isUseClickScale;
        private SerializedProperty m_normalScale;
        private SerializedProperty m_clickScale;

        private SerializedProperty m_isUseClickSound;
        private SerializedProperty m_clickSoundPath;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_ClickProtectPanelOpen = EditorPrefs.GetBool("UGUIPro.m_ClickProtectPanelOpen", m_ClickProtectPanelOpen);
            m_DoubleClickPanelOpen = EditorPrefs.GetBool("UGUIPro.m_DoubleClickPanelOpen", m_DoubleClickPanelOpen);
            m_LongPressPanelOpen = EditorPrefs.GetBool("UGUIPro.m_LongPressPanelOpen", m_LongPressPanelOpen);
            m_ClickSoundPanelOpen = EditorPrefs.GetBool("UGUIPro.m_ClickSoundPanelOpen", m_ClickSoundPanelOpen);
            m_ClickScalePanelOpen = EditorPrefs.GetBool("UGUIPro.m_ClickScalePanelOpen", m_ClickScalePanelOpen);

            m_isUseClickProtect = serializedObject.FindProperty("m_buttonClickProtectExtend.m_isUseClickProtect");
            m_protectTime = serializedObject.FindProperty("m_buttonClickProtectExtend.m_protectTime");

            m_isUseDoubleClick = serializedObject.FindProperty("m_buttonDoubleClickExtend.m_isUseDoubleClick");
            m_clickInterval = serializedObject.FindProperty("m_buttonDoubleClickExtend.m_clickInterval");
            m_doubleClickedEvent = serializedObject.FindProperty("m_buttonDoubleClickExtend.m_doubleClickedEvent");

            m_isUseLongPress = serializedObject.FindProperty("m_buttonLongPressExtend.m_isUseLongPress");
            m_duration = serializedObject.FindProperty("m_buttonLongPressExtend.m_duration");
            m_isLoopLongPress = serializedObject.FindProperty("m_buttonLongPressExtend.m_isLoopLongPress");
            m_interval = serializedObject.FindProperty("m_buttonLongPressExtend.m_interval");
            m_buttonLongPressEvent = serializedObject.FindProperty("m_buttonLongPressExtend.m_buttonLongPressEvent");

            m_isUseClickScale = serializedObject.FindProperty("m_buttonClickScaleExtend.m_isUseClickScale");
            m_normalScale = serializedObject.FindProperty("m_buttonClickScaleExtend.m_normalScale");
            m_clickScale = serializedObject.FindProperty("m_buttonClickScaleExtend.m_clickScale");

            m_isUseClickSound = serializedObject.FindProperty("m_buttonClickSoundExtend.m_isUseClickSound");
            m_clickSoundPath = serializedObject.FindProperty("m_buttonClickSoundExtend.m_clickSoundPath");
        }

        public override void OnInspectorGUI()
        {
            // 更新编辑器修改
            serializedObject.Update();
            // 绘制 UIButton
            UIButtonGUI();
            // 应用编辑器修改
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }

        public void UIButtonGUI()
        {
            UIButtonDrawEditor.DrawClickProtectGUI("连点保护模式", ref m_ClickProtectPanelOpen, m_isUseClickProtect, m_protectTime);
            UIButtonDrawEditor.DrawDoubleClickGUI("双击模式", ref m_DoubleClickPanelOpen, m_isUseDoubleClick, m_clickInterval, m_doubleClickedEvent);
            UIButtonDrawEditor.DrawLongPressGUI("长按模式", ref m_LongPressPanelOpen, m_isUseLongPress, m_duration, m_isLoopLongPress, m_interval, m_buttonLongPressEvent);
            UIButtonDrawEditor.DrawClickScaleGUI("点击缩放", ref m_ClickScalePanelOpen, m_isUseClickScale, m_normalScale, m_clickScale);
            UIButtonDrawEditor.DrawClickSoundGUI("按钮音效", ref m_ClickSoundPanelOpen, m_isUseClickSound, m_clickSoundPath);
            if (GUI.changed)
            {
                EditorPrefs.SetBool("UGUIPro.m_ClickProtectPanelOpen", m_ClickProtectPanelOpen);
                EditorPrefs.SetBool("UGUIPro.m_DoubleClickPanelOpen", m_DoubleClickPanelOpen);
                EditorPrefs.SetBool("UGUIPro.m_LongPressPanelOpen", m_LongPressPanelOpen);
                EditorPrefs.SetBool("UGUIPro.m_ClickScalePanelOpen", m_ClickScalePanelOpen);
                EditorPrefs.SetBool("UGUIPro.m_ClickSoundPanelOpen", m_ClickSoundPanelOpen);
            }
        }
    }
}