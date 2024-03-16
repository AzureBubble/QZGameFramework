using Cysharp.Threading.Tasks;
using QZGameFramework.PackageMgr.ResourcesManager;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Events;

namespace QZGameFramework.AutoUIManager
{
    public class UIManager : Singleton<UIManager>
    {
        public bool SingMaskSystem => mUIConfig.SINGMASK_SYSTEM; // UI遮罩模式开
        private UIConfig mUIConfig; // UI 配置文件
        private Camera mUICamera; // 场景 UI 相机
        private Transform mUIRoot; // UI 根物体

        private Dictionary<string, WindowBase> allWindowDic = new Dictionary<string, WindowBase>(); // 所有窗口的Dic key-窗口类名
        private List<WindowBase> allWindowList = new List<WindowBase>(); // 所有窗口的列表
        private List<WindowBase> visibleWindowList = new List<WindowBase>(); // 所有可见窗口的列表

        private Queue<WindowBase> windowStack = new Queue<WindowBase>(); // 队列 用来管理弹窗的循环弹出
        private bool mStartPopStackWndStatus = false; // 开始弹出堆栈的表只 可以用来处理多种情况 比如：正在出栈种有其他界面弹出 可以直接放到栈内进行弹出 等

        /// <summary>
        /// 初始化 UIModule 管理器方法
        /// </summary>
        public override void Initialize()
        {
            // 找到 UIRoot 物体 如果没有就动态创建
            mUIRoot = GameObject.Find("UIRoot")?.transform;
            if (mUIRoot == null)
            {
                GameObject rootObj = GameObject.Instantiate(ResourcesMgr.Instance.LoadRes<GameObject>("UI/UIRoot"));
                rootObj.name = "UIRoot";
                mUIRoot = rootObj.transform;
                GameObject.DontDestroyOnLoad(rootObj);
                // 找到UI渲染相机
                //mUICamera = GameObject.Find("UIRoot/UICamera").GetComponent<Camera>();
                mUICamera = rootObj.GetComponentInChildren<Camera>();
            }

            mUIConfig = ResourcesMgr.Instance.LoadRes<UIConfig>("UI/UIConfig/UIConfig");
            //在手机上不会触发调用
#if UNITY_EDITOR
            mUIConfig.GeneratorWindowConfig();
#endif
        }

        /// <summary>
        /// 预加载窗口，不调用生命周期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void PreLoadWindow<T>() where T : WindowBase, new()
        {
            Type type = typeof(T);
            string wndName = type.Name;
            T windowBase = new T();
            // 克隆界面，初始化界面信息
            // 1.生成对应的窗口预制体
            this.LoadWindowAsync(wndName, (nWnd) =>
            {
                // 2.初始出对应管理类
                if (nWnd != null)
                {
                    if (allWindowDic.ContainsKey(wndName))
                    {
                        GameObject.Destroy(nWnd);
                        Debug.Log("PreLoadWindow:" + wndName + " has exist.");
                        return;
                    }
                    windowBase.gameObject = nWnd;
                    windowBase.transform = nWnd.transform;
                    windowBase.Canvas = nWnd.GetComponent<Canvas>();
                    windowBase.Canvas.worldCamera = mUICamera;
                    windowBase.Name = nWnd.name;
                    windowBase.OnAwake();
                    windowBase.SetVisible(false);
                    RectTransform rectTrans = nWnd.GetComponent<RectTransform>();
                    rectTrans.anchorMax = Vector2.one;
                    rectTrans.offsetMax = Vector2.zero;
                    rectTrans.offsetMin = Vector2.zero;
                    allWindowDic.Add(wndName, windowBase);
                    allWindowList.Add(windowBase);
                }
                Debug.Log("PreLoadWindow:" + wndName);
            });
            // 同步方式
            // 1.生成对应的窗口预制体
            //GameObject nWnd = LoadWindow(wndName);
            // 2.初始出对应管理类
            //if (nWnd != null)
            //{
            //    windowBase.gameObject = nWnd;
            //    windowBase.transform = nWnd.transform;
            //    windowBase.Canvas = nWnd.GetComponent<Canvas>();
            //    windowBase.Canvas.worldCamera = mUICamera;
            //    windowBase.Name = nWnd.name;
            //    windowBase.OnAwake();
            //    windowBase.SetVisible(false);
            //    RectTransform rectTrans = nWnd.GetComponent<RectTransform>();
            //    rectTrans.anchorMax = Vector2.one;
            //    rectTrans.offsetMax = Vector2.zero;
            //    rectTrans.offsetMin = Vector2.zero;
            //    allWindowDic.Add(wndName, windowBase);
            //    allWindowList.Add(windowBase);
            //}
            //Debug.Log("PreLoadWindow:" + wndName);
        }

