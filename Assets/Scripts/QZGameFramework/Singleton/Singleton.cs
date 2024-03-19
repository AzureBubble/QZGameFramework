using System;

public abstract class Singleton<T> : ISingleton where T : class, ISingleton
{
    private static Lazy<T> instance = new Lazy<T>(() => SingletonManager.CreateSingleton<T>());
    public static T Instance => instance.Value;

    public bool IsDisposed { get; set; }

    public virtual void Initialize()
    {
    }

    public virtual void Dispose()
    {
        IsDisposed = true;
        instance = new System.Lazy<T>(SingletonManager.CreateSingleton<T>());
    }

    //private static T instance;

    //public static T Instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //        {
    //            instance = SingletonManager.CreateSingleton<T>();
    //        }
    //        return instance;
    //    }
    //}
}

/// <summary>
/// 懒汉单例模式:第一次请求时 才会创建
/// </summary>
/// <typeparam name="T">必须有一个公共无参构造函数</typeparam>
//public class Singleton<T> where T : new()
//{
//    private static T instance;

//    public static T Instance
//    {
//        get
//        {
//            if (instance == null)
//            {
//                instance = new T();
//            }
//            return instance;
//        }
//    }
//}