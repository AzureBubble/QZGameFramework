using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QZGameFramework.Utilities
{
    public class ClassEditorHelper
    {
        /// <summary>
        /// 获取 type 类型脚本的 classID
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int GetClassID(System.Type type)
        {
            GameObject go = EditorUtility.CreateGameObjectWithHideFlags("Temp", HideFlags.HideAndDontSave);
            Component uiSprite = go.AddComponent(type);
            SerializedObject ob = new SerializedObject(uiSprite);
            int classID = ob.FindProperty("m_Script").objectReferenceInstanceIDValue;
            GameObject.DestroyImmediate(go);
            return classID;
        }

        /// <summary>
        /// 根据泛型获取脚本的 classID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int GetClassID<T>() where T : MonoBehaviour
        {
            return GetClassID(typeof(T));
        }

        /// <summary>
        /// 根据脚本类型ID 替换脚本
        /// </summary>
        /// <param name="mb"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static SerializedObject ReplaceClass(MonoBehaviour mb, System.Type type)
        {
            int id = GetClassID(type);
            SerializedObject ob = new SerializedObject(mb);
            ob.Update();
            ob.FindProperty("m_Script").objectReferenceInstanceIDValue = id;
            ob.ApplyModifiedProperties();
            ob.Update();
            return ob;
        }
    }
}