        #region 显示UI面板

        /// <summary>
        /// 通过泛型 同步显示面板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ShowWindow<T>() where T : WindowBase, new()
        {
            Type type = typeof(T);
            string windowName = type.Name;
            // 查找是否已经打开过 Window
            WindowBase window = this.GetWindow(windowName);
            if (window != null)
            {
                return this.ShowWindow(windowName) as T;
            }

            // 第一次调用窗口则进行创建
            T tWindow = new T();
            return this.InitializeWindow(tWindow, windowName) as T;
        }

        /// <summary>
        /// 通过泛型 异步显示面板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async UniTask<T> ShowWindowAsync<T>() where T : WindowBase, new()
        {
            Debug.Log("进行异步显示窗口");

            Type type = typeof(T);
            string windowName = type.Name;
            // 查找是否已经打开过 Window
            WindowBase window = this.GetWindow(windowName);
            if (window != null)
            {
                return await this.ShowWindowAsync(windowName) as T;
            }

            // 第一次调用窗口则进行创建
            return this.InitializeWindowAsync(new T(), windowName) as T;
        }

        /// <summary>
        /// 弹出一个窗口 并渲染在视窗最前面
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        private WindowBase ShowWindow(WindowBase window)
        {
            Type type = window.GetType();
            string windowName = type.Name;
            WindowBase wnd = GetWindow(windowName);
            if (wnd != null)
            {
                return ShowWindow(windowName);
            }

            return InitializeWindow(window, windowName);
        }

        private async UniTask<WindowBase> ShowWindowAsync(WindowBase window)
        {
            Type type = window.GetType();
            string windowName = type.Name;
            WindowBase wnd = GetWindow(windowName);
            if (wnd != null)
            {
                return await ShowWindowAsync(windowName);
            }

            return await InitializeWindowAsync(window, windowName);
        }

        /// <summary>
        /// 初始化新面板
        /// </summary>
        /// <param name="window">面板类</param>
        /// <param name="windowName">面板名</param>
        /// <returns></returns>
        private WindowBase InitializeWindow(WindowBase window, string windowName)
        {
            // 生成对应的窗口预制体
            GameObject windowObj = LoadWindow(windowName);

            // 初始化窗口管理类
            if (windowObj != null)
            {
                window.Name = windowName;
                window.gameObject = windowObj;
                window.transform = windowObj.transform;
                window.Canvas = windowObj.GetComponent<Canvas>();
                window.Canvas.worldCamera = mUICamera;
                window.transform.SetAsLastSibling();
                window.OnAwake();
                window.SetVisible(true);
                window.OnShow();
                RectTransform rectTrans = windowObj.GetComponent<RectTransform>();
                rectTrans.anchorMax = Vector2.one;
                rectTrans.offsetMax = Vector2.zero;
                rectTrans.offsetMin = Vector2.zero;
                allWindowDic.Add(windowName, window);
                allWindowList.Add(window);
                visibleWindowList.Add(window);
                SetWindowMaskVisible();
                return window;
            }

            Debug.LogError($"{windowName} window does not exist");
            return null;
        }

