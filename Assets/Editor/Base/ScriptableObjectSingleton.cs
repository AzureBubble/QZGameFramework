using System;
using System.IO;
using System.Reflection;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// SO文件单例类
/// </summary>
/// <typeparam name="T"></typeparam>
public class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Load();
            }

            return instance;
        }
    }

    /// <summary>
    /// 加载实例
    /// </summary>
    /// <returns></returns>
    private static T Load()
    {
        // 获取 SO 文件存放路径
        string scriptableObjectPath = GetScriptableObjectPath();
        if (string.IsNullOrEmpty(scriptableObjectPath))
        {
            return null;
        }

        // 加载 scriptableObjectPath 路径下的 序列化资源文件
        var loadSerializedFileAndForget = InternalEditorUtility.LoadSerializedFileAndForget(scriptableObjectPath);

        if (loadSerializedFileAndForget.Length <= 0)
        {
            // 创建对应 T 类型的实例对象返回给单例对象
            return CreateInstance<T>();
        }

        // 返回获取的序列化文件 给单例对象
        return loadSerializedFileAndForget[0] as T;
    }

    /// <summary>
    /// 保存 SO 文件到本地
    /// </summary>
    /// <param name="saveAsText">是否以文本形式存储</param>
    public static void Save(bool saveAsText = true)
    {
        if (instance == null)
        {
            Debug.LogError("无法保存ScriptableObjectSingleton:没有ScriptableObjectSingleton的静态实例");
            return;
        }

        string scriptableObjectPath = GetScriptableObjectPath();

        if (string.IsNullOrEmpty(scriptableObjectPath))
        {
            return;
        }

        // 获取 scriptableObjectPath 目录路径
        string directoryName = Path.GetDirectoryName(scriptableObjectPath);

        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        // 创建一个 instance 的Object数组对象 用于保存到本地
        UnityEngine.Object[] obj = { instance };
        // 将 obj 数组文件保存到路径中 true以文件格式保存 false以二进制保存
        InternalEditorUtility.SaveToSerializedFileAndForget(obj, scriptableObjectPath, saveAsText);
    }

    /// <summary>
    /// 必须用 ScriptableObjectPathAttribute 特性标记
    /// 获得 ScriptableObject 的存放路径
    /// </summary>
    /// <returns></returns>
    private static string GetScriptableObjectPath()
    {
        // 获得当前实例的自定义特性 ScriptableObjectPathAttribute
        var scriptableObjectPathAttribute = typeof(T).GetCustomAttribute(typeof(ScriptableObjectPathAttribute)) as ScriptableObjectPathAttribute;

        return scriptableObjectPathAttribute?.ScriptableObjectPath;
    }
}

/// <summary>
/// 获得 SO 文件资源存放路径的 Attribute
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ScriptableObjectPathAttribute : Attribute
{
    internal readonly string ScriptableObjectPath;

    public ScriptableObjectPathAttribute(string scriptableObjectPath)
    {
        if (string.IsNullOrEmpty(scriptableObjectPath))
        {
            throw new ArgumentNullException("无效路径");
        }

        if (scriptableObjectPath[0] == '/')
        {
            // 去掉路径的第一个 '/'
            scriptableObjectPath = scriptableObjectPath.Substring(1);
        }

        ScriptableObjectPath = scriptableObjectPath;
    }
}