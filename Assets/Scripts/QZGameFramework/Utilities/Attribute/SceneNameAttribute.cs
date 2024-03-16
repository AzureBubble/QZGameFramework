using System;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

/// <summary>
/// SceneName 特性
/// </summary>
public class SceneNameAttribute : PropertyAttribute
{
}

#if UNITY_EDITOR

/// <summary>
/// 针对哪个类型的 Property 在 Inspector 进行绘制
/// </summary>
[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public class SceneNameDrawer : PropertyDrawer
{
    private int newSceneIndex = -1; // 新场景的 Index
    private int oldSceneIndex; // 旧场景的 Index
    private GUIContent[] sceneNames; // 场景名字构成的 GUIContent 数组
    private readonly string[] scenePathSplit = new string[] { "/", ".unity" }; // 用于切割场景路径获得场景名字的切割字符串数组

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 如果 BuildSettings 中没有场景 则提示添加场景
        if (EditorBuildSettings.scenes.Length == 0)
        {
            newSceneIndex = 0;
            sceneNames = new GUIContent[] { new GUIContent("Check Your Build Settings To Add Scene") };
            property.stringValue = string.Empty;
        }

        // 场景序号为 -1 则需要进行初始化
        if (newSceneIndex == -1)
        {
            // 初始化获得场景名称数组
            GetSceneNameArray(property);
        }

        oldSceneIndex = newSceneIndex;
        // 保存当前选中的场景的索引值
        newSceneIndex = EditorGUI.Popup(position, label, newSceneIndex, sceneNames);

        if (oldSceneIndex != newSceneIndex)
        {
            // 如果索引改变了 才重新进行绘制
            property.stringValue = sceneNames[newSceneIndex].text;
        }
    }

    /// <summary>
    /// 获得当前 BuildSettings 中的所有数组
    /// </summary>
    /// <param name="property">标记这个特性的属性值</param>
    private void GetSceneNameArray(SerializedProperty property)
    {
        // 获得 BuildSettings 中的所有场景的路径数组
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        // 初始化场景 GUIContent 数组
        sceneNames = new GUIContent[scenes.Length];
        // 遍历 scenes 数组中的元素 进行字符串切割 获得场景的名字
        for (int i = 0; i < scenes.Length; i++)
        {
            // 获得第 i 个场景的路径
            string path = scenes[i].path;
            // 进行字符串切割
            string[] splitPath = path.Split(scenePathSplit, StringSplitOptions.RemoveEmptyEntries);
            string sceneName = "";
            if (splitPath.Length > 0)
            {
                // 获得切割数组中最后一个元素 也就是场景的名字
                sceneName = splitPath.Last();
            }
            else
            {
                sceneName = "(Delete Scene)";
            }
            // 把场景名字构造成 GUIContent 元素 添加到数组中
            sceneNames[i] = new GUIContent(sceneName);
        }

        if (string.IsNullOrEmpty(property.stringValue))
        {
            newSceneIndex = 0;
        }
        else
        {
            // 场景存在标识
            bool foundSceneName = false;
            for (int i = 0; i < sceneNames.Length; i++)
            {
                if (property.stringValue == sceneNames[i].text)
                {
                    // 如果选中的名字 在场景中存在 则修改新的索引值
                    newSceneIndex = i;
                    foundSceneName = true;
                    break;
                }
            }

            if (!foundSceneName)
            {
                // 如果不存在 则索引改为第 0 个
                newSceneIndex = 0;
            }
        }

        // 修改 Inspector 窗口中的属性值 为选中的场景名
        property.stringValue = sceneNames[newSceneIndex].text;
    }
}

#endif