        /// <summary>
        /// 初始化新面板
        /// </summary>
        /// <param name="window">面板类</param>
        /// <param name="windowName">面板名</param>
        /// <returns></returns>
        private async UniTask<WindowBase> InitializeWindowAsync(WindowBase window, string windowName)
        {
            // 异步加载窗口预制体
            GameObject windowObj = await LoadWindowAsyncByUniTask(windowName);

            if (windowObj != null)
            {
                if (allWindowDic.ContainsKey(windowName))
                {
                    GameObject.Destroy(windowObj);
                    window = allWindowDic[windowName];
                    window.SetVisible(true);
                    window.OnShow();
                    visibleWindowList.Add(window);
                    SetWindowMaskVisible();
                    return window;
                }
                window.Name = windowName;
                window.gameObject = windowObj;
                window.transform = windowObj.transform;
                window.Canvas = windowObj.GetComponent<Canvas>();
                window.Canvas.worldCamera = mUICamera;
                window.transform.SetAsLastSibling();
                window.OnAwake();
                window.SetVisible(true);
                window.OnShow();
                RectTransform rectTrans = windowObj.GetComponent<RectTransform>();
                rectTrans.anchorMax = Vector2.one;
                rectTrans.offsetMax = Vector2.zero;
                rectTrans.offsetMin = Vector2.zero;
                allWindowDic.Add(windowName, window);
                allWindowList.Add(window);
                visibleWindowList.Add(window);
                SetWindowMaskVisible();

                return window;
            }

            Debug.LogError($"{windowName} window does not exist");
            return null;
        }

        private WindowBase ShowWindow(string windowName)
        {
            WindowBase window = null;
            // 已经打开过
            if (allWindowDic.ContainsKey(windowName))
            {
                window = allWindowDic[windowName];
                // 窗口存在 但没有显示
                if (window.gameObject != null && !window.Visible)
                {
                    // 添加到可见列表中管理
                    visibleWindowList.Add(window);
                    // 将窗口位置设置为最后一个 最优先渲染
                    window.transform.SetAsLastSibling();
                    window.SetVisible(true);
                    SetWindowMaskVisible();
                    window.OnShow();
                }
                return window;
            }
            else
            {
                Debug.LogError($"{windowName} window does not exist");
            }
            return null;
        }

        private async UniTask<WindowBase> ShowWindowAsync(string windowName)
        {
            await UniTask.SwitchToMainThread();
            WindowBase window = null;
            // 已经打开过
            if (allWindowDic.ContainsKey(windowName))
            {
                window = allWindowDic[windowName];
                // 窗口存在 但没有显示
                if (window.gameObject != null && !window.Visible)
                {
                    // 添加到可见列表中管理
                    visibleWindowList.Add(window);
                    // 将窗口位置设置为最后一个 最优先渲染
                    window.transform.SetAsLastSibling();
                    window.SetVisible(true);
                    SetWindowMaskVisible();
                    window.OnShow();
                }
                return window;
            }
            else
            {
                Debug.LogError($"{windowName} window does not exist");
            }
            return null;
        }

        #endregion

        #region 隐藏UI面板

        /// <summary>
        /// 通过泛型 隐藏面板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void HideWindow<T>() where T : WindowBase, new()
        {
            HideWindow(typeof(T).Name);
        }

        public void HideWindow(string windowName)
        {
            WindowBase window = visibleWindowList.Find(wnd =>
            {
                return wnd.Name == windowName;
            });

            HideWindow(window);
        }

        private void HideWindow(WindowBase window)
        {
            if (window != null && window.Visible)
            {
                // 移除可见列表中的窗口对象
                visibleWindowList.Remove(window);
                // 设置不可见
                window.SetVisible(false);
                SetWindowMaskVisible();
                window.OnHide();
            }
            // 在出栈的情况下 上一个窗口关闭 就弹出下一个窗口
            PopNextStackWindow(window);
        }

        #endregion

        #region 获取UI面板

        /// <summary>
        /// 通过泛型获取 面板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetWindow<T>() where T : WindowBase, new()
        {
            Type type = typeof(T);
            string windowName = type.Name;
            WindowBase window = visibleWindowList.Find(wnd =>
            {
                return wnd.Name == windowName;
            });

            if (window != null)
            {
                return window as T;
            }
            Debug.LogError($"{windowName} window not found");
            return null;
        }

