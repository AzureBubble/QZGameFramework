using QZGameFramework.MonoManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using UnityEngine.Events;

/// <summary>
/// 管理全局的 Singleton
/// </summary>
public static class SingletonManager
{
    private static bool isInitialize; // 是否初始化单例管理器
    private static List<IUpdateSingleton> updateSingletons = new List<IUpdateSingleton>(50);
    private static Dictionary<Type, ISingleton> singletons = new Dictionary<Type, ISingleton>(50);
    private static MonoController monoController;
    private static bool isDirty; // 是否排序 updateSingletons
    private static bool needToDestory;

    /// <summary>
    /// 初始化 Singleton 管理器
    /// </summary>
    public static void Initialize()
    {
        if (isInitialize) return;

        isInitialize = true;
        GameObject obj = new GameObject("MonoController");
        monoController = obj.AddComponent<MonoController>();
        monoController?.AddUpdateListener(OnUpdate);
    }

    /// <summary>
    /// 帧更新方法
    /// </summary>
    private static void OnUpdate()
    {
        if (updateSingletons.Count == 0) return;

        if (isDirty)
        {
            isDirty = false;
            updateSingletons.Sort((left, right) =>
            {
                if (left.Priority > right.Priority) return -1;
                else if (left.Priority == right.Priority) return 0;
                else return 1;
            });
        }

        if (needToDestory)
        {
            return;
        }

        for (int i = 0; i < updateSingletons.Count; i++)
        {
            updateSingletons[i]?.OnUpdate();
            if (needToDestory)
            {
                needToDestory = false;
                i--;
            }
        }
    }

    /// <summary>
    /// 创建单例对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T CreateSingleton<T>() where T : class, ISingleton
    {
        Type type = typeof(T);
        if (singletons.ContainsKey(type))
        {
            return singletons[type] as T;
        }

        T singleton = Activator.CreateInstance<T>();
        //singleton.Initialize();

#if UNITY_EDITOR
        //Debug.Log(type.Name + " Singleton has been Created.");
#endif

        if (singleton is IUpdateSingleton updateSingleton)
        {
            updateSingletons.Add(updateSingleton);
            isDirty = true;
        }

        singletons.Add(type, singleton);
        return singleton;
    }

    /// <summary>
    /// 获取单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetSingleton<T>() where T : class, ISingleton
    {
        if (ContainsSingleton<T>())
        {
            if (ContainsUpdateSingleton<T>())
            {
                foreach (var item in updateSingletons)
                {
                    if (item.GetType() == typeof(T))
                    {
                        return item as T;
                    }
                }
            }
            return singletons[typeof(T)] as T;
        }
        return null;
    }

    /// <summary>
    /// 是否存在普通单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool ContainsSingleton<T>() where T : class, ISingleton
    {
        if (singletons.ContainsKey(typeof(T)))
        {
            return true;
        }
#if UNITY_EDITOR
        Debug.Log($"不存在{typeof(T)}单例对象");
#endif
        return false;
    }

    /// <summary>
    /// 是否存在 UpdateSingleton
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool ContainsUpdateSingleton<T>() where T : class, ISingleton
    {
        foreach (var item in updateSingletons)
        {
            if (item.GetType() == typeof(T))
            {
                return true;
            }
        }
#if UNITY_EDITOR
        Debug.Log($"不存在{typeof(T)}Update单例对象");
#endif
        return false;
    }

    /// <summary>
    /// 删除某一特定单例对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool DestorySingleton<T>() where T : class, ISingleton
    {
        Type type = typeof(T);
        return DestorySingleton(type);
    }

    /// <summary>
    /// 删除某一特定单例对象
    /// </summary>
    /// <returns></returns>
    private static bool DestorySingleton(Type type)
    {
        if (singletons.ContainsKey(type))
        {
            needToDestory = true;
            ISingleton tempSingleton = singletons[type];
            if (tempSingleton is IUpdateSingleton updateSingleton)
            {
                updateSingletons.Remove(updateSingleton);
            }

            tempSingleton.Dispose();
            singletons.Remove(type);
#if UNITY_EDITOR
            Debug.Log($"The singleton has been deleted. SingletonName: {type.Name}");
#endif
            needToDestory = false;
            return true;
        }

        return false;
    }

    #region Mono声明周期函数监听

    public static void AddUpdateListener(UnityAction action)
    {
        monoController.AddUpdateListener(action);
    }

    public static void RemoveUpdateListener(UnityAction action)
    {
        monoController.RemoveUpdateListener(action);
    }

    public static void AddFixedUpdateListener(UnityAction action)
    {
        monoController.AddFixedUpdateListener(action);
    }

    public static void RemoveFixedUpdateListener(UnityAction action)
    {
        monoController.RemoveFixedUpdateListener(action);
    }

    public static void AddLateUpdateListener(UnityAction action)
    {
        monoController.AddLateUpdateListener(action);
    }

    public static void RemoveLateUpdateListener(UnityAction action)
    {
        monoController.RemoveLateUpdateListener(action);
    }

    #endregion

    #region 开启协程

    public static Coroutine StartCoroutine(IEnumerator coroutine)
    {
        return monoController.StartCoroutine(coroutine);
    }

    public static Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value)
    {
        return monoController.StartCoroutine(methodName, value);
    }

    public static Coroutine StartCoroutine(string methodName)
    {
        return monoController.StartCoroutine(methodName);
    }

    #endregion

    #region 停止协程

    public static void StopCoroutine(IEnumerator coroutine)
    {
        monoController.StopCoroutine(coroutine);
    }

    public static void StopCoroutine(Coroutine coroutine)
    {
        monoController.StopCoroutine(coroutine);
    }

    public static void StopCoroutine(string methodName)
    {
        monoController.StopCoroutine(methodName);
    }

    public static void StopAllCoroutines()
    {
        monoController.StopAllCoroutines();
    }

    #endregion

    /// <summary>
    /// 销毁所有单例
    /// </summary>
    /// <param name="filterList">过滤列表</param>
    public static void DestoryAllSingleton(List<Type> filterList = null)
    {
        if (filterList != null)
        {
            List<Type> removeList = new List<Type>();
            foreach (Type type in singletons.Keys)
            {
                if (!filterList.Contains(type))
                {
                    removeList.Add(type);
                }
            }

            for (int i = 0; i < removeList.Count; i++)
            {
                DestorySingleton(removeList[i]);
#if UNITY_EDITOR
                Debug.Log($"The singleton has been deleted. SingletonName: {removeList[i].Name}");
#endif
            }
            removeList.Clear();
        }
        else
        {
            foreach (Type type in singletons.Keys)
            {
                DestorySingleton(type);
#if UNITY_EDITOR
                Debug.Log($"The singleton has been deleted. SingletonName: {type.Name}");
#endif
            }
            singletons.Clear();
            updateSingletons.Clear();
        }
    }

    /// <summary>
    /// 销毁单例管理器
    /// </summary>
    public static void Dispose()
    {
        if (!isInitialize) return;

        DestoryAllSingleton();
        if (monoController != null)
        {
            monoController.RemoveUpdateListener(OnUpdate);
            GameObject.Destroy(monoController.gameObject);
            monoController = null;
        }
        updateSingletons.Clear();
        singletons.Clear();
        isInitialize = false;
    }
}