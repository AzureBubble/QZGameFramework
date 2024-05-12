using UnityEngine;

/// <summary>
/// 自动创建游戏对象的单例模式
/// </summary>
/// <typeparam name="T"></typeparam>
[DisallowMultipleComponent]
public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // 如果单例不存在，则自动在场景创建一个单例对象 不要命名空间路径
                GameObject obj = new GameObject(typeof(T).ToString().Substring(typeof(T).ToString().LastIndexOf(".") + 1));
                instance = obj.AddComponent<T>();
                // 过场景不移除
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}