        /// <summary>
        /// 通过面板的类名 获取面板
        /// </summary>
        /// <param name="windowName">面板类名</param>
        /// <returns></returns>
        private WindowBase GetWindow(string windowName)
        {
            if (allWindowDic.ContainsKey(windowName))
            {
                return allWindowDic[windowName];
            }
            return null;
        }

        #endregion

        #region 销毁UI面板

        /// <summary>
        /// 通过泛型 销毁UI面板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void DestroyWindow<T>() where T : WindowBase, new()
        {
            DestroyWindow(typeof(T).Name);
        }

        private void DestroyWindow(string windowName)
        {
            WindowBase window = GetWindow(windowName);
            DestroyWindow(window);
        }

        private void DestroyWindow(WindowBase window)
        {
            if (window == null) return;

            if (allWindowDic.ContainsKey(window.Name))
            {
                allWindowDic.Remove(window.Name);
                allWindowList.Remove(window);
                visibleWindowList.Remove(window);
            }

            window.SetVisible(false);
            SetWindowMaskVisible();
            window.OnHide();
            window.OnDestroy();
            GameObject.Destroy(window.gameObject);
            // 在出栈的情况下 上一个窗口销毁时 就弹出下一个窗口
            PopNextStackWindow(window);
        }

        /// <summary>
        /// 销毁所有的面板都销毁掉
        /// </summary>
        /// <param name="filterList">过滤列表</param>
        public void DestroyAllWindow(List<string> filterList = null)
        {
            // 反向循环进行边循环边删除 列表的本质是数组
            for (int i = allWindowList.Count - 1; i >= 0; i--)
            {
                WindowBase window = allWindowList[i];
                if (window == null || (filterList != null && filterList.Contains(window.Name)))
                {
                    continue;
                }
                DestroyWindow(window.Name);
            }
            // 注意释放资源的时机
            Resources.UnloadUnusedAssets();
        }

        #endregion

        #region 设置UI面板的遮罩相关

        private void SetWindowMaskVisible()
        {
            // 单遮和叠遮处理
            if (!SingMaskSystem)
            {
                return;
            }

            // 最高层级的 UI 面板
            WindowBase maxOrderWindowBase = null;
            int maxOrder = 0; // 最大渲染层级
            int maxIndex = 0; // 最大排序下标 在相同父节点下的位置下标

            for (int i = 0; i < visibleWindowList.Count; ++i)
            {
                WindowBase window = visibleWindowList[i];

                if ((window == null && window.gameObject == null)
                    || window.Canvas == null) continue;
                // 把所有的 面板的 遮罩设置为 False
                window.SetMaskVisible(false);

                // 找到层级最大的 UI 面板
                if (maxOrderWindowBase == null)
                {
                    maxOrderWindowBase = window;
                    maxOrder = window.Canvas.sortingOrder;
                    maxIndex = window.transform.GetSiblingIndex();
                }
                else
                {
                    // 找到最大渲染层级的窗口 拿到它
                    if (maxOrder < window.Canvas.sortingOrder)
                    {
                        maxOrderWindowBase = window;
                        maxOrder = window.Canvas.sortingOrder;
                    }
                    // 如果两个窗口的渲染层级相同 就找到同节点下最靠下一个物体 优先渲染Mask
                    else if (maxOrder == window.Canvas.sortingOrder && maxIndex < window.transform.GetSiblingIndex())
                    {
                        maxOrderWindowBase = window;
                        maxIndex = window.transform.GetSiblingIndex();
                    }
                }
            }

            // 打开层级最高的 UI 面板的 UIMask
            if (maxOrderWindowBase != null)
            {
                maxOrderWindowBase.SetMaskVisible(true);
            }
        }

        #endregion

        #region 堆栈系统

        /// <summary>
        /// 进栈一个界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="popCallBack"></param>
        public void PushWindowToStack<T>(Action<WindowBase> popCallBack = null) where T : WindowBase, new()
        {
            T wndBase = new T();
            wndBase.PopStackListener = popCallBack;
            windowStack.Enqueue(wndBase);
        }

