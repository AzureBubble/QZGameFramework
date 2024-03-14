using UnityEngine;

/// <summary>
/// 继承 MonoBehaviour 的单例基类
/// </summary>
[DisallowMultipleComponent]
public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance => instance;

    protected virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this as T;
        // 过场景不移除
        DontDestroyOnLoad(this.gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}