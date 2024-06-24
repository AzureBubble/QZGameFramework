public abstract class Singleton<T> : ISingleton where T : class, ISingleton
{
    public bool IsDisposed { get; set; }

    private static volatile T instance;
    private static readonly object lockObject = new object();

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = SingletonManager.CreateSingleton<T>();
                        instance.Initialize();
                    }
                }
            }
            return instance;
        }
    }

    public virtual void Initialize()
    {
    }

    public virtual void Dispose()
    {
        IsDisposed = true;
        instance = null;
    }
}