        /// <summary>
        /// 弹出堆栈中第一个弹窗
        /// </summary>
        public void StartPopFirstStackWindow()
        {
            if (mStartPopStackWndStatus) return;
            mStartPopStackWndStatus = true;//已经开始进行堆栈弹出的流程，
            PopStackWindow();
        }

        /// <summary>
        /// 压入并且弹出堆栈弹窗
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="popCallBack"></param>
        public void PushAndPopStackWindow<T>(Action<WindowBase> popCallBack = null) where T : WindowBase, new()
        {
            PushWindowToStack<T>(popCallBack);
            StartPopFirstStackWindow();
        }

        /// <summary>
        /// 弹出堆栈中的下一个窗口
        /// </summary>
        /// <param name="windowBase"></param>
        private void PopNextStackWindow(WindowBase windowBase)
        {
            if (windowBase != null && mStartPopStackWndStatus && windowBase.PopStack)
            {
                windowBase.PopStack = false;
                PopStackWindow();
            }
        }

        /// <summary>
        /// 弹出堆栈弹窗
        /// </summary>
        /// <returns></returns>
        public bool PopStackWindow()
        {
            if (windowStack.Count > 0)
            {
                WindowBase window = windowStack.Dequeue();
                WindowBase popWindow = ShowWindow(window);
                popWindow.PopStackListener = window.PopStackListener;
                popWindow.PopStack = true;
                popWindow.PopStackListener?.Invoke(popWindow);
                popWindow.PopStackListener = null;
                return true;
            }
            else
            {
                mStartPopStackWndStatus = false;
                return false;
            }
        }

        /// <summary>
        /// 清空缓存队列
        /// </summary>
        public void ClearStackWindows()
        {
            windowStack.Clear();
        }

        #endregion

        /// <summary>
        /// 加载面板 GameObject
        /// </summary>
        /// <param name="windowName">面板名字</param>
        /// <returns></returns>
        private GameObject LoadWindow(string windowName)
        {
            GameObject window = GameObject.Instantiate(ResourcesMgr.Instance.LoadRes<GameObject>(
                mUIConfig.GetWindowPath(windowName)), mUIRoot);
            window.name = windowName;
            window.transform.localScale = Vector3.one;
            window.transform.localPosition = Vector3.zero;
            window.transform.rotation = Quaternion.identity;
            return window;
        }

        private void LoadWindowAsync(string windowName, UnityAction<GameObject> callback)
        {
            ResourcesMgr.Instance.LoadResAsync<GameObject>(
                mUIConfig.GetWindowPath(windowName), (window) =>
                {
                    GameObject windowObj = GameObject.Instantiate(window, mUIRoot);
                    windowObj.name = windowName;
                    windowObj.transform.localScale = Vector3.one;
                    windowObj.transform.localPosition = Vector3.zero;
                    windowObj.transform.rotation = Quaternion.identity;

                    callback?.Invoke(windowObj);
                });
        }

        private async UniTask<GameObject> LoadWindowAsyncByUniTask(string windowName)
        {
            var completionSource = new UniTaskCompletionSource<GameObject>();

            ResourcesMgr.Instance.LoadResAsync<GameObject>(mUIConfig.GetWindowPath(windowName), (window) =>
            {
                if (window != null)
                {
                    GameObject windowObj = GameObject.Instantiate(window, mUIRoot);
                    windowObj.name = windowName;
                    windowObj.transform.localScale = Vector3.one;
                    windowObj.transform.localPosition = Vector3.zero;
                    windowObj.transform.rotation = Quaternion.identity;

                    completionSource.TrySetResult(windowObj);
                }
                else
                {
                    Debug.LogError($"Failed to load window: {windowName}");
                    completionSource.TrySetResult(null);
                }
            });

            return await completionSource.Task;
        }

        public override void Dispose()
        {
            if (IsDisposed) return;
            //TODO:过场景销毁所有面板的时候 过来加载面板
            ResourcesMgr.Instance.UnloadAsset<GameObject>("UI/UIRoot", isDelImmediate: false);
            ResourcesMgr.Instance.UnloadAsset<UIConfig>("UI/UIConfig/UIConfig");
            DestroyAllWindow();
            ClearStackWindows();
            base.Dispose();
        }
    }
}