using QZGameFramework.DebuggerSystem;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 游戏一开始就会创建的全局唯一的游戏管理器
/// 如有需要初始化的脚本、数据等，可以在这里进行初始化
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    private static bool isInit = false; // 是否已经初始化过

    // 新输入系统的键盘映射
    public PlayerInputAction playerInputAction;

    /// <summary>
    /// 饿汉单例:游戏运行时加载程序唯一全局单例管理器
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitalizeGameManager()
    {
        GameObject obj;
        // 已经初始化过 则不再进行初始化
        if (isInit)
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    obj = new GameObject("GameManager");
                    instance = obj.AddComponent<GameManager>();
                }
                DontDestroyOnLoad(instance.gameObject);
            }

            Debug.LogError("GameManager Has Initialized.");
            return;
        }

        obj = new GameObject("GameManager");
        instance = obj.AddComponent<GameManager>();
        DontDestroyOnLoad(obj);

        // 游戏数据 管理器等进行初始化
        //UIManager.Instance.ShowPanel<LoginPanel>();
        //MusicMgr.Instance.PlayGameMusic("BGM");
        //MusicMgr.Instance.PlayAmbientMusic("a");
        //InputManager.Instance.PushStack();
        SingletonManager.Initialize();

        // TODO: 是否启动Debugger模式
#if OPEN_LOG
        Debugger.InitDebuggerSystem();
#else
        if (Debugger.logConfig != null)
        {
            Debug.unityLogger.logEnabled = Debugger.logConfig.unityLoggerEnabled;
        }
#endif

        isInit = true;
    }

    private void Awake()
    {
        playerInputAction = new PlayerInputAction();
        // 从本地读取键盘设置
        string inputActionMapJson = PlayerPrefs.GetString("ActionMap", null);
        if (inputActionMapJson != null)
        {
            playerInputAction.asset.LoadBindingOverridesFromJson(inputActionMapJson);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            Destroy(this.gameObject);
        }
        instance = null;
    }
}