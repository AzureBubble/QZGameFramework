using UnityEditor;
using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute
{
    public bool isReadOnly = true;
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //{
    //    GUI.enabled = false;
    //    EditorGUI.PropertyField(position, property, label, true);
    //    GUI.enabled = true;
    //}

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ReadOnlyAttribute readOnlyAttribute = (ReadOnlyAttribute)attribute;
        readOnlyAttribute.isReadOnly = EditorGUI.Toggle(new Rect(position.x + position.width - 20, position.y, 20, position.height), readOnlyAttribute.isReadOnly);
        GUI.enabled = !readOnlyAttribute.isReadOnly;
        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width - 20, position.height), property, label, true);
        GUI.enabled = true;
    }
}

#endif