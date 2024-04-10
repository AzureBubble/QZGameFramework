using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace QZGameFramework.GFSceneManager
{
    /// <summary>
    /// 场景切换管理者
    /// </summary>
    public class SceneMgr
    {
        private static event UnityAction<AsyncOperation> sceneLoadingAction;

        private static event UnityAction<float> OnSceneLoadingUniTask;

        private static event UnityAction OnAfterSceneLoadedUniTask;

        private static event UnityAction OnBeforeSceneUnloadedUniTask;

        #region 场景切换时机事件添加

        /// <summary>
        /// 添加 异步加载场景执行中的事件
        /// </summary>
        /// <param name="action"></param>
        public static void AddSceneLoadingAsyncEvent(UnityAction<AsyncOperation> action)
        {
            if (action != null)
                sceneLoadingAction += action;
        }

        /// <summary>
        /// 删除 异步加载场景执行中的事件
        /// </summary>
        /// <param name="action"></param>
        public static void RemoveSceneLoadingAsyncEvent(UnityAction<AsyncOperation> action)
        {
            if (action != null)
                sceneLoadingAction -= action;
        }

        /// <summary>
        /// 添加 异步卸载场景前的事件
        /// </summary>
        /// <param name="action"></param>
        public static void AddBeforeSceneUnloadedUniTaskEvent(UnityAction action)
        {
            if (action != null)
                OnBeforeSceneUnloadedUniTask += action;
        }

        /// <summary>
        /// 删除 异步卸载场景前的事件
        /// </summary>
        /// <param name="action"></param>
        public static void RemoveBeforeSceneUnloadedUniTaskEvent(UnityAction action)
        {
            if (action != null)
                OnBeforeSceneUnloadedUniTask -= action;
        }

        /// <summary>
        /// 添加 异步加载场景后的事件
        /// </summary>
        /// <param name="action"></param>
        public static void AddAfterSceneUnloadedUniTaskEvent(UnityAction action)
        {
            if (action != null)
                OnAfterSceneLoadedUniTask += action;
        }

        /// <summary>
        /// 删除 异步加载场景后的事件
        /// </summary>
        /// <param name="action"></param>
        public static void RemoveAfterSceneUnloadedUniTaskEvent(UnityAction action)
        {
            if (action != null)
                OnAfterSceneLoadedUniTask -= action;
        }

        /// <summary>
        /// 添加 异步加载场景执行中的事件
        /// </summary>
        /// <param name="action"></param>
        public static void AddSceneLoadingUniTaskEvent(UnityAction<float> action)
        {
            if (action != null)
                OnSceneLoadingUniTask += action;
        }

        /// <summary>
        /// 删除 异步加载场景执行中的事件
        /// </summary>
        /// <param name="action"></param>
        public static void RemoveSceneLoadingUniTaskEvent(UnityAction<float> action)
        {
            if (action != null)
                OnSceneLoadingUniTask += action;
        }

        /// <summary>
        /// 添加 当前活动场景发生变化时事件
        /// </summary>
        /// <param name="action"></param>
        public static void AddActiveSceneChangedEvent(UnityAction<Scene, Scene> action)
        {
            if (action != null)
                SceneManager.activeSceneChanged += action;
        }

        /// <summary>
        /// 删除 当前活动场景发生变化时事件
        /// </summary>
        /// <param name="action"></param>
        public static void RemoveActiveSceneChangedEvent(UnityAction<Scene, Scene> action)
        {
            if (action != null)
                SceneManager.activeSceneChanged += action;
        }

        /// <summary>
        /// 添加 场景加载前事件
        /// 新场景加载完成后，在新场景的Awake()方法执行之前被调用。
        /// </summary>
        /// <param name="action"></param>
        public static void AddBeforeSceneLoadEvent(UnityAction<Scene, LoadSceneMode> action)
        {
            if (action != null)
                SceneManager.sceneLoaded += action;
        }

        /// <summary>
        /// 删除 场景加载前事件
        /// </summary>
        /// <param name="action"></param>
        public static void RemoveBeforeSceneLoadEvent(UnityAction<Scene, LoadSceneMode> action)
        {
            if (action != null)
                SceneManager.sceneLoaded += action;
        }

        /// <summary>
        /// 添加 场景卸载后事件
        /// </summary>
        /// <param name="action"></param>
        public static void AddUnloadedSceneEvent(UnityAction<Scene> action)
        {
            if (action != null)
                SceneManager.sceneUnloaded += action;
        }

        /// <summary>
        /// 删除 场景卸载后事件
        /// </summary>
        /// <param name="action"></param>
        public static void RemoveUnloadedSceneEvent(UnityAction<Scene> action)
        {
            if (action != null)
                SceneManager.sceneUnloaded += action;
        }

        #endregion

        #region 根据场景索引加载场景

        /// <summary>
        /// 同步切换场景
        /// </summary>
        /// <param name="sceneBuildIndex">场景索引</param>
        public static void LoadScene(int sceneBuildIndex)
        {
            SceneManager.LoadScene(sceneBuildIndex);
        }

        /// <summary>
        /// 同步切换场景
        /// </summary>
        /// <param name="sceneBuildIndex">场景索引</param>
        /// <param name="mode">加载场景的方式</param>
        public static void LoadScene(int sceneBuildIndex, LoadSceneMode mode)
        {
            SceneManager.LoadScene(sceneBuildIndex);
        }

        /// <summary>
        /// 异步切换场景
        /// </summary>
        /// <param name="sceneBuildIndex">场景索引</param>
        /// <param name="mode">加载场景的方式</param>
        public static void LoadSceneAsync(int sceneBuildIndex, LoadSceneMode mode)
        {
            // SingletonManager 启动异步加载场景协程
            //SingletonManager.StartCoroutine(LoadSceneAsyncCoroutine(sceneBuildIndex, mode));
            LoadSceneAsyncByUniTask(sceneBuildIndex, mode).Forget();
        }

        /// <summary>
        /// 协程异步加载场景
        /// </summary>
        /// <param name="sceneBuildIndex">场景索引</param>
        /// <param name="mode">加载场景的方式</param>
        /// <returns></returns>
        private static IEnumerator LoadSceneAsyncCoroutine(int sceneBuildIndex, LoadSceneMode mode)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
            // 异步加载进度条
            while (!ao.isDone)
            {
                // 场景异步加载中事件
                sceneLoadingAction?.Invoke(ao);
                // 可在这里处理场景切换进度条问题
                yield return ao.progress;
            }
        }

        /// <summary>
        /// UniTask异步加载场景
        /// </summary>
        /// <param name="sceneBuildIndex">场景索引</param>
        /// <param name="mode">加载场景的方式</param>
        /// <returns></returns>
        private static async UniTaskVoid LoadSceneAsyncByUniTask(int sceneBuildIndex, LoadSceneMode mode)
        {
            OnBeforeSceneUnloadedUniTask?.Invoke();
            AsyncOperation ao = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
            ao.allowSceneActivation = false;
            await ao.ToUniTask(Progress.Create<float>(async progress =>
            {
                if (progress >= 0.9f)
                {
                    ao.allowSceneActivation = true;
                }

                OnSceneLoadingUniTask?.Invoke(progress);
                Debug.Log($"{SceneManager.GetSceneByBuildIndex(sceneBuildIndex).name} 场景加载进度 =============> {progress * 100}%");

                await UniTask.Yield();
            }));

            if (ao.isDone)
            {
                Debug.Log($"{SceneManager.GetSceneByBuildIndex(sceneBuildIndex).name} 场景加载进度 =============> 100%");

                OnSceneLoadingUniTask?.Invoke(1);
            }

            OnAfterSceneLoadedUniTask?.Invoke();
        }

        /// <summary>
        /// 异步切换场景
        /// </summary>
        /// <param name="sceneBuildIndex">场景索引</param>
        public static void LoadSceneAsync(int sceneBuildIndex)
        {
            // SingletonManager 启动异步加载场景协程
            //SingletonManager.StartCoroutine(LoadSceneAsyncCoroutine(sceneBuildIndex));
            LoadSceneAsyncByUniTask(sceneBuildIndex).Forget();
        }

        /// <summary>
        /// 协程异步加载场景
        /// </summary>
        /// <param name="sceneBuildIndex">场景索引</param>
        /// <returns></returns>
        private static IEnumerator LoadSceneAsyncCoroutine(int sceneBuildIndex)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(sceneBuildIndex);
            // 异步加载进度条
            while (!ao.isDone)
            {
                // 场景异步加载中事件
                sceneLoadingAction?.Invoke(ao);
                // 可在这里处理场景切换进度条问题
                yield return ao.progress;
            }
        }

        /// <summary>
        /// UniTask异步加载场景
        /// </summary>
        /// <param name="sceneBuildIndex">场景索引</param>
        /// <returns></returns>
        private static async UniTaskVoid LoadSceneAsyncByUniTask(int sceneBuildIndex)
        {
            OnBeforeSceneUnloadedUniTask?.Invoke();

            AsyncOperation ao = SceneManager.LoadSceneAsync(sceneBuildIndex);
            ao.allowSceneActivation = false;
            await ao.ToUniTask(Progress.Create<float>(async progress =>
            {
                if (progress >= 0.9f)
                {
                    ao.allowSceneActivation = true;
                }

                OnSceneLoadingUniTask?.Invoke(progress);
                Debug.Log($"{SceneManager.GetSceneByBuildIndex(sceneBuildIndex).name} 场景加载进度 =============> {progress * 100}%");

                await UniTask.Yield();
            }));

            if (ao.isDone)
            {
                Debug.Log($"{SceneManager.GetSceneByBuildIndex(sceneBuildIndex).name} 场景加载进度 =============> 100%");

                OnSceneLoadingUniTask?.Invoke(1);
            }

            OnAfterSceneLoadedUniTask?.Invoke();
        }

        #endregion

        #region 根据场景名字加载场景

        /// <summary>
        /// 同步切换场景
        /// </summary>
        /// <param name="sceneName">场景名字</param>
        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// 同步切换场景
        /// </summary>
        /// <param name="sceneName">场景名字</param>
        /// <param name="mode">加载场景的方式</param>
        public static void LoadScene(string sceneName, LoadSceneMode mode)
        {
            SceneManager.LoadScene(sceneName, mode);
        }

        /// <summary>
        /// 异步切换场景
        /// </summary>
        /// <param name="sceneName">场景名字</param>
        public static void LoadSceneAsync(string sceneName)
        {
            // SingletonManager 启动异步加载场景协程
            //SingletonManager.StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
            LoadSceneAsyncByUniTask(sceneName).Forget();
        }

        /// <summary>
        /// 协程异步加载场景
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <returns></returns>
        private static IEnumerator LoadSceneAsyncCoroutine(string sceneName)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
            // 异步加载进度条
            while (!ao.isDone)
            {
                // 场景异步加载中事件
                sceneLoadingAction?.Invoke(ao);
                // 可在这里处理场景切换进度条问题
                yield return ao.progress;
            }
        }

        /// <summary>
        /// UniTask异步加载场景
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <returns></returns>
        private static async UniTaskVoid LoadSceneAsyncByUniTask(string sceneName)
        {
            OnBeforeSceneUnloadedUniTask?.Invoke();
            AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
            ao.allowSceneActivation = false;
            await ao.ToUniTask(Progress.Create<float>(async progress =>
            {
                if (progress >= 0.9f)
                {
                    ao.allowSceneActivation = true;
                }

                OnSceneLoadingUniTask?.Invoke(progress);
                Debug.Log($"{sceneName} 场景加载进度 =============> {progress * 100}%");

                await UniTask.Yield();
            }));

            if (ao.isDone)
            {
                Debug.Log($"{sceneName} 场景加载进度 =============> 100%");

                OnSceneLoadingUniTask?.Invoke(1);
            }

            OnAfterSceneLoadedUniTask?.Invoke();
        }

        /// <summary>
        /// 异步切换场景
        /// </summary>
        /// <param name="sceneName">场景名字</param>
        /// <param name="mode">加载场景的方式</param>
        public static void LoadSceneAsync(string sceneName, LoadSceneMode mode)
        {
            // SingletonManager 启动异步加载场景协程
            //SingletonManager.StartCoroutine(LoadSceneAsyncCoroutine(sceneName, mode));
            LoadSceneAsyncByUniTask(sceneName, mode).Forget();
        }

        /// <summary>
        /// 协程异步加载场景
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="mode">加载场景的方式</param>
        /// <returns></returns>
        private static IEnumerator LoadSceneAsyncCoroutine(string sceneName, LoadSceneMode mode)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName, mode);
            // 异步加载进度条
            while (!ao.isDone)
            {
                // 场景异步加载中事件
                sceneLoadingAction?.Invoke(ao);
                // 可在这里处理场景切换进度条问题
                yield return ao.progress;
            }
        }

        /// <summary>
        /// UniTask异步加载场景
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="mode">加载场景的方式</param>
        /// <returns></returns>
        private static async UniTaskVoid LoadSceneAsyncByUniTask(string sceneName, LoadSceneMode mode)
        {
            OnBeforeSceneUnloadedUniTask?.Invoke();

            AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName, mode);
            ao.allowSceneActivation = false;
            await ao.ToUniTask(Progress.Create<float>(async progress =>
            {
                if (progress >= 0.9f)
                {
                    ao.allowSceneActivation = true;
                }

                OnSceneLoadingUniTask?.Invoke(progress);
                Debug.Log($"{sceneName} 场景加载进度 =============> {progress * 100}%");

                await UniTask.Yield();
            }));

            if (ao.isDone)
            {
                Debug.Log($"{sceneName} 场景加载进度 =============> 100%");

                OnSceneLoadingUniTask?.Invoke(1);
            }

            OnAfterSceneLoadedUniTask?.Invoke();
        }

        #endregion
